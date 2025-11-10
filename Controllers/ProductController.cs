using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SweetfyAPI.DTOs.ProductDTO;
using SweetfyAPI.Services;

namespace SweetfyAPI.Controllers
{
    /// <summary>
    /// Manages final products for the logged-in user's bakery. 
    /// Products can be made of ingredients, services, and other recipes.
    /// </summary>
    [ApiController]
    [Route("api/products")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductController"/>.
        /// </summary>
        /// <param name="productService">The service responsible for product logic.</param>
        /// <param name="mapper">The AutoMapper instance for DTO conversion.</param>
        public ProductController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets a simplified list of all products for the user's bakery.
        /// </summary>
        /// <returns>A list of simple ProductDto objects.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetMyProducts()
        {
            var products = await _productService.GetProductsForUserAsync();
            return Ok(_mapper.Map<IEnumerable<ProductDto>>(products));
        }

        /// <summary>
        /// Gets the full details of a specific product, including its components (ingredients, recipes, services).
        /// </summary>
        /// <param name="id">The ID of the product to retrieve.</param>
        /// <returns>The complete ProductDetailsDto.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProductDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDetailsDto>> GetProductDetails(int id)
        {
            var product = await _productService.GetProductDetailsByIdForUserAsync(id);
            if (product == null)
                return NotFound(new { Message = "Product not found." });

            return Ok(_mapper.Map<ProductDetailsDto>(product));
        }

        /// <summary>
        /// Creates a new product. This operation is transactional and 
        /// creates all associated components (ingredients, recipes, services).
        /// </summary>
        /// <param name="dto">The composite DTO containing product data and its components.</param>
        /// <returns>The newly created product with all its details.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ProductDetailsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductDetailsDto>> CreateProduct([FromBody] CreateProductDto dto)
        {
            var newProduct = await _productService.CreateProductAsync(dto);
            if (newProduct == null)
                return BadRequest(new { Message = "Failed to create product." });

            var readDto = _mapper.Map<ProductDetailsDto>(newProduct);
            return CreatedAtAction(nameof(GetProductDetails), new { id = readDto.Id }, readDto);
        }

        /// <summary>
        /// Updates an existing product and its components list.
        /// The old component list will be replaced by the new one.
        /// </summary>
        /// <param name="id">The ID of the product to update.</param>
        /// <param name="dto">The updated composite DTO for the product.</param>
        /// <returns>No content (204) on success.</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto dto)
        {
            var updatedProduct = await _productService.UpdateProductAsync(id, dto);
            if (updatedProduct == null)
                return NotFound(new { Message = "Product not found." });

            return NoContent();
        }

        /// <summary>
        /// Deletes a product and all its associated components by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        /// <returns>No content (204) on success.</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result)
                return NotFound(new { Message = "Product not found." });

            return NoContent();
        }
    }
}