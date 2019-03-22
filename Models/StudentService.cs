using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;
using TestingDb;

namespace Backend.Model.Services
{
    public class StudentWriter : ICommandService
    {
        private readonly IEventStoreConnection _store;

        public StudentWriter(IEventStoreConnection store)
        {
            _store = store;
        }

        public async Task<Student> CreateStudent(string name)
        {
            var created = new StudentCreated(new Student(name));

            await _store.AppendToStreamAsync(Streams.StudentStream, ExpectedVersion.Any, created.GenerateData());
            return created.Value;
        }

        public async Task UpdateStudent(Student student)
        {
            var updated = new StudentUpdated(student);
            await _store.AppendToStreamAsync(Streams.StudentStream, ExpectedVersion.Any, updated.GenerateData());
        }

        public async Task RemoveStudent(Guid id)
        {
            var removed = new StudentRemoved(id);
            await _store.AppendToStreamAsync(Streams.StudentStream, ExpectedVersion.Any, removed.GenerateData());
        }

        public async Task IssueCommand(ICommand c)
        {
            switch (c)
            {
                case CreateStudent create:
                    {
                        await CreateStudent(create.Name);
                        break;
                    }
                case UpdateStudent update:  
                    {
                        await UpdateStudent(new Student(update.Id, update.Name));
                        break;
                    }
                case RemoveStudent remove:
                    {
                        await RemoveStudent(remove.Value.Id);
                        break;
                    }
            }
        }
    }

    public class StudentReader : IViewService
    {
        private ImmutableDictionary<Guid, Student> _students = ImmutableDictionary<Guid, Student>.Empty;
        public IEnumerable<Student> AllStudents => _students.Values;

        public StudentReader()
        {
            
        }

        public Student GetStudent(Guid id)
        {
            return _students[id];
        }

        public IEnumerable<Student> SearchStudents(string name)
        {
            return _students.Values.Where(s => s.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        public bool StudentExists(Guid id)
        {
            return _students.ContainsKey(id);
        }

        public void RecordEvent(ResolvedEvent resolved)
        {
            var data = Encoding.UTF8.GetString(resolved.Event.Data);
            
            switch(resolved.Event.EventType)
            {
                case nameof(StudentCreated):
                {
                    var created = JsonConvert.DeserializeObject<StudentCreated>(data);
                    _students = _students.Add(created.Value.Id, created.Value);
                    break;
                }
                case nameof(StudentUpdated):
                {
                    var updated = JsonConvert.DeserializeObject<StudentUpdated>(data);
                    _students = _students.Remove(updated.Value.Id).Add(updated.Value.Id, updated.Value);
                    break;
                }
                case nameof(StudentRemoved):
                {
                    var removed = JsonConvert.DeserializeObject<StudentRemoved>(data);
                    _students = _students.Remove(removed.Id);
                    break;
                }
            }
        }
    }
}