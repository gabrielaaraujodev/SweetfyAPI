using AutoMapper;
using SweetfyAPI.DTOs.RecipeDTO;
using SweetfyAPI.Repositories;

namespace SweetfyAPI.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepo;
        private readonly IIngredientRepository _ingredientRepo; 
        private readonly IServiceRepository _serviceRepo;    
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public RecipeService(
            IRecipeRepository recipeRepo,
            IIngredientRepository ingredientRepo,
            IServiceRepository serviceRepo,
            IUserService userService,
            IMapper mapper)
        {
            _recipeRepo = recipeRepo;
            _ingredientRepo = ingredientRepo;
            _serviceRepo = serviceRepo;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Recipe>> GetRecipesForUserAsync()
        {
            var bakeryId = _userService.GetMyBakeryId();
            return await _recipeRepo.GetByBakeryIdAsync(bakeryId);
        }

        public async Task<Recipe?> GetRecipeDetailsByIdForUserAsync(int id)
        {
            var bakeryId = _userService.GetMyBakeryId();
            var recipe = await _recipeRepo.GetByIdWithComponentsAsync(id);

            if (recipe == null || recipe.BakeryId != bakeryId)
            {
                return null;
            }

            return recipe;
        }

        public async Task<Recipe?> CreateRecipeAsync(CreateRecipeDto dto)
        {
            var bakeryId = _userService.GetMyBakeryId();
            var recipe = _mapper.Map<Recipe>(dto);
            recipe.BakeryId = bakeryId;
            recipe.CreatedAt = DateTime.UtcNow;

            foreach (var ri in recipe.RecipeIngredients)
            {
                var ingredient = await _ingredientRepo.GetByIdAsync(ri.IngredientId);
                if (ingredient == null || ingredient.BakeryId != bakeryId)
                {
                    return null; 
                }
                ri.UnitPriceSnapshot = ingredient.UnitPrice;
            }

            foreach (var rs in recipe.RecipeServices)
            {
                var service = await _serviceRepo.GetByIdAsync(rs.ServiceId);
                if (service == null || service.BakeryId != bakeryId)
                {
                    return null; 
                }
                rs.UnitPriceSnapshot = service.UnitPrice;
            }

            return await _recipeRepo.AddAsync(recipe);
        }

        public async Task<Recipe?> UpdateRecipeAsync(int id, UpdateRecipeDto dto)
        {
            var bakeryId = _userService.GetMyBakeryId();
            var existingRecipe = await _recipeRepo.GetByIdWithComponentsAsync(id);

            if (existingRecipe == null || existingRecipe.BakeryId != bakeryId)
            {
                return null;
            }

            _mapper.Map(dto, existingRecipe);

            foreach (var ri in existingRecipe.RecipeIngredients)
            {
                ri.RecipeId = existingRecipe.Id; 
                var ingredient = await _ingredientRepo.GetByIdAsync(ri.IngredientId);
                if (ingredient == null || ingredient.BakeryId != bakeryId) return null;
                ri.UnitPriceSnapshot = ingredient.UnitPrice;
            }
            foreach (var rs in existingRecipe.RecipeServices)
            {
                rs.RecipeId = existingRecipe.Id; 
                var service = await _serviceRepo.GetByIdAsync(rs.ServiceId);
                if (service == null || service.BakeryId != bakeryId) return null;
                rs.UnitPriceSnapshot = service.UnitPrice;
            }

            existingRecipe.UpdatedAt = DateTime.UtcNow;

            return await _recipeRepo.UpdateAsync(existingRecipe);
        }

        public async Task<bool> DeleteRecipeAsync(int id)
        {
            var bakeryId = _userService.GetMyBakeryId();
            var existingRecipe = await _recipeRepo.GetByIdAsync(id);

            if (existingRecipe == null || existingRecipe.BakeryId != bakeryId)
            {
                return false;
            }

            var result = await _recipeRepo.DeleteAsync(id);
            return result != null;
        }
    }
}