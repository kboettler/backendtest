using System;
using MongoDB.Bson.Serialization;

namespace Backend.Model
{
    public class EventId : IComparable<EventId>
    {
        public static EventId Zero => new EventId(0);

        public EventId Next => new EventId(_id + 1);

        private readonly long _id;

        private EventId(long id)
        {
            _id = id;
        }

        public int CompareTo(EventId other)
        {
            return _id.CompareTo(other._id);
        }

        public static Action<BsonClassMap<EventId>> GetClassMap()
        {
            return cm =>
            {
                cm.AutoMap();
                cm.MapCreator(p => new EventId(p._id));
                cm.MapField(p => p._id);
            };
        }
    }
}