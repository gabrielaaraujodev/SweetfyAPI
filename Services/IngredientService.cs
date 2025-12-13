using AutoMapper;
using SweetfyAPI.DTOs.IndredientDTO;
using SweetfyAPI.DTOs.ServiceDTO;
using SweetfyAPI.Repositories;

namespace SweetfyAPI.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly IIngredientRepository _ingredientRepo;
        private readonly ICostPropagationService _costPropagationService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public IngredientService(
            IIngredientRepository ingredientRepo,
            IUserService userService,
            IMapper mapper,
            ICostPropagationService costPropagationService)
        {
            _ingredientRepo = ingredientRepo;
            _userService = userService;
            _mapper = mapper;
            _costPropagationService = costPropagationService;
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

            await _ingredientRepo.UpdateAsync(existingIngredient);

            await _costPropagationService.PropagateIngredientChangesAsync(id, bakeryId);

            return existingIngredient;
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

        public async Task<(bool IsSuccess, string Message)> BulkDeleteIngredientsAsync(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
                return (false, "Nenhum ID fornecido para exclusão.");

            var userBakeryId = _userService.GetMyBakeryId();
            var ingredientsToDelete = await _ingredientRepo.GetByIdsAsync(ids);

            var authorizedToDelete = ingredientsToDelete
                .Where(i => i.BakeryId == userBakeryId)
                .ToList();

            if (!authorizedToDelete.Any())
                return (false, "Nenhum ingrediente válido encontrado...");

            var success = await _ingredientRepo.DeleteRangeAsync(authorizedToDelete);

            if (success)
                return (true, $"Sucesso! {authorizedToDelete.Count} ingredientes excluídos.");
            else
                return (false, "Erro ao salvar as alterações...");
        }

        public async Task<(bool IsSuccess, string Message)> BulkUpdatePricesAsync(List<BulkUpdateIngredientPriceDto> updates)
        {
            var bakeryId = _userService.GetMyBakeryId();
            int count = 0;

            foreach (var item in updates)
            {
                var ingredient = await _ingredientRepo.GetByIdAsync(item.Id);

                if (ingredient != null && ingredient.BakeryId == bakeryId)
                {
                    ingredient.UnitPrice = item.NewPrice;
                    ingredient.UpdatedAt = DateTime.UtcNow;

                    await _ingredientRepo.UpdateAsync(ingredient);

                    await _costPropagationService.PropagateIngredientChangesAsync(item.Id, bakeryId);

                    count++;
                }
            }

            return (true, $"{count} preços de ingredientes atualizados.");
        }
    }
}