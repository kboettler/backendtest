using System;
using System.Threading.Tasks;
using Backend.Model;
using Backend.Model.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtoRequestController : ControllerBase
    {
        private readonly EmployeeView _employees;
        private readonly UtoView _requests;
        private readonly UtoWriter _writer;

        public UtoRequestController(UtoView requests, UtoWriter writer, EmployeeView employees)
        {
            _requests = requests;
            _writer = writer;
            _employees = employees;
        }

        [HttpGet]
        public async Task<IActionResult> GetRequests(
            [BindRequired] [FromQuery] int employeeId)
        {
            if (!_employees.EmployeeExists(employeeId))
            {
                return NotFound(employeeId);
            }

            var employee = await Task.Run(() => _employees.GetEmployee(employeeId));
            var requests = await Task.Run(() => _requests.GetRequests(employee));

            return Ok(requests);
        }

        [HttpGet("{requestId}")]
        public async Task<IActionResult> GetRequest(
            [BindRequired] [FromRoute] int requestId)
        {
            if (!_requests.RequestExists(requestId))
            {
                return NotFound(requestId);
            }

            var request = await Task.Run(() => _requests.GetRequest(requestId));
            var status = await Task.Run(() => _requests.GetStatus(request));

            return Ok(new {Request = request, Status = status.ToString()});
        }

        [HttpPost]
        public async Task<IActionResult> NewRequest(
            [BindRequired] [FromBody] UtoRequestMetadata metadata)
        {
            if (!_employees.EmployeeExists(metadata.EmployeeId))
            {
                return NotFound(metadata.EmployeeId);
            }
            if (metadata.Hours < 0)
            {
                return BadRequest();
            }

            var employee = await Task.Run(() => _employees.GetEmployee(metadata.EmployeeId));
            var request = await Task.Run(() => _writer.CreateRequest(employee, metadata.Day, metadata.Hours == 0 ? 8 : metadata.Hours));

            return CreatedAtAction(nameof(NewRequest), new { id = request.Id }, request);
        }

        [HttpPut("{requestId}/{approval}")]
        public async Task<IActionResult> UpdateApproval(
            [BindRequired] [FromRoute] int requestId,
            [BindRequired] [FromRoute] bool approval)
        {
            if (!_requests.RequestExists(requestId))
            {
                return NotFound(requestId);
            }

            if (approval)
            {
                await Task.Run(() => _writer.ApproveRequest(requestId));
            }
            else
            {
                await Task.Run(() => _writer.DenyRequest(requestId));
            }

            return Ok();
        }

        [HttpDelete("{requestId}")]
        public async Task<IActionResult> RemoveRequest(
            [FromRoute] int requestId)
        {
            if (!_requests.RequestExists(requestId))
            {
                return NotFound(requestId);
            }

            await Task.Run(() => _writer.DeleteRequest(requestId));
            return Ok();
        }
    }
}