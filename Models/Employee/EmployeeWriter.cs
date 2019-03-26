using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;

namespace Backend.Model.Services
{
    public class EmployeeWriter
    {
        private readonly IEventStoreConnection _store;
        private int _maxId;

        public EmployeeWriter(IEventStoreConnection store)
        {
            _store = store;
            _store.SubscribeToStreamFrom(
                Streams.Employee, StreamCheckpoint.StreamStart, CatchUpSubscriptionSettings.Default, (_, e) => RecordEvent(e));
        }

        public async Task<Employee> CreateEmployee(string name)
        {
            var created = new EmployeeCreated(new Employee(_maxId + 1, name));

            await _store.AppendToStreamAsync(Streams.Employee, ExpectedVersion.Any, created.GenerateData());
            return created.Value;
        }

        public async Task UpdateEmployee(Employee student)
        {
            var updated = new EmployeeUpdated(student);
            await _store.AppendToStreamAsync(Streams.Employee, ExpectedVersion.StreamExists, updated.GenerateData());
        }

        public async Task RemoveEmployee(int id)
        {
            var removed = new EmployeeRemoved(id);
            await _store.AppendToStreamAsync(Streams.Employee, ExpectedVersion.StreamExists, removed.GenerateData());
        }

        private void RecordEvent(ResolvedEvent resolved)
        {
            if (resolved.Event.EventType.Equals(nameof(EmployeeCreated)))
            {
                var data = Encoding.UTF8.GetString(resolved.Event.Data);
                var created = JsonConvert.DeserializeObject<EmployeeCreated>(data);
                _maxId = _maxId > created.Value.Id ? _maxId : created.Value.Id;
            }
        }
    }
}