using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Backend.Model.Services
{
    public class EmployeeView : IViewService
    {
        private readonly IDictionary<int, Employee> _employees 
            = new Dictionary<int, Employee>();
        public IEnumerable<Employee> AllEmployees => _employees.Values;

        public Employee GetEmployee(int id)
        {
            if(!_employees.ContainsKey(id))
            {
                throw new ArgumentException("The specified employee did not exist");
            }

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
                        _employees.Add(created.Value.Id, created.Value);
                        break;
                    }
                case nameof(EmployeeUpdated):
                    {
                        var updated = JsonConvert.DeserializeObject<EmployeeUpdated>(data);
                        _employees.Remove(updated.Value.Id);
                        _employees.Add(updated.Value.Id, updated.Value);
                        break;
                    }
                case nameof(EmployeeRemoved):
                    {
                        var removed = JsonConvert.DeserializeObject<EmployeeRemoved>(data);
                        _employees.Remove(removed.Id);
                        break;
                    }
            }
        }
    }
}