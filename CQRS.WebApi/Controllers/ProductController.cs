using CQRS.Application.DTO;
using CQRS.Application.Features.Product.Commands.CreateProduct;
using CQRS.Application.Features.Product.Queries.GetAll;
using CQRS.Application.Features.Product.Queries.GetByCode;
using CQRS.Application.Features.Product.Queries.GetById;
using CQRS.Application.Shared.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CQRS.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IMediator mediator, ILogger<ProductController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger _logger = logger;

        // GET: api/<ProductController>
        [HttpGet]
        public async Task<List<ProductDto>> Get()
        {
            var products = await _mediator.Send(new GetProductQuery());

            return products;
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public async Task<ProductDto> Get(int id)
        {
            var product = await _mediator.Send(new GetProductByIdQuery(id));

            return product;
        }

        [HttpGet("code/{modelcode}")]
        public async Task<ProductDto> GetByCode([FromRoute] string modelcode)
        {
            var product = await _mediator.Send(new GetProductByCodeQuery(modelcode));

            return product;
        }

        // POST api/<ProductController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CreateProductCommand createProductCommand)
        {
            if (createProductCommand == null)
            {
                _logger.LogWarning("Received null details for Product Creation.");
                return BadRequest("Request body cannot be null.");
            }
            try
            {
                // Send command to mediator to handle the command
                var result = await _mediator.Send(createProductCommand);

                //Ensure the result is not null
                if (result == null)
                {
                    _logger.LogError("Product creation failed, result is null.");
                    return BadRequest(result);
                }

                // Return 201 Created with the location of the created resource
                return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
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

        // PUT api/<ProductController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
