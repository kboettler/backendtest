using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Backend.Model.Services
{
    public class EmployeeWriter
    {
        public static EmployeeWriter Create(Func<EventReader> reader, Func<EventWriter> writer)
        {
            var lastCreated = reader().GetReverseOrderedCollection(Streams.Employee)
                .FirstOrDefault(e => e.IsType<EmployeeCreated>());
                
            var maxId = lastCreated?.ToEvent<EmployeeCreated>()?.Value?.Id ?? 0;

            return new EmployeeWriter(maxId, writer);
        }

        private uint _maxId;
        private readonly Func<EventWriter> _getStore;

        private EmployeeWriter(uint maxId, Func<EventWriter> getStore)
        {
            _maxId = maxId;
            _getStore = getStore;
        }

        public async Task<Employee> CreateEmployee(string name)
        {
            var created = new EmployeeCreated(new Employee(++_maxId, name));

            await _getStore().WriteEvent(created, Streams.Employee);
            return created.Value;
        }

        public async Task UpdateEmployee(Employee employee)
        {
            var updated = new EmployeeUpdated(employee);
            await _getStore().WriteEvent(updated, Streams.Employee);
        }

        public async Task RemoveEmployee(uint id)
        {
            var removed = new EmployeeRemoved(id);
            await _getStore().WriteEvent(removed, Streams.Employee);
        }
    }
}