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

            services.AddSingleton<EmployeeView, EmployeeView>(CreateEmployeeView);
            services.AddSingleton<EmployeeWriter, EmployeeWriter>();

            services.AddSingleton<UtoView, UtoView>(CreateRequestView);
            services.AddSingleton<UtoWriter, UtoWriter>();
        }

        private async static Task<IEventStoreConnection> ConnectToDataStore()
        {
            var connectionString = "ConnectTo=tcp://writer:ez1234@192.168.0.3:1113; HeartBeatTimeout=500";

            Console.WriteLine($"Connecting to event store at {connectionString}");

            var connSettings = ConnectionSettings.Create().UseConsoleLogger().EnableVerboseLogging();

            var conn = EventStoreConnection.Create(connectionString, connSettings);

            conn.Connected += OnConnected;
            conn.Disconnected += OnDisconnect;
            conn.ErrorOccurred += OnError;
            conn.Reconnecting += OnReconnect;

            await conn.ConnectAsync();

            Console.WriteLine($"Got Here: {conn.ConnectionName}");
            return conn;
        }

        private static void OnReconnect(object sender, ClientReconnectingEventArgs e)
        {
            Console.WriteLine($"Attempting to reconnect to event store");
        }

        private static void OnError(object sender, ClientErrorEventArgs e)
        {
            Console.WriteLine($"An error occurred: {e.Exception.Message}");
        }

        private static void OnDisconnect(object sender, ClientConnectionEventArgs e)
        {
            Console.Write("Disconnected from event store");
        }

        private static void OnConnected(object sender, ClientConnectionEventArgs e)
        {
            Console.WriteLine("Connected Successfully");
        }

        private static EmployeeView CreateEmployeeView(IServiceProvider provider)
        {
            var store = provider.GetService<IEventStoreConnection>();

            var reader = new EmployeeView();
            store.SubscribeToStreamFrom(
                Streams.Employee, StreamCheckpoint.StreamStart, CatchUpSubscriptionSettings.Default, (_, e) => reader.RecordEvent(e));

            return reader;
        }

        private static UtoView CreateRequestView(IServiceProvider provider)
        {
            var store = provider.GetService<IEventStoreConnection>();

            var reader = new UtoView();
            store.SubscribeToStreamFrom(
                Streams.UtoRequests, StreamCheckpoint.StreamStart, CatchUpSubscriptionSettings.Default, (_, e) => reader.RecordEvent(e));
            
            return reader;
        }

        private class ConsoleLogger : ILogger
        {
            public void Debug(string format, params object[] args)
            {
                Console.WriteLine(format, args);
            }

            public void Debug(Exception ex, string format, params object[] args)
            {
                Console.WriteLine(format, args);
            }

            public void Error(string format, params object[] args)
            {
                Console.WriteLine(format, args);
            }

            public void Error(Exception ex, string format, params object[] args)
            {
                Console.WriteLine(format, args);
            }

            public void Info(string format, params object[] args)
            {
                Console.WriteLine(format, args);
            }

            public void Info(Exception ex, string format, params object[] args)
            {
                Console.WriteLine(format, args);
            }
        }
    }
}