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
                var employees = await Task.Run(() => _employees.AllEmployees);
                return Ok(employees);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(
            [BindRequired] [FromRoute] int id)
        {
            if (!_employees.EmployeeExists((uint)id))
            {
                return NotFound(id);
            }

            var employee = await Task.Run(() => _employees.GetEmployee((uint)id));
            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee(
            [BindRequired] [FromBody] Employee employee)
        {
            if (employee.Name == null ||
                !employee.Name.Any())
            {
                return BadRequest();
            }

            var result = await _writer.CreateEmployee(employee.Name);

            return CreatedAtAction(nameof(AddEmployee), new { id = result.Id }, result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStudent(
            [BindRequired] [FromBody] Employee employee)
        {
            if (!_employees.EmployeeExists(employee.Id))
            {
                return NotFound(employee);
            }

            await Task.Run(() => _writer.UpdateEmployee(employee));

            return Ok(employee);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveStudent(
            [FromRoute] int id)
        {
            if (!_employees.EmployeeExists((uint)id))
            {
                return NotFound(id);
            }

            await Task.Run(() => _writer.RemoveEmployee((uint)id));
            return Ok();
        }
    }
}
