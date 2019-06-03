using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;

namespace Backend.Model
{
    public class EventReader
    {
        private readonly IMongoDatabase _db;

        public EventReader(IMongoDatabase db)
        {
            _db = db;
        }

        public IEnumerable<StoredEvent> GetCollection(string collection)
        {
            return _db.GetCollection<StoredEvent>(collection)
            .Find(FilterDefinition<StoredEvent>.Empty)
            .ToEnumerable();
        }

        public IEnumerable<StoredEvent> GetOrderedCollection(string collection)
        {
            return _db.GetCollection<StoredEvent>(collection)
                .AsQueryable()
                .OrderBy(e => e.Id);
        }

        public IEnumerable<StoredEvent> GetReverseOrderedCollection(string collection)
        {
            return _db.GetCollection<StoredEvent>(collection)
            .AsQueryable()
            .OrderByDescending(e => e.Id);
        }
    }
}