using System.Collections.Generic;
using System.Collections.Immutable;

namespace Backend.Model
{
    public class House
    {
        public static House Hufflepuff => new House(nameof(Hufflepuff));
        public static House Slytherin => new House(nameof(Slytherin));

        private readonly ImmutableList<PointEvent> _points;
        private readonly ImmutableList<Student> _students;

        public string Name { get; }
        public IEnumerable<Student> Students => _students;

        private House(string name)
            : this(name, ImmutableList<Student>.Empty, ImmutableList<PointEvent>.Empty)
        {

        }

        private House(string name, ImmutableList<Student> students, ImmutableList<PointEvent> points)
        {
            Name = name;
            _students = students;
            _points = points;
        }

        public House AddStudent(Student student)
        {
            return new House(Name, _students.Add(student), _points);
        }

        public House AwardPoints(Student student, int points)
        {
            return new House(Name, _students, _points.Add(new PointEvent(student, points)));
        }
    }

    public class PointEvent
    {
        public int Amount { get; }
        public Student Recipient { get; }

        public PointEvent(Student student, int points)
        {
            Amount = points;
            Recipient = student;
        }
    }
}