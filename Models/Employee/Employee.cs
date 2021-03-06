using System;
using Backend.Model.Services;
using Newtonsoft.Json;

namespace Backend.Model
{
    public class Employee : IEquatable<Employee>
    {
        public uint Id { get; }
        public string Name { get; }

        public Employee(uint id, string name)
        {
            Id = id;
            Name = name;
        }

        public bool Equals(Employee other)
        {
            return other.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class EmployeeCreated : IEvent
    {
        public Employee Value { get; }

        public EmployeeCreated(Employee value)
        {
            Value = value;
        }
    }

    public class EmployeeUpdated : IEvent
    {
        public Employee Value { get; }
        public EmployeeUpdated(Employee value)
        {
            Value = value;
        }
    }

    public class EmployeeRemoved : IEvent
    {
        public uint Id { get; }
        public EmployeeRemoved(uint id)
        {
            Id = id;
        }
    }
}