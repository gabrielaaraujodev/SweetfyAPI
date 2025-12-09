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
        private readonly ICostPropagationService _costPropagationService;

        public RecipeService(
            IRecipeRepository recipeRepo,
            IIngredientRepository ingredientRepo,
            IServiceRepository serviceRepo,
            IUserService userService,
            IMapper mapper,
            ICostPropagationService costPropagationService)
        {
            _recipeRepo = recipeRepo;
            _ingredientRepo = ingredientRepo;
            _serviceRepo = serviceRepo;
            _userService = userService;
            _mapper = mapper;
            _costPropagationService = costPropagationService;
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
                ri.Ingredient = ingredient;
                ri.UnitPriceSnapshot = ingredient.UnitPrice;
            }

            foreach (var rs in recipe.RecipeServices)
            {
                var service = await _serviceRepo.GetByIdAsync(rs.ServiceId);
                if (service == null || service.BakeryId != bakeryId)
                {
                    return null; 
                }
                rs.Service = service;
                rs.UnitPriceSnapshot = service.UnitPrice;
            }
            recipe.BaseCost = CalculateRecipeCost(recipe);

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
            await _recipeRepo.UpdateAsync(existingRecipe);

            await _costPropagationService.PropagateRecipeChangesAsync(id, bakeryId);

            existingRecipe.UpdatedAt = DateTime.UtcNow;

            return existingRecipe;
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

        public decimal CalculateRecipeCost(Recipe recipe)
        {
            decimal totalCost = 0;

            if (recipe.RecipeIngredients != null)
            {
                totalCost += recipe.RecipeIngredients.Sum(ri => (ri.UnitPriceSnapshot ?? 0) * ri.Quantity);
            }

            if (recipe.RecipeServices != null)
            {
                totalCost += recipe.RecipeServices.Sum(rs => (rs.UnitPriceSnapshot ?? 0) * rs.Quantity);
            }

            decimal unitCost = (recipe.YieldQuantity > 0) ? (totalCost / recipe.YieldQuantity) : 0;

            unitCost *= (1 + recipe.AdditionalCostPercent / 100);

            return unitCost;
        }

        public async Task<(bool IsSuccess, string Message)> BulkDeleteRecipesAsync(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
                return (false, "Nenhum ID fornecido para exclusão.");

            var userBakeryId = _userService.GetMyBakeryId();

            var recipesToDelete = await _recipeRepo.GetByIdsAsync(ids);

            var authorizedToDelete = recipesToDelete
                .Where(r => r.BakeryId == userBakeryId)
                .ToList();

            if (!authorizedToDelete.Any())
            {
                return (true, "Nenhuma receita válida para exclusão foi encontrada ou elas não pertencem à sua padaria.");
            }

            var success = await _recipeRepo.DeleteRangeAsync(authorizedToDelete);

            if (success)
            {
                return (true, $"Sucesso! {authorizedToDelete.Count} receitas excluídas.");
            }
            else
            {
                return (false, "Erro ao salvar as alterações no banco de dados durante a exclusão.");
            }
        }
    }
}