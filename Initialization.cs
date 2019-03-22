using System;
using Backend.Model.Services;
using TestingDb;
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
            var credentials = new UserCredentials("writer", "ez1234");

            services.AddSingleton<IEventStoreConnection, IEventStoreConnection>(p => ConnectToDataStore().Result);

            services.AddSingleton<StudentReader, StudentReader>(p => CreateReader(p, credentials));

            services.AddSingleton<StudentWriter, StudentWriter>();
            services.AddTransient<ICommandService, CommandListenerCollection>(CreateCommandListeners);
        }

        private async static Task<IEventStoreConnection> ConnectToDataStore()
        {
            var conn = EventStoreConnection.Create(new Uri("tcp://writer:ez1234@localhost:1113"));
            await conn.ConnectAsync();
            return conn;
        }

        private static StudentReader CreateReader(IServiceProvider provider, UserCredentials credentials)
        {
            var store = provider.GetService<IEventStoreConnection>();

            var reader = new StudentReader();
            store.SubscribeToStreamFrom(Streams.StudentStream, StreamCheckpoint.StreamStart, CatchUpSubscriptionSettings.Default, (_, e) => reader.RecordEvent(e));

            return reader;
        }

        private static CommandListenerCollection CreateCommandListeners(IServiceProvider provider)
        {
            var studentWriter = provider.GetService<StudentWriter>();

            var collection = new CommandListenerCollection(new[] { studentWriter });
            return collection;
        }
    }
}