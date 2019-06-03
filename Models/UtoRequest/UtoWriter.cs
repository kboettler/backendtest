using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Backend.Model.Services
{
    public class UtoWriter
    {
        private readonly Func<EventWriter> _getStore;
        private int _maxId;

        public static UtoWriter Create(Func<EventReader> reader, Func<EventWriter> writer)
        {
            var lastCreated = reader().GetReverseOrderedCollection(Streams.UtoRequests).ToList();
                
                var next = lastCreated.FirstOrDefault(e => e.IsType<RequestCreated>());
            
            var maxId = next?.ToEvent<RequestCreated>()?.Value?.Id ?? 0;
                
            return new UtoWriter(maxId, writer);
        }

        private UtoWriter(int maxId, Func<EventWriter> store)
        {
            _maxId = maxId;
            _getStore = store;
        }

        public async Task<UtoRequest> CreateRequest(Employee employee, DateTime day, uint hours)
        {
            var request = new UtoRequest(++_maxId, employee, day, hours);
            var created = new RequestCreated(request);

            await _getStore().WriteEvent(created, Streams.UtoRequests);
            return request;
        }

        public async Task<IEnumerable<UtoRequest>> CreateRequests(Employee employee, DateTime start, DateTime end)
        {
            var requests = UtoRequest.Create(++_maxId, employee, start, end);
            var created = requests.Select(r => new RequestCreated(r));

            foreach(var e in created)
            {
                await _getStore().WriteEvent(e, Streams.UtoRequests);
            }
            return requests;
        }

        public async Task ApproveRequest(int id)
        {
            var approved = new RequestApproved(id);
            await _getStore().WriteEvent(approved, Streams.UtoRequests);
        }

        public async Task DenyRequest(int id)
        {
            var denied = new RequestDenied(id);
            await _getStore().WriteEvent(denied, Streams.UtoRequests);
        }

        public async Task DeleteRequest(int id)
        {
            var deleted = new RequestRemoved(id);
            await _getStore().WriteEvent(deleted, Streams.UtoRequests);
        }
    }
}