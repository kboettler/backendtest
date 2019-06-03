using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Model.Services;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Backend.Model
{
        public class EventWriter
    {
        private static readonly object _idLock = new object();
        private readonly IMongoDatabase _db;
        private readonly IListener _listener;

        public EventWriter(IMongoDatabase db, IListener listener)
        {
            _listener = listener;
            _db = db;
        }

        public async Task<StoredEvent> WriteEvent(IEvent e, string collection)
        {
            var id = BurnId();
            var stored = new StoredEvent(id, e.Type(), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(e)));

            await Task.Run(() => _db.GetCollection<StoredEvent>(collection).InsertOne(stored));
            _listener.RecordEvent(stored);
            return stored;
        }

        private EventId BurnId()
        {
            lock(_idLock)
            {
                var collection = _db.GetCollection<EventId>("MaxId");
                var max = collection.Find(FilterDefinition<EventId>.Empty).ToList().FirstOrDefault();

                if(max == null)
                {
                    max = EventId.Zero;
                }
                collection.DeleteOne(x => true);

                var next = max.Next;
                collection.InsertOne(next);

                return next;
            }
        }
    }
}