using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Backend.Model;
using Backend.Model.Services;

namespace TestingDb
{
    public class InMemoryStudentDb
    {
        public static InMemoryStudentDb Empty => new InMemoryStudentDb(ImmutableDictionary<int, Student>.Empty);

        private readonly ImmutableDictionary<int, Student> _students;

        private InMemoryStudentDb(ImmutableDictionary<int, Student> students)
        {
            _students = students;
        }

        public (InMemoryStudentDb db, Student student) AddStudent(string name)
        {
            var id = GetAllStudents().Max(s => s.Id) + 1;
            var student = new Student(id, name);
            return (Add(student), student);
        }

        public InMemoryStudentDb AddStudents(IEnumerable<Student> students)
        {
            return !students.Any() ? this :
                Add(students.First()).AddStudents(students.Skip(1));
        }

        public InMemoryStudentDb RemoveStudent(int id)
        {
            return new InMemoryStudentDb(_students.Remove(id));
        }

        public Student GetStudent(int id)
        {
            return _students.TryGetValue(id, out Student student) ? 
                student : throw new ArgumentException("The specified student did not exist");
        }

        public IEnumerable<Student> GetAllStudents()
        {
            return _students.Values;
        }

        public InMemoryStudentDb UpdateStudent(Student student)
        {
            return RemoveStudent(student.Id)
                .Add(student);
        }

        public bool StudentExists(int id)
        {
            return _students.ContainsKey(id);
        }

        private InMemoryStudentDb Add(Student student)
        {
            return (new InMemoryStudentDb(_students.Add(student.Id, student)));
        }
    }

    internal static class StudentDbHelper
    {
        private static readonly Student[] _initialStudents = new[]
        {
            new Student(11, "Mr. Nice"),
            new Student(12, "Narco"),
            new Student(13, "Bombasto"),
            new Student(14, "Celeritas"),
            new Student(15, "Magneta"),
            new Student(16, "RubberMan"),
            new Student(17, "Dynama"),
            new Student(18, "Dr IQ"),
            new Student(19, "Magma"),
            new Student(20, "Tornado"),
        };

        public static IEnumerable<Student> Initial => _initialStudents;
        public static InMemoryStudentDb InitialDb =
            InMemoryStudentDb.Empty.AddStudents(Initial);
    }
}