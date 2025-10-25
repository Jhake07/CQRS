using CQRS.Application.DTO;
using CQRS.Application.Features.JobOrder.Commands.Create;
using CQRS.Application.Features.JobOrder.Commands.Delete;
using CQRS.Application.Features.JobOrder.Commands.Update;
using CQRS.Application.Features.JobOrder.Queries.GetAll;
using CQRS.Application.Features.JobOrder.Queries.GetById;
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
        public async Task<List<JobOrderDto>> Get()
        {
            var joborders = await _mediator.Send(new GetJobOrderQuery());
            return joborders;
        }

        // GET api/<JobOrderController>/5
        [HttpGet("{id}")]
        public async Task<JobOrderDto> Get(int id)
        {
            var joborder = await _mediator.Send(new GetJobOrderByIdQuery(id));

            return joborder;
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
        public async Task<ActionResult> Put(int id, [FromBody] UpdateJobOrderCommand updateJobOrder)
        {
            if (updateJobOrder == null || id != updateJobOrder.Id)
            {
                _logger.LogWarning("Received invalid details for Job Order Update.");
                return BadRequest("Invalid request data.");
            }
            try
            {
                if (updateJobOrder.Id != id)
                {
                    _logger.LogWarning("Mismatch between route ID ({Id}) and request ID ({RequestId}).", id, updateJobOrder.Id);
                    return BadRequest("Route ID and body ID must match.");
                }

                // Send the command to the mediator
                var response = await _mediator.Send(updateJobOrder);

                if (response == null || string.IsNullOrEmpty(response.Id))
                {
                    _logger.LogWarning("Failed to update Job Order details. No valid data was returned.");
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex, "Validation or bad request error occurred while updating Job Order.");
                return BadRequest(new
                {
                    Message = "Controller Validation failed.",
                    ex.ValidationErrors // Return structured validation errors here
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating Job Order.");
                return StatusCode(500, new
                {
                    Message = "An internal server error occurred. Please try again later.",
                    Details = ex.Message // Optional: Include this for debugging purposes
                });
            }
        }

        // DELETE api/<JobOrderController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Received invalid ID for Job Order Deletion: {Id}", id);
                return BadRequest("Invalid ID.");
            }

            try
            {
                var response = await _mediator.Send(new DeleteJobOrderCommand { Id = id });

                if (response == null || string.IsNullOrEmpty(response.Id))
                {
                    _logger.LogWarning("Failed to cancel Job Order. No valid data was returned for Id: {Id}", id);
                    return BadRequest(response);
                }

                _logger.LogInformation("Job Order with Id {Id} successfully cancelled.", id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during batch serial cancellation: {Id}", id);

                return StatusCode(500, new
                {
                    Message = "An internal server error occurred.",
                    Details = ex.Message
                });
            }
        }
    }
}
