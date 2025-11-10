using AutoMapper;
using SweetfyAPI.DTOs.ProductDTO;
using SweetfyAPI.Repositories;

namespace SweetfyAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly IIngredientRepository _ingredientRepo;
        private readonly IServiceRepository _serviceRepo;
        private readonly IRecipeRepository _recipeRepo;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public ProductService(
            IProductRepository productRepo,
            IIngredientRepository ingredientRepo,
            IServiceRepository serviceRepo,
            IRecipeRepository recipeRepo,
            IUserService userService,
            IMapper mapper)
        {
            _productRepo = productRepo;
            _ingredientRepo = ingredientRepo;
            _serviceRepo = serviceRepo;
            _recipeRepo = recipeRepo;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<global::Product>> GetProductsForUserAsync()
        {
            var bakeryId = _userService.GetMyBakeryId();
            return await _productRepo.GetByBakeryIdAsync(bakeryId);
        }

        public async Task<global::Product?> GetProductDetailsByIdForUserAsync(int id)
        {
            var bakeryId = _userService.GetMyBakeryId();
            var product = await _productRepo.GetByIdWithComponentsAsync(id);

            if (product == null || product.BakeryId != bakeryId)
            {
                return null;
            }
            return product;
        }

        public async Task<global::Product?> CreateProductAsync(CreateProductDto dto)
        {
            var bakeryId = _userService.GetMyBakeryId();
            var product = _mapper.Map<global::Product>(dto);
            product.BakeryId = bakeryId;
            product.CreatedAt = DateTime.UtcNow;

            product.BaseCost = await CalculateBaseCostAsync(bakeryId, product.ProductIngredients, product.ProductRecipes, product.ProductServices);

            CalculateProfit(product);

            var newProduct = await _productRepo.AddAsync(product);


            return await _productRepo.GetByIdWithComponentsAsync(newProduct.Id);
        }

        public async Task<global::Product?> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            var bakeryId = _userService.GetMyBakeryId();
            var existingProduct = await _productRepo.GetByIdWithComponentsAsync(id);

            if (existingProduct == null || existingProduct.BakeryId != bakeryId)
            {
                return null;
            }

            _mapper.Map(dto, existingProduct);

            existingProduct.BaseCost = await CalculateBaseCostAsync(bakeryId, existingProduct.ProductIngredients, existingProduct.ProductRecipes, existingProduct.ProductServices);

            CalculateProfit(existingProduct);

            existingProduct.UpdatedAt = DateTime.UtcNow;

            await _productRepo.UpdateAsync(existingProduct);

            return await _productRepo.GetByIdWithComponentsAsync(id);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var bakeryId = _userService.GetMyBakeryId();
            var existingProduct = await _productRepo.GetByIdAsync(id);

            if (existingProduct == null || existingProduct.BakeryId != bakeryId)
            {
                return false;
            }

            var result = await _productRepo.DeleteAsync(id);
            return result != null;
        }


        private async Task<decimal> CalculateBaseCostAsync(
            int bakeryId,
            ICollection<global::ProductIngredient> pIngredients,
            ICollection<global::ProductRecipe> pRecipes,
            ICollection<global::ProductService> pServices)
        {
            decimal baseCost = 0;

            foreach (var pi in pIngredients)
            {
                var ingredient = await _ingredientRepo.GetByIdAsync(pi.IngredientId);
                if (ingredient == null || ingredient.BakeryId != bakeryId) throw new Exception("Invalid Ingredient.");


                pi.UnitPriceSnapshot = ingredient.UnitPrice; 
                baseCost += (pi.UnitPriceSnapshot.Value * pi.Quantity);
            }


            foreach (var ps in pServices)
            {
                var service = await _serviceRepo.GetByIdAsync(ps.ServiceId); 
                if (service == null || service.BakeryId != bakeryId) throw new Exception("Invalide Service.");

                ps.UnitPriceSnapshot = service.UnitPrice; 
                baseCost += (ps.UnitPriceSnapshot.Value * ps.Quantity); 
            }

            foreach (var pr in pRecipes)
            {
                var recipe = await _recipeRepo.GetByIdWithComponentsAsync(pr.RecipeId);
                if (recipe == null || recipe.BakeryId != bakeryId) throw new Exception("Invalid Recipe.");

                decimal recipeCost = 0;
                recipeCost += recipe.RecipeIngredients.Sum(ri => (ri.UnitPriceSnapshot ?? 0) * ri.Quantity);
                recipeCost += recipe.RecipeServices.Sum(rs => (rs.UnitPriceSnapshot ?? 0) * rs.Quantity);

                decimal recipeUnitCost = (recipe.YieldQuantity > 0) ? (recipeCost / recipe.YieldQuantity) : 0;

                recipeUnitCost *= (1 + recipe.AdditionalCostPercent / 100);

                pr.UnitPriceSnapshot = recipeUnitCost; 
                baseCost += (pr.UnitPriceSnapshot.Value * pr.Quantity);
            }

            return baseCost;
        }

        private void CalculateProfit(global::Product product)
        {
            if (product.BaseCost == null) return;

            var baseCost = product.BaseCost.Value;

            if (product.ProfitPercent != null && product.ProfitPercent > 0)
            {
                product.ProfitAmount = baseCost * (product.ProfitPercent.Value / 100);
                product.SalePrice = baseCost + product.ProfitAmount;
            }
            else if (product.SalePrice != null && product.SalePrice > 0)
            {
                product.ProfitAmount = product.SalePrice - baseCost;
                if (baseCost > 0)
                {
                    product.ProfitPercent = (product.ProfitAmount / baseCost) * 100;
                }
                else
                {
                    product.ProfitPercent = 0;
                }
            }
        }
    }
}