using CQRS.Application.DTO;
using CQRS.Application.Features.BatchSerial.Commands.CreateBatchSerial;
using CQRS.Application.Features.BatchSerial.Commands.DeleteBatchSerial;
using CQRS.Application.Features.BatchSerial.Commands.UpdateBatchSerial;
using CQRS.Application.Features.BatchSerial.Queries.GetAll;
using CQRS.Application.Features.BatchSerial.Queries.GetByContractNo;
using CQRS.Application.Features.BatchSerial.Queries.GetById;
using CQRS.Application.Shared.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CQRS.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatchSerialController(IMediator mediator, ILogger<BatchSerialController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger _logger = logger;

        // GET: api/<BatchSerialController>
        [HttpGet]
        public async Task<List<BatchSerialDto>> Get()
        {
            var batchserials = await _mediator.Send(new GetBatchSerialQuery());

            return batchserials;
        }

        // GET api/<BatchSerialController>/5
        [HttpGet("{id}")]
        public async Task<BatchSerialDto> Get(int id)
        {
            var batchserials = await _mediator.Send(new GetBatchSerialByIdQuery(id));

            return batchserials;
        }

        // GET api/<BatchSerialController>/5        
        [HttpGet("contract/{contractNo}")]
        public async Task<BatchSerialDto> GetByContractNo([FromRoute] string contractNo)
        {
            var batchserials = await _mediator.Send(new GetBatchSerialByContractNoQuery(contractNo));

            return batchserials;
        }

        // POST api/<BatchSerialController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CreateBatchSerialCommand createBatchSerial)
        {
            if (createBatchSerial == null)
            {
                _logger.LogWarning("Received null details for Batch Serial Creation.");
                return BadRequest("Request body cannot be null.");
            }

            try
            {
                // Send the command to the mediator
                var response = await _mediator.Send(createBatchSerial);

                // Ensure the response contains the ID of the newly created entity
                if (response == null || string.IsNullOrEmpty(response.Id))
                {
                    _logger.LogWarning("Failed to create Batch Contract and Mainserials. No valid data was returned.");
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

        /// Updates the BatchSerial with the specified ID.
        // PUT api/<BatchSerialController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] UpdateBatchSerialCommand updateBatchSerial)
        {
            if (updateBatchSerial == null)
            {
                _logger.LogWarning("Received null Update Batch Serial Command.");
                return BadRequest("Request body cannot be null.");
            }

            try
            {
                {
                    // Ensure the ID in the route matches the ID in the request
                    if (updateBatchSerial.Id != id)
                    {
                        _logger.LogWarning("Mismatch between route ID ({Id}) and request ID ({RequestId}).", id, updateBatchSerial.Id);
                        return BadRequest("Route ID and body ID must match.");
                    }

                    // Send the command to the mediator
                    var response = await _mediator.Send(updateBatchSerial);

                    if (response == null || string.IsNullOrEmpty(response.Id))
                    {
                        _logger.LogWarning("Failed to update Batch Contract details. No valid data was returned.");
                        return BadRequest(response);
                    }

                    return Ok(response);
                }
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

        // DELETE api/<BatchSerialController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid batch serial ID received: {Id}", id);
                return BadRequest("Invalid Batch Serial ID.");
            }

            try
            {
                var response = await _mediator.Send(new DeleteBatchSerialsCommand { Id = id });

                if (response == null || string.IsNullOrEmpty(response.Id))
                {
                    _logger.LogWarning("Failed to update Batch Contract details. No valid data was returned.");
                    return BadRequest(response);
                }

                _logger.LogInformation("Batch serial {Id} cancelled successfully.", id);
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
