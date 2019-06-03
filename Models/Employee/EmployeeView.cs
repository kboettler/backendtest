using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Backend.Model.Services
{
    public class EmployeeView : IListener
    {
        private readonly IDictionary<uint, Employee> _employees 
            = new Dictionary<uint, Employee>();
        public IEnumerable<Employee> AllEmployees => _employees.Values;

        public Employee GetEmployee(uint id)
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

        public bool EmployeeExists(uint id)
        {
            return _employees.ContainsKey(id);
        }

        public void RecordEvent(StoredEvent stored)
        {
            switch (stored.EventName)
            {
                case nameof(EmployeeCreated):
                    {
                        var created = stored.ToEvent<EmployeeCreated>();
                        _employees.Add(created.Value.Id, created.Value);
                        break;
                    }
                case nameof(EmployeeUpdated):
                    {
                        var updated = stored.ToEvent<EmployeeUpdated>();
                        _employees.Remove(updated.Value.Id);
                        _employees.Add(updated.Value.Id, updated.Value);
                        break;
                    }
                case nameof(EmployeeRemoved):
                    {
                        var removed = stored.ToEvent<EmployeeRemoved>();
                        _employees.Remove(removed.Id);
                        break;
                    }
            }
        }
    }
}