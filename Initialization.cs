using System;
using Backend.Model.Services;
using Microsoft.Extensions.DependencyInjection;
using Backend.Model;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Initialization
    {
        public static void InitializeStudents(this IServiceCollection services)
        {
            services.AddSingleton<IEventStoreConnection, IEventStoreConnection>(p => ConnectToDataStore().Result);

            services.AddSingleton<EmployeeView, EmployeeView>(p => CreateReader(p));

            services.AddSingleton<EmployeeWriter, EmployeeWriter>();
            services.AddTransient<ICommandService, CommandListenerCollection>(CreateCommandListeners);
        }

        private async static Task<IEventStoreConnection> ConnectToDataStore()
        {
            var conn = EventStoreConnection.Create(new Uri("tcp://writer:ez1234@localhost:1113"));
            await conn.ConnectAsync();
            return conn;
        }

        private static EmployeeView CreateReader(IServiceProvider provider)
        {
            var store = provider.GetService<IEventStoreConnection>();

            var reader = new EmployeeView();
            store.SubscribeToStreamFrom(
                Streams.Employee, StreamCheckpoint.StreamStart, CatchUpSubscriptionSettings.Default, (_, e) => reader.RecordEvent(e));

            return reader;
        }

        private static CommandListenerCollection CreateCommandListeners(IServiceProvider provider)
        {
            var studentWriter = provider.GetService<EmployeeWriter>();

            var collection = new CommandListenerCollection(new[] { studentWriter });
            return collection;
        }
    }
}