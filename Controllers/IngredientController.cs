using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SweetfyAPI.DTOs.IndredientDTO; 
using SweetfyAPI.Services;

namespace SweetfyAPI.Controllers
{
    /// <summary>
    /// Manages ingredients (raw materials) for the logged-in user's bakery.
    /// </summary>
    [ApiController]
    [Route("api/ingredients")]
    [Authorize] 
    public class IngredientController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="IngredientController"/>.
        /// </summary>
        /// <param name="ingredientService">The service responsible for ingredient logic.</param>
        /// <param name="mapper">The AutoMapper instance for DTO conversion.</param>
        public IngredientController(IIngredientService ingredientService, IMapper mapper)
        {
            _ingredientService = ingredientService;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all ingredients for the logged-in user's bakery.
        /// </summary>
        /// <returns>A list of ingredients.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<IngredientDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<IngredientDto>>> GetMyIngredients()
        {
            var ingredients = await _ingredientService.GetIngredientsForUserAsync();
            return Ok(_mapper.Map<IEnumerable<IngredientDto>>(ingredients));
        }


        /// <summary>
        /// Gets a specific ingredient by its unique ID.
        /// </summary>
        /// <param name="id">The ID of the ingredient to retrieve.</param>
        /// <returns>The requested ingredient.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(IngredientDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IngredientDto>> GetIngredient(int id)
        {
            var ingredient = await _ingredientService.GetIngredientByIdForUserAsync(id);
            if (ingredient == null)
                return NotFound(new { Message = "Ingredient not found." });

            return Ok(_mapper.Map<IngredientDto>(ingredient));
        }

        /// <summary>
        /// Creates a new ingredient for the user's bakery.
        /// </summary>
        /// <param name="dto">The data used to create the new ingredient.</param>
        /// <returns>The newly created ingredient.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(IngredientDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IngredientDto>> CreateIngredient([FromBody] CreateIngredientDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newIngredient = await _ingredientService.CreateIngredientAsync(dto);

            var readDto = _mapper.Map<IngredientDto>(newIngredient);
            return CreatedAtAction(nameof(GetIngredient), new { id = readDto.Id }, readDto);
        }

        /// <summary>
        /// Updates an existing ingredient.
        /// </summary>
        /// <param name="id">The ID of the ingredient to update.</param>
        /// <param name="dto">The updated data for the ingredient.</param>
        /// <returns>No content (204) if the update is successful.</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateIngredient(int id, [FromBody] UpdateIngredientDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedIngredient = await _ingredientService.UpdateIngredientAsync(id, dto);
            if (updatedIngredient == null)
                return NotFound(new { Message = "Ingredient not found." });

            return NoContent();
        }

        /// <summary>
        /// Deletes an ingredient by its unique ID.
        /// </summary>
        /// <param name="id">The ID of the ingredient to delete.</param>
        /// <returns>No content (204) if the deletion is successful.</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteIngredient(int id)
        {
            var result = await _ingredientService.DeleteIngredientAsync(id);
            if (!result)
                return NotFound(new { Message = "Ingredient not found." });

            return NoContent();
        }
    }
}