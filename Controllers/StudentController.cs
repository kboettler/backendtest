using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Model;
using Backend.Model.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TestingDb;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly StudentReader _students;
        private readonly StudentWriter _writer;

        public StudentController(StudentReader students, StudentWriter writer)
        {
            _students = students;
            _writer = writer;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudents(
            [FromQuery] string name)
        {
            if (name != null && name.Any())
            {
                var filtered = await Task.Run(() => _students.SearchStudents(name));
                return Ok(filtered);
            }
            else
            {
                var students = await Task.Run(() => _students.AllStudents);
                return Ok(students);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudent(
            [BindRequired] [FromRoute] int id)
        {
            if (!_students.StudentExists(id))
            {
                return NotFound(id);
            }

            var student = await Task.Run(() => _students.GetStudent(id));
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

            var result = await Task.Run(() => _writer.CreateStudent(student.Name));

            return CreatedAtAction(nameof(AddStudent), new { id = result.Id }, result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStudent(
            [BindRequired] [FromBody] Student student)
        {
            if (!_students.StudentExists(student.Id))
            {
                return NotFound(student);
            }

            await Task.Run(() => _writer.UpdateStudent(student));

            return Ok(student);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveStudent(
            [FromRoute] int id)
        {
            if (!_students.StudentExists(id))
            {
                return NotFound(id);
            }

            await Task.Run(() => _writer.RemoveStudent(id));
            return NoContent();
        }
    }
}
