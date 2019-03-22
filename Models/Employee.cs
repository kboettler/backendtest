using System;
using Backend.Model.Services;
using Newtonsoft.Json;

namespace Backend.Model
{
    public class Employee
    {
        public int Id { get; }
        public string Name { get; }

        public Employee(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class CreateEmployee : ICommand
    {
        public string Name { get; }
        public CreateEmployee(string name)
        {
            Name = name;
        }
    }

    public class EmployeeCreated : IEvent
    {
        public string Type => nameof(EmployeeCreated);
        public Employee Value { get; }

        public EmployeeCreated(Employee value)
        {
            Value = value;
        }
    }

    public class UpdateEmployee : ICommand
    {
        public int Id { get; }
        public string Name { get; }

        public UpdateEmployee(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class EmployeeUpdated : IEvent
    {
        public string Type => nameof(EmployeeUpdated);
        public Employee Value { get; }
        public EmployeeUpdated(Employee value)
        {
            Value = value;
        }
    }

    public class RemoveEmployee : ICommand
    {
        public Employee Value { get; }
        public RemoveEmployee(Employee value)
        {
            Value = value;
        }
    }

    public class EmployeeRemoved : IEvent
    {
        public string Type => nameof(EmployeeRemoved);
        public int Id { get; }
        public EmployeeRemoved(int id)
        {
            Id = id;
        }
    }
}