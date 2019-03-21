using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using TestingDb;

namespace Backend.Model.Services
{
    public class StudentWriter : ICommandService
    {
        private InMemoryStudentDb _db = InMemoryStudentDb.Empty;
        private readonly IEventService _eventService;

        public StudentWriter(IEventService eventService, InMemoryStudentDb initial)
        {
            _eventService = eventService;
            _db = initial;
        }

        public Student CreateStudent(string name)
        {
            var result = _db.AddStudent(name);
            _db = result.db;
            _eventService.RecordEvent(new StudentCreated(result.student));

            return result.student;
        }

        public void UpdateStudent(Student student)
        {
            _db = _db.UpdateStudent(student);
            _eventService.RecordEvent(new StudentUpdated(student));
        }

        public void RemoveStudent(int id)
        {
            var student = _db.GetStudent(id);
            _db = _db.RemoveStudent(id);

            _eventService.RecordEvent(new StudentRemoved(student));
        }

        public void IssueCommand(ICommand c)
        {
            switch (c)
            {
                case CreateStudent create:
                    {
                        CreateStudent(create.Name);
                        break;
                    }
                case UpdateStudent update:  
                    {
                        UpdateStudent(new Student(update.Id, update.Name));
                        break;
                    }
                case RemoveStudent remove:
                    {
                        RemoveStudent(remove.Value.Id);
                        break;
                    }
            }
        }
    }

    public class StudentReader : IEventService
    {
        private ImmutableDictionary<int, Student> _students = ImmutableDictionary<int, Student>.Empty;

        public IEnumerable<Student> AllStudents => _students.Values;

        public Student GetStudent(int id)
        {
            return _students[id];
        }

        public IEnumerable<Student> SearchStudents(string name)
        {
            return _students.Values.Where(s => s.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        public bool StudentExists(int id)
        {
            return _students.ContainsKey(id);
        }

        public void RecordEvent(IEvent e)
        {
            switch (e)
            {
                case StudentCreated created:
                    {
                        _students = _students.Add(created.Value.Id, created.Value);
                        break;
                    }
                case StudentUpdated updated:
                    {
                        _students = _students.Remove(updated.Value.Id)
                            .Add(updated.Value.Id, updated.Value);
                        break;
                    }
                case StudentRemoved removed:
                    {
                        _students = _students.Remove(removed.Value.Id);
                        break;
                    }
            }
        }
    }
}