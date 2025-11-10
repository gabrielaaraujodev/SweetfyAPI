using AutoMapper;
using SweetfyAPI.DTOs.IndredientDTO;
using SweetfyAPI.Repositories;

namespace SweetfyAPI.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly IIngredientRepository _ingredientRepo;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public IngredientService(
            IIngredientRepository ingredientRepo,
            IUserService userService,
            IMapper mapper)
        {
            _ingredientRepo = ingredientRepo;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Ingredient>> GetIngredientsForUserAsync()
        {
            var bakeryId = _userService.GetMyBakeryId();
            return await _ingredientRepo.GetByBakeryIdAsync(bakeryId);
        }

        public async Task<Ingredient?> GetIngredientByIdForUserAsync(int id)
        {
            var bakeryId = _userService.GetMyBakeryId();
            var ingredient = await _ingredientRepo.GetByIdAsync(id);

            if (ingredient == null || ingredient.BakeryId != bakeryId)
            {
                return null;
            }

            return ingredient;
        }

        public async Task<Ingredient> CreateIngredientAsync(CreateIngredientDto dto)
        {
            var bakeryId = _userService.GetMyBakeryId();

            var ingredient = _mapper.Map<Ingredient>(dto);

            ingredient.BakeryId = bakeryId;
            ingredient.CreatedAt = DateTime.UtcNow;

            return await _ingredientRepo.AddAsync(ingredient);
        }

        public async Task<Ingredient?> UpdateIngredientAsync(int id, UpdateIngredientDto dto)
        {
            var bakeryId = _userService.GetMyBakeryId();
            var existingIngredient = await _ingredientRepo.GetByIdAsync(id);

            if (existingIngredient == null || existingIngredient.BakeryId != bakeryId)
            {
                return null;
            }

            _mapper.Map(dto, existingIngredient);
            existingIngredient.UpdatedAt = DateTime.UtcNow;

            return await _ingredientRepo.UpdateAsync(existingIngredient);
        }

        public async Task<bool> DeleteIngredientAsync(int id)
        {
            var bakeryId = _userService.GetMyBakeryId();
            var existingIngredient = await _ingredientRepo.GetByIdAsync(id);

            if (existingIngredient == null || existingIngredient.BakeryId != bakeryId)
            {
                return false; 
            }

            var result = await _ingredientRepo.DeleteAsync(id);
            return result != null; 
        }
    }
}