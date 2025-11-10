using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SweetfyAPI.DTOs.RecipeDTO;
using SweetfyAPI.Services;

namespace SweetfyAPI.Controllers
{
    /// <summary>
    /// Manages recipes for the logged-in user's bakery. 
    /// Recipes are composite items made of ingredients and services.
    /// </summary>
    [ApiController]
    [Route("api/recipes")]
    [Authorize]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService _recipeService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeController"/>.
        /// </summary>
        /// <param name="recipeService">The service responsible for recipe logic.</param>
        /// <param name="mapper">The AutoMapper instance for DTO conversion.</param>
        public RecipeController(IRecipeService recipeService, IMapper mapper)
        {
            _recipeService = recipeService;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets a simplified list of all recipes for the user's bakery.
        /// </summary>
        /// <returns>A list of simple RecipeDto objects.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RecipeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RecipeDto>>> GetMyRecipes()
        {
            var recipes = await _recipeService.GetRecipesForUserAsync();
            return Ok(_mapper.Map<IEnumerable<RecipeDto>>(recipes));
        }

        /// <summary>
        /// Gets the full details of a specific recipe, including its ingredients and services.
        /// </summary>
        /// <param name="id">The ID of the recipe to retrieve.</param>
        /// <returns>The complete RecipeDetailsDto.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(RecipeDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RecipeDetailsDto>> GetRecipeDetails(int id)
        {
            var recipe = await _recipeService.GetRecipeDetailsByIdForUserAsync(id);
            if (recipe == null)
                return NotFound(new { Message = "Recipe not found." });

            return Ok(_mapper.Map<RecipeDetailsDto>(recipe));
        }

        /// <summary>
        /// Creates a new recipe. This operation is transactional and will also 
        /// create the associated recipe ingredients and services.
        /// </summary>
        /// <param name="dto">The composite DTO containing recipe data and its components.</param>
        /// <returns>The newly created recipe with all its details.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(RecipeDetailsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RecipeDetailsDto>> CreateRecipe([FromBody] CreateRecipeDto dto)
        {
            var newRecipe = await _recipeService.CreateRecipeAsync(dto);
            if (newRecipe == null)
                return BadRequest(new { Message = "Failed to create recipe. Check if component IDs are valid." });

            var readDto = _mapper.Map<RecipeDetailsDto>(newRecipe);
            return CreatedAtAction(nameof(GetRecipeDetails), new { id = readDto.Id }, readDto);
        }

        /// <summary>
        /// Updates an existing recipe, including its list of ingredients and services. 
        /// The old component list will be replaced by the new one.
        /// </summary>
        /// <param name="id">The ID of the recipe to update.</param>
        /// <param name="dto">The updated composite DTO for the recipe.</param>
        /// <returns>No content (204) on success.</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRecipe(int id, [FromBody] UpdateRecipeDto dto)
        {
            var updatedRecipe = await _recipeService.UpdateRecipeAsync(id, dto);
            if (updatedRecipe == null)
                return NotFound(new { Message = "Recipe not found." });

            return NoContent();
        }

        /// <summary>
        /// Deletes a recipe and all its associated components (ingredients/services) by its ID.
        /// </summary>
        /// <param name="id">The ID of the recipe to delete.</param>
        /// <returns>No content (204) on success.</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var result = await _recipeService.DeleteRecipeAsync(id);
            if (!result)
                return NotFound(new { Message = "Recipe not found." });

            return NoContent();
        }
    }
}