using EmployeeCatalog.Shared.Data;
using EmployeeCatalog.Shared.Providers;
using EmployeeCatalog.Shared.Services;
using Microsoft.AspNetCore.Mvc;

namespace CRUDonDynmoDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ISQSProvider _sQSProvider;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeRepository employeeRepository, ISQSProvider sQSProvider)
        {
            _logger = logger;
            _employeeRepository = employeeRepository;
            _sQSProvider = sQSProvider;
        }
        [HttpGet("{employeeId:guid}")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByEmployeeId(Guid employeeId, CancellationToken cancellationToken = default)
        {
            try
            {
                var employee = await _employeeRepository.GetEmployeeById(employeeId, cancellationToken);
                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return BadRequest();
        }

        [HttpGet]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllEmployees(CancellationToken cancellationToken = default)
        {
            try
            {
                var employee = await _employeeRepository.GetAllEmployee(cancellationToken);
                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return BadRequest();
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddEmployee([FromBody] EmployeeRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var employee = await _employeeRepository.CreateEmployee(request, cancellationToken);
                await _sQSProvider.SendMessagesAsync(EmployeeSQSQueueTopic.EmpCreated, request, cancellationToken);
                return Created("", employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return BadRequest();
        }

        [HttpPut]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateEmployee([FromBody] Employee request, CancellationToken cancellationToken = default)
        {
            try
            {
                await _employeeRepository.UpdateEmployee(request, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return BadRequest();
        }

        [HttpDelete("{employeeId:guid}")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteEmployee(Guid employeeId, CancellationToken cancellationToken = default)
        {
            try
            {
                await _employeeRepository.DeleteEmployee(employeeId, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return BadRequest();
        }
    }
}
