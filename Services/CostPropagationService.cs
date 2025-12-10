using Microsoft.EntityFrameworkCore;
using SweetfyAPI.Entities;

namespace SweetfyAPI.Services
{
    public class CostPropagationService : ICostPropagationService
    {
        private readonly AppDbContext _context;

        public CostPropagationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task PropagateIngredientChangesAsync(int ingredientId, int bakeryId)
        {
            var recipesToUpdate = await _context.RecipeIngredients
                .Where(ri => ri.IngredientId == ingredientId && ri.Recipe.BakeryId == bakeryId)
                .Select(ri => ri.RecipeId)
                .Distinct()
                .ToListAsync();

            foreach (var recipeId in recipesToUpdate)
            {
                await RecalculateAndSaveRecipeAsync(recipeId);
                await PropagateRecipeChangesAsync(recipeId, bakeryId);
            }

            var productsToUpdate = await _context.ProductIngredients
                .Where(pi => pi.IngredientId == ingredientId && pi.Product.BakeryId == bakeryId)
                .Select(pi => pi.ProductId)
                .Distinct()
                .ToListAsync();

            foreach (var productId in productsToUpdate)
            {
                await RecalculateAndSaveProductAsync(productId);
                await PropagateProductChangesAsync(productId, bakeryId);
            }
        }

        public async Task PropagateServiceChangesAsync(int serviceId, int bakeryId)
        {
            var recipesToUpdate = await _context.RecipeServices
                .Where(rs => rs.ServiceId == serviceId && rs.Recipe.BakeryId == bakeryId)
                .Select(rs => rs.RecipeId)
                .Distinct()
                .ToListAsync();

            foreach (var rId in recipesToUpdate)
            {
                await RecalculateAndSaveRecipeAsync(rId);
                await PropagateRecipeChangesAsync(rId, bakeryId);
            }

            var productsToUpdate = await _context.ProductServices
                .Where(ps => ps.ServiceId == serviceId && ps.Product.BakeryId == bakeryId)
                .Select(ps => ps.ProductId)
                .Distinct()
                .ToListAsync();

            foreach (var pId in productsToUpdate)
            {
                await RecalculateAndSaveProductAsync(pId);
                await PropagateProductChangesAsync(pId, bakeryId);
            }
        }

        public async Task PropagateRecipeChangesAsync(int recipeId, int bakeryId)
        {
            var productsToUpdate = await _context.ProductRecipes
                .Where(pr => pr.RecipeId == recipeId && pr.Product.BakeryId == bakeryId)
                .Select(pr => pr.ProductId)
                .Distinct()
                .ToListAsync();

            foreach (var productId in productsToUpdate)
            {
                await RecalculateAndSaveProductAsync(productId);
                await PropagateProductChangesAsync(productId, bakeryId);
            }

            var ordersToUpdate = await _context.OrderRecipes
                .Where(or => or.RecipeId == recipeId && or.Order.BakeryId == bakeryId)
                .Select(or => or.OrderId)
                .Distinct()
                .ToListAsync();

            foreach (var orderId in ordersToUpdate)
            {
                await RecalculateAndSaveOrderAsync(orderId);
            }
        }

        public async Task PropagateProductChangesAsync(int productId, int bakeryId)
        {
            var ordersToUpdate = await _context.OrderProducts
                .Where(op => op.ProductId == productId && op.Order.BakeryId == bakeryId)
                .Select(op => op.OrderId)
                .Distinct()
                .ToListAsync();

            foreach (var orderId in ordersToUpdate)
            {
                await RecalculateAndSaveOrderAsync(orderId);
            }
        }

