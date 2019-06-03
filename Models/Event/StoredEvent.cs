using System;
using System.Text;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;

namespace Backend.Model
{
    public class StoredEvent : IEvent, IComparable<StoredEvent>
    {
        public EventId Id{get;}
        public string EventName{get;}
        public byte[] Data{get;}

        public StoredEvent(EventId id, string eventName, byte[] data)
        {
            Id = id;
            EventName = eventName;
            Data = data;
        }

        public int CompareTo(StoredEvent other)
        {
            return Id.CompareTo(other.Id);
        }

        public bool IsType<T>()
         where T : IEvent
        {
            return EventName.Equals(typeof(T).Name, StringComparison.OrdinalIgnoreCase);
        }

        public T ToEvent<T>()
         where T : IEvent
        {
            var data = Encoding.UTF8.GetString(Data);
            var e = JsonConvert.DeserializeObject<T>(data);
            return e;
        }

        public static Action<BsonClassMap<StoredEvent>> GetClassMap()
        {
            return cm =>
            {
                cm.AutoMap();
                cm.MapCreator(p => new StoredEvent(p.Id, p.EventName, p.Data));
                cm.MapProperty(p => p.Id);
                cm.MapProperty(p => p.EventName);
                cm.MapProperty(p => p.Data);
            };
        }
    }
}