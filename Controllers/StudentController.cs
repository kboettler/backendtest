using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TestingDb;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private static InMemoryStudentDb _db = StudentDbHelper.InitialDb;

        [HttpGet]
        public async Task<IActionResult> GetStudents(
            [FromQuery] string name)
        {
            var students = await Task.Run(() => _db.GetAllStudents());

            if (name != null && name.Any())
            {
                var filtered = await Task.Run(() => students.Where(s =>
                    s.Name.Contains(name, StringComparison.OrdinalIgnoreCase)));
                return Ok(filtered);
            }

            return Ok(students);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudent(
            [BindRequired] [FromRoute] int id)
        {
            if (!_db.StudentExists(id))
            {
                return NotFound(id);
            }

            var student = await Task.Run(() => _db.GetStudent(id));
            return Ok(student);
        }

        [HttpPost]
        public async Task<IActionResult> AddStudent(
            [BindRequired] [FromBody] Student student)
        {
            if (student.Name == null ||
                !student.Name.Any())
            {
                return BadRequest();
            }

            var result = await Task.Run(() => _db.AddStudent(student));
            _db = result.db;

            return CreatedAtAction(nameof(AddStudent), new { id = result.id }, new Student(result.id, student.Name));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStudent(
            [BindRequired] [FromBody] Student student)
        {
            if (!_db.StudentExists(student.Id))
            {
                return NotFound(student);
            }

            await Task.Run(() => _db = _db.UpdateStudent(student));

            return Ok(student);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveStudent(
            [FromRoute] int id)
        {
            if (!_db.StudentExists(id))
            {
                return NotFound(id);
            }

            await Task.Run(() => _db = _db.RemoveStudent(id));
            return NoContent();
        }
    }
}
