using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Backend.Model.Services
{
    public class UtoWriter
    {
        private readonly IEventStoreConnection _store;
        private int _maxId;

        public UtoWriter(IEventStoreConnection store)
        {
            _store = store;
            var sub =_store.SubscribeToStreamFrom(
                Streams.UtoRequests, StreamCheckpoint.StreamStart, CatchUpSubscriptionSettings.Default, (_, e) => RecordEvent(e));
        }

        public async Task<UtoRequest> CreateRequest(Employee employee, DateTime day, uint hours)
        {
            var request = new UtoRequest(_maxId + 1, employee, day, hours);
            var created = new RequestCreated(request);

            await _store.AppendToStreamAsync(Streams.UtoRequests, ExpectedVersion.Any, created.GenerateData());
            return request;
        }

        public async Task<IEnumerable<UtoRequest>> CreateRequests(Employee employee, DateTime start, DateTime end)
        {
            var requests = UtoRequest.Create(_maxId + 1, employee, start, end);
            var created = requests.Select(r => new RequestCreated(r));

            await _store.AppendToStreamAsync(Streams.UtoRequests, ExpectedVersion.Any, created.Select(c => c.GenerateData()));
            return requests;
        }

        public async Task ApproveRequest(int id)
        {
            var approved = new RequestApproved(id);
            await _store.AppendToStreamAsync(Streams.UtoRequests, ExpectedVersion.StreamExists, approved.GenerateData());
        }

        public async Task DenyRequest(int id)
        {
            var denied = new RequestDenied(id);
            await _store.AppendToStreamAsync(Streams.UtoRequests, ExpectedVersion.StreamExists, denied.GenerateData());
        }

        public async Task DeleteRequest(int id)
        {
            var deleted = new RequestRemoved(id);
            await _store.AppendToStreamAsync(Streams.UtoRequests, ExpectedVersion.StreamExists, deleted.GenerateData());
        }

        private void RecordEvent(ResolvedEvent resolved)
        {
            if (resolved.Event.EventType.Equals(nameof(RequestCreated)))
            {
                var data = Encoding.UTF8.GetString(resolved.Event.Data);
                var created = JsonConvert.DeserializeObject<RequestCreated>(data);
                _maxId = _maxId > created.Value.Id ? _maxId : created.Value.Id;
            }
        }
    }
}