using CQRS.Application.DTO;
using CQRS.Application.Features.Product.Commands.CreateProduct;
using CQRS.Application.Features.Product.Commands.DeleteProduct;
using CQRS.Application.Features.Product.Commands.UpdateProduct;
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
        public async Task<ActionResult> Put(int id, [FromBody] UpdateProductCommand updateProduct)
        {
            if (updateProduct == null)
            {
                _logger.LogWarning("Received null Update Batch Serial Command.");
                return BadRequest("Request body cannot be null.");
            }

            try
            {
                // Ensure the ID in the route matches the ID in the request
                if (updateProduct.Id != id)
                {
                    _logger.LogWarning("Mismatch between route ID ({Id}) and request ID ({RequestId}).", id, updateProduct.Id);
                    return BadRequest("Route ID and body ID must match.");
                }

                // Send the command to the mediator
                var response = await _mediator.Send(updateProduct);

                if (response == null || string.IsNullOrEmpty(response.Id))
                {
                    _logger.LogWarning("Failed to update Product details. No valid data was returned.");
                    return BadRequest(response);
                }

                return Ok(response);

            }
            catch (BadRequestException ex)
            {
                // Handle validation or bad request errors
                _logger.LogError(ex, "Validation or bad request error occurred while creating new Product.");

                return BadRequest(new
                {
                    Message = "Controller Validation failed.",
                    ex.ValidationErrors // Return structured validation errors here
                });
            }
            catch (Exception ex)
            {
                // Log unexpected exceptions
                _logger.LogError(ex, "An unexpected error occurred while creating Product.");
                return StatusCode(500, new
                {
                    Message = "An internal server error occurred. Please try again later.",
                    Details = ex.Message // Optional: Include this for debugging purposes
                });
            }
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid ID provided for deletion: {Id}", id);
                return BadRequest("Invalid product provided.");
            }
            try
            {
                var response = await _mediator.Send(new DeleteProductCommand { Id = id });

                if (response == null || string.IsNullOrEmpty(response.Id))
                {
                    _logger.LogWarning("Failed to delete Product with ID {Id}. No valid data was returned.", id);
                    return BadRequest(response);
                }

                _logger.LogInformation("Product with ID {Id} successfully deleted.", id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error");

                return StatusCode(500, new
                {
                    Message = "An internal server error occurred. Please try again later.",
                    Details = ex.Message // Optional: Include this for debugging purposes
                });
            }
        }
    }
}