        private async Task RecalculateAndSaveRecipeAsync(int recipeId)
        {
            var recipe = await _context.Recipes
                .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
                .Include(r => r.RecipeServices).ThenInclude(rs => rs.Service)
                .FirstOrDefaultAsync(r => r.Id == recipeId);

            if (recipe == null) return;

            decimal totalCost = 0;
            foreach (var ri in recipe.RecipeIngredients)
            {
                ri.UnitPriceSnapshot = ri.Ingredient.UnitPrice;

                decimal costPerUnit = (ri.Ingredient.Quantity > 0)
                    ? (ri.Ingredient.UnitPrice / ri.Ingredient.Quantity)
                    : 0;

                totalCost += (costPerUnit * ri.Quantity);
            }
            foreach (var rs in recipe.RecipeServices)
            {
                rs.UnitPriceSnapshot = rs.Service.UnitPrice;
                totalCost += (rs.Service.UnitPrice * rs.Quantity);
            }
            decimal unitCost = (recipe.YieldQuantity > 0) ? (totalCost / recipe.YieldQuantity) : 0;
            unitCost *= (1 + recipe.AdditionalCostPercent / 100);

            recipe.BaseCost = unitCost;
            recipe.UpdatedAt = DateTime.UtcNow;

            foreach (var ri in recipe.RecipeIngredients) ri.UnitPriceSnapshot = ri.Ingredient.UnitPrice;
            foreach (var rs in recipe.RecipeServices) rs.UnitPriceSnapshot = rs.Service.UnitPrice;

            await _context.SaveChangesAsync();
        }

        private async Task RecalculateAndSaveProductAsync(int productId)
        {
            var product = await _context.Products
                .Include(p => p.ProductIngredients).ThenInclude(pi => pi.Ingredient)
                .Include(p => p.ProductServices).ThenInclude(ps => ps.Service)
                .Include(p => p.ProductRecipes).ThenInclude(pr => pr.Recipe)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null) return;

            decimal baseCost = 0;

            baseCost += product.ProductIngredients.Sum(pi => pi.Ingredient.UnitPrice * pi.Quantity);
            baseCost += product.ProductServices.Sum(ps => ps.Service.UnitPrice * ps.Quantity);
            baseCost += product.ProductRecipes.Sum(pr => pr.Recipe.BaseCost * pr.Quantity);

            product.BaseCost = baseCost;
            product.UpdatedAt = DateTime.UtcNow;

            if (product.ProfitPercent.HasValue && product.ProfitPercent > 0)
            {
                product.ProfitAmount = baseCost * (product.ProfitPercent.Value / 100);
                product.SalePrice = baseCost + product.ProfitAmount;
            }

            foreach (var pi in product.ProductIngredients) pi.UnitPriceSnapshot = pi.Ingredient.UnitPrice;
            foreach (var ps in product.ProductServices) ps.UnitPriceSnapshot = ps.Service.UnitPrice;
            foreach (var pr in product.ProductRecipes) pr.UnitPriceSnapshot = pr.Recipe.BaseCost;

            await _context.SaveChangesAsync();
        }

        private async Task RecalculateAndSaveOrderAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderProducts).ThenInclude(op => op.Product)
                .Include(o => o.OrderRecipes).ThenInclude(or => or.Recipe)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return;

            decimal totalCost = 0;
            decimal totalSale = 0;
            decimal totalYield = 0;

            foreach (var op in order.OrderProducts)
            {
                op.CostSnapshot = op.Product.BaseCost ?? 0;
                op.UnitPriceSnapshot = op.Product.SalePrice ?? 0;

                totalCost += (op.CostSnapshot.Value * op.Quantity);
                totalSale += (op.UnitPriceSnapshot.Value * op.Quantity);

                totalYield += op.Quantity;
            }

            foreach (var or in order.OrderRecipes)
            {
                or.CostSnapshot = or.Recipe.BaseCost;
                totalCost += (or.CostSnapshot.Value * or.Quantity);

                totalYield += or.Quantity;
            }

            order.TotalCost = totalCost;

            order.SalePrice = totalSale;

            order.Profit = (order.SalePrice ?? 0) - (order.TotalCost ?? 0);

            order.TotalYield = totalYield;

            await _context.SaveChangesAsync();
        }
    }
}