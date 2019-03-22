using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Backend.Model.Services
{
    public class EmployeeView : IViewService
    {
        private ImmutableDictionary<int, Employee> _employees = ImmutableDictionary<int, Employee>.Empty;
        public IEnumerable<Employee> AllStudents => _employees.Values;

        public EmployeeView()
        {

        }

        public Employee GetEmployee(int id)
        {
            return _employees[id];
        }

        public IEnumerable<Employee> SearchEmployees(string name)
        {
            return _employees.Values.Where(s => s.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        public bool EmployeeExists(int id)
        {
            return _employees.ContainsKey(id);
        }

        public void RecordEvent(ResolvedEvent resolved)
        {
            var data = Encoding.UTF8.GetString(resolved.Event.Data);

            switch (resolved.Event.EventType)
            {
                case nameof(EmployeeCreated):
                    {
                        var created = JsonConvert.DeserializeObject<EmployeeCreated>(data);
                        _employees = _employees.Add(created.Value.Id, created.Value);
                        break;
                    }
                case nameof(EmployeeUpdated):
                    {
                        var updated = JsonConvert.DeserializeObject<EmployeeUpdated>(data);
                        _employees = _employees.Remove(updated.Value.Id).Add(updated.Value.Id, updated.Value);
                        break;
                    }
                case nameof(EmployeeRemoved):
                    {
                        var removed = JsonConvert.DeserializeObject<EmployeeRemoved>(data);
                        _employees = _employees.Remove(removed.Id);
                        break;
                    }
            }
        }
    }
}