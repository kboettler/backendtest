using Backend.Model.Services;

namespace Backend.Model
{
    public class Student
    {
        public int Id { get; }
        public string Name { get; }

        public Student(int id, string name)
        {
            Id = id;
            Name = name;
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
        public Student Value { get; }

        public StudentCreated(Student student)
        {
            Value = student;
        }
    }

    public class UpdateStudent : ICommand
    {
        public int Id { get; }
        public string Name { get; }

        public UpdateStudent(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class StudentUpdated : IEvent
    {
        public Student Value { get; }
        public StudentUpdated(Student student)
        {
            Value = student;
        }
    }

    public class RemoveStudent : ICommand
    {
        public Student Value { get; }
        public RemoveStudent(Student student)
        {
            Value = student;
        }
    }

    public class StudentRemoved : IEvent
    {
        public Student Value { get; }
        public StudentRemoved(Student student)
        {
            Value = student;
        }
    }
}