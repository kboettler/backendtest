using System;
using Backend.Model.Services;
using Microsoft.Extensions.DependencyInjection;
using Backend.Model;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using System.Threading.Tasks;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Initialization
    {
        public static void InitializeStudents(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<MongoConfiguration, MongoConfiguration>(p => MongoConfiguration.Create(config));
            services.AddTransient<IMongoDatabase, IMongoDatabase>(ConnectToDataStore);

            services.AddSingleton<EmployeeView, EmployeeView>(p => CreateListener<EmployeeView>(p, Streams.Employee));
            services.AddSingleton<UtoView, UtoView>(p => CreateListener<UtoView>(p, Streams.UtoRequests));

            services.AddSingleton<IListener, ListenerCollection>(CreateListenerCollection);

            services.AddTransient<EventReader, EventReader>(p => new EventReader(p.GetService<IMongoDatabase>()));
            services.AddTransient<EventWriter, EventWriter>(p => new EventWriter(p.GetService<IMongoDatabase>(), p.GetService<IListener>()));

            services.AddSingleton<EmployeeWriter, EmployeeWriter>(CreateEmployeeWriter);
            services.AddSingleton<UtoWriter, UtoWriter>(CreateUtoWriter);
        }


        private static EmployeeWriter CreateEmployeeWriter(IServiceProvider p)
        {
            return EmployeeWriter.Create(p.GetService<EventReader>, p.GetService<EventWriter>);
        }

        private static UtoWriter CreateUtoWriter(IServiceProvider p)
        {
            return UtoWriter.Create(p.GetService<EventReader>, p.GetService<EventWriter>);
        }

        private static IMongoDatabase ConnectToDataStore(IServiceProvider p)
        {
            var config = p.GetService<MongoConfiguration>();

            var client = new MongoClient(config.ConnectionString);
            var db = client.GetDatabase(config.Database);

            return db;
        }

        private static T CreateListener<T>(IServiceProvider p, string collection)
            where T : IListener, new()
        {
            var eReader = p.GetService<EventReader>();

            var view = new T();
            foreach(var e in eReader.GetOrderedCollection(collection))
            {
                view.RecordEvent(e);
            }

            return view;
        }

        private static ListenerCollection CreateListenerCollection(IServiceProvider p)
        {
            var type = typeof(IListener);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => type.IsAssignableFrom(t) && !t.IsInterface && !t.IsAssignableFrom(typeof(ListenerCollection)));
            
            var services = types.Select(t => p.GetService(t) as IListener)
                .Where(s => s != null);

            return new ListenerCollection(services);
        }
    }
}