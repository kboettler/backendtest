using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization;

namespace Backend.Model
{
    public interface IListener
    {
        void RecordEvent(StoredEvent stored);
    }

    public sealed class ListenerCollection : IListener
    {
        private readonly IEnumerable<IListener> _listeners;

        public ListenerCollection(IEnumerable<IListener> listeners)
        {
            _listeners = listeners;
        }

        public void RecordEvent(StoredEvent stored)
        {
            foreach(var listener in _listeners)
            {
                listener.RecordEvent(stored);
            }
        }
    }

    public static class ServiceHelpers
    {
        public static string Type(this IEvent e)
        {
            return e.GetType().Name;
        }
    }

    public class MongoConfiguration
    {
        public string ConnectionString{get;}
        public string Database{get;}

        public static MongoConfiguration Create(IConfiguration config)
        {
            BsonClassMap.RegisterClassMap<EventId>(EventId.GetClassMap());
            BsonClassMap.RegisterClassMap<StoredEvent>(StoredEvent.GetClassMap());

            var connection = config.GetConnectionString("Datastore");
            var db = config.GetChildren().Single(c => c.Key.Equals("DatabaseName")).Value;

            return new MongoConfiguration(connection, db);
        }

        private MongoConfiguration(string connection, string db)
        {
            ConnectionString = connection;
            Database = db;
        }
    }
}