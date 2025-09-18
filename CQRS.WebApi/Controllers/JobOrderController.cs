using CQRS.Application.Features.JobOrder.Commands.Create;
using CQRS.Application.Shared.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CQRS.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobOrderController(IMediator mediator, ILogger<JobOrderController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger _logger = logger;

        // GET: api/<JobOrderController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<JobOrderController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<JobOrderController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CreateJobOrderCommand createJobOrder)
        {
            if (createJobOrder == null)
            {
                _logger.LogWarning("Received null details for Job Order Creation.");
                // Log warning about null details for Job Order Creation
                return BadRequest("Request body cannot be null.");
            }
            try
            {
                // Send command to mediator to handle the command
                var response = await _mediator.Send(createJobOrder);

                // Ensure the response contains the ID of the newly created entity
                if (response == null || string.IsNullOrEmpty(response.Id))
                {
                    _logger.LogWarning("Failed to create Job Order. No valid data was returned.");
                    return BadRequest(response);
                }

                // Return 201 Created with the location of the created resource
                return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
            }
            catch (BadRequestException ex)
            {
                // Handle validation or bad request errors
                _logger.LogError(ex, "Validation or bad request error occurred while creating BatchSerial.");

                return BadRequest(new
                {
                    Message = "Controller Validation failed.",
                    ex.ValidationErrors // Return structured validation errors here
                });
            }
            catch (Exception ex)
            {
                // Log unexpected exceptions
                _logger.LogError(ex, "An unexpected error occurred while creating BatchSerial.");
                return StatusCode(500, new
                {
                    Message = "An internal server error occurred. Please try again later.",
                    Details = ex.Message // Optional: Include this for debugging purposes
                });
            }
        }

        // PUT api/<JobOrderController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<JobOrderController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
