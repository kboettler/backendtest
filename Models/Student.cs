using System;
using Backend.Model.Services;
using Newtonsoft.Json;

namespace Backend.Model
{
    public class Student
    {
        public Guid Id { get; }
        public string Name { get; }

        public Student(string name)
            : this(Guid.NewGuid(), name)
        {
        }

        public Student(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        [JsonConstructor]
        public Student(string id, string name)
            : this(Guid.Parse(id), name)
        {

        }
    }

    public class CreateStudent : ICommand
    {
        public string Name { get; }
        public CreateStudent(string name)
        {
            Name = name;
        }
    }

    public class StudentCreated : IEvent
    {
        public string Type => nameof(StudentCreated);
        public Student Value { get; }

        public StudentCreated(Student value)
        {
            Value = value;
        }
    }

    public class UpdateStudent : ICommand
    {
        public Guid Id { get; }
        public string Name { get; }

        public UpdateStudent(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class StudentUpdated : IEvent
    {
        public string Type => nameof(StudentUpdated);
        public Student Value { get; }
        public StudentUpdated(Student value)
        {
            Value = value;
        }
    }

    public class RemoveStudent : ICommand
    {
        public Student Value { get; }
        public RemoveStudent(Student value)
        {
            Value = value;
        }
    }

    public class StudentRemoved : IEvent
    {
        public string Type => nameof(StudentRemoved);
        public Guid Id { get; }
        public StudentRemoved(Guid id)
        {
            Id = id;
        }
    }
}