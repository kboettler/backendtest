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
        public static InMemoryStudentDb Empty => new InMemoryStudentDb(ImmutableDictionary<Guid, Student>.Empty);

        private readonly ImmutableDictionary<Guid, Student> _students;

        private InMemoryStudentDb(ImmutableDictionary<Guid, Student> students)
        {
            _students = students;
        }

        public (InMemoryStudentDb db, Student student) AddStudent(string name)
        {
            var student = new Student(name);
            return (Add(student), student);
        }

        public InMemoryStudentDb AddStudents(IEnumerable<Student> students)
        {
            return !students.Any() ? this :
                Add(students.First()).AddStudents(students.Skip(1));
        }

        public InMemoryStudentDb RemoveStudent(Guid id)
        {
            return new InMemoryStudentDb(_students.Remove(id));
        }

        public Student GetStudent(Guid id)
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

        public bool StudentExists(Guid id)
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
            new Student("Mr. Nice"),
            new Student("Narco"),
            new Student("Bombasto"),
            new Student("Celeritas"),
            new Student("Magneta"),
            new Student("RubberMan"),
            new Student("Dynama"),
            new Student("Dr IQ"),
            new Student("Magma"),
            new Student("Tornado"),
        };

        public static IEnumerable<Student> Initial => _initialStudents;
        public static InMemoryStudentDb InitialDb =
            InMemoryStudentDb.Empty.AddStudents(Initial);
    }
}