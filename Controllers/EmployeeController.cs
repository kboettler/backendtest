using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Model;
using Backend.Model.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeView _employees;
        private readonly EmployeeWriter _writer;

        public EmployeeController(EmployeeView employees, EmployeeWriter writer)
        {
            _employees = employees;
            _writer = writer;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees(
            [FromQuery] string name)
        {
            if (name != null && name.Any())
            {
                var filtered = await Task.Run(() => _employees.SearchEmployees(name));
                return Ok(filtered);
            }
            else
            {
                var students = await Task.Run(() => _employees.AllEmployees);
                return Ok(students);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(
            [BindRequired] [FromRoute] int id)
        {
            if (!_employees.EmployeeExists(id))
            {
                return NotFound(id);
            }

            var student = await Task.Run(() => _employees.GetEmployee(id));
            return Ok(student);
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee(
            [BindRequired] [FromBody] Employee student)
        {
            if (student.Name == null ||
                !student.Name.Any())
            {
                return BadRequest();
            }

            var result = await _writer.CreateEmployee(student.Name);

            return CreatedAtAction(nameof(AddEmployee), new { id = result.Id }, result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStudent(
            [BindRequired] [FromBody] Employee student)
        {
            if (!_employees.EmployeeExists(student.Id))
            {
                return NotFound(student);
            }

            await Task.Run(() => _writer.UpdateEmployee(student));

            return Ok(student);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveStudent(
            [FromRoute] int id)
        {
            if (!_employees.EmployeeExists(id))
            {
                return NotFound(id);
            }

            await Task.Run(() => _writer.RemoveEmployee(id));
            return Ok();
        }
    }
}
