using CQRS.Application.Features.ScannedUnit.Commands.UpdateScannedUnit.UpdateAccessories;
using CQRS.Application.Features.ScannedUnit.Commands.UpdateScannedUnit.UpdateComponent;
using CQRS.Application.Features.ScannedUnit.Commands.UpdateScannedUnit.UpdateTag;
using CQRS.Application.Shared.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CQRS.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScannedUnitsController(IMediator mediator, ILogger<ProductController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger _logger = logger;

        // GET: api/<ScannedUnitsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ScannedUnitsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ScannedUnitsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ScannedUnitsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ScannedUnitsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpPatch("{mainserial}/components")]
        // 1. Change the method signature to use the CustomResultResponse in the success case
        public async Task<IActionResult> UpdateComponents([FromRoute] string mainserial, [FromBody] UpdateComponentsCommand request)
        {
            _logger.LogInformation("Station 3: Received command to update components for serial {MainSerial}", mainserial);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validation failed for component update on {MainSerial}", mainserial);
                return BadRequest(ModelState);
            }

            try
            {
                var command = new UpdateComponentsCommand(
                    MainSerial: mainserial,
                    MotherboardSerial: request.MotherboardSerial,
                    PcbiSerial: request.PcbiSerial,
                    PowerSupplySerial: request.PowerSupplySerial);

                // 2. CAPTURE the result from the mediator!
                var result = await _mediator.Send(command);

                _logger.LogInformation("Successfully executed component update for serial {MainSerial}", mainserial);

                // 3. RETURN the result object with a 200 OK status.
                return Ok(result);

                // Alternatively, if the update takes time, you could return Accepted(result); (202)
            }
            catch (BadRequestException ex)
            {
                // Handle validation or bad request errors (HTTP 400)
                _logger.LogError(ex, "Validation or bad request error occurred while updating the components.");

                // If you are using an IResult convention (recommended), you can use result.ValidationErrors here
                return BadRequest(new
                {
                    Message = "Validation or business rule failed.",
                    ex.ValidationErrors
                });
            }
            catch (Exception ex)
            {
                // Log unexpected exceptions (HTTP 500)
                _logger.LogError(ex, "An unexpected error occurred while updating components for {MainSerial}", mainserial);
                return StatusCode(500, new
                {
                    Message = "An internal server error occurred. Please try again later.",
                    Details = ex.Message
                });
            }
        }

        [HttpPatch("{mainserial}/tag")]
        // 1. Change the method signature to use the CustomResultResponse in the success case
        public async Task<IActionResult> UpdateTag([FromRoute] string mainserial, [FromBody] UpdateTagCommand request)
        {
            _logger.LogInformation("Station 3: Received command to update tag for serial {MainSerial}", mainserial);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validation failed for tagging update on {MainSerial}", mainserial);
                return BadRequest(ModelState);
            }

            try
            {
                var command = new UpdateTagCommand(
                    MainSerial: mainserial,
                    NewTagNo: request.NewTagNo);

                // 2. CAPTURE the result from the mediator!
                var result = await _mediator.Send(command);

                _logger.LogInformation("Successfully executed tag update for serial {MainSerial}", mainserial);

                // 3. RETURN the result object with a 200 OK status.
                return Ok(result);

                // Alternatively, if the update takes time, you could return Accepted(result); (202)
            }
            catch (BadRequestException ex)
            {
                // Handle validation or bad request errors (HTTP 400)
                _logger.LogError(ex, "Validation or bad request error occurred while updating the unit tag.");

                // If you are using an IResult convention (recommended), you can use result.ValidationErrors here
                return BadRequest(new
                {
                    Message = "Validation or business rule failed.",
                    ex.ValidationErrors
                });
            }
            catch (Exception ex)
            {
                // Log unexpected exceptions (HTTP 500)
                _logger.LogError(ex, "An unexpected error occurred while updating the tag for {MainSerial}", mainserial);
                return StatusCode(500, new
                {
                    Message = "An internal server error occurred. Please try again later.",
                    Details = ex.Message
                });
            }
        }

        [HttpPatch("{mainserial}/accessories")]
        // 1. Change the method signature to use the CustomResultResponse in the success case
        public async Task<IActionResult> UpdateAccessories([FromRoute] string mainserial, [FromBody] UpdateAccessoriesCommand request)
        {
            _logger.LogInformation("Station 3: Received command to update accessories for serial {MainSerial}", mainserial);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validation failed for accessories update on {MainSerial}", mainserial);
                return BadRequest(ModelState);
            }

            try
            {
                var command = new UpdateAccessoriesCommand(
                    mainSerial: mainserial,
                    accessoriesSerial: request.accessoriesSerial);

                // 2. CAPTURE the result from the mediator!
                var result = await _mediator.Send(command);

                _logger.LogInformation("Successfully executed accessories update for serial {MainSerial}", mainserial);

                // 3. RETURN the result object with a 200 OK status.
                return Ok(result);

                // Alternatively, if the update takes time, you could return Accepted(result); (202)
            }
            catch (BadRequestException ex)
            {
                // Handle validation or bad request errors (HTTP 400)
                _logger.LogError(ex, "Validation or bad request error occurred while updating the unit accessories.");

                // If you are using an IResult convention (recommended), you can use result.ValidationErrors here
                return BadRequest(new
                {
                    Message = "Validation or business rule failed.",
                    ex.ValidationErrors
                });
            }
            catch (Exception ex)
            {
                // Log unexpected exceptions (HTTP 500)
                _logger.LogError(ex, "An unexpected error occurred while updating the tag for {MainSerial}", mainserial);
                return StatusCode(500, new
                {
                    Message = "An internal server error occurred. Please try again later.",
                    Details = ex.Message
                });
            }
        }

    }
}
