using AutoMapper;
using SweetfyAPI.DTOs.OrderDTO;
using SweetfyAPI.Repositories;

namespace SweetfyAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IProductRepository _productRepo;
        private readonly IRecipeRepository _recipeRepo;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepo,
            IProductRepository productRepo,
            IRecipeRepository recipeRepo,
            IUserService userService,
            IMapper mapper)
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _recipeRepo = recipeRepo;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Order>> GetOrdersForUserAsync()
        {
            var bakeryId = _userService.GetMyBakeryId();
            return await _orderRepo.GetByBakeryIdAsync(bakeryId);
        }

        public async Task<Order?> GetOrderDetailsByIdForUserAsync(int id)
        {
            var bakeryId = _userService.GetMyBakeryId();
            var order = await _orderRepo.GetByIdWithItemsAsync(id);

            if (order == null || order.BakeryId != bakeryId)
            {
                return null;
            }
            return order;
        }

        public async Task<Order?> CreateOrderAsync(CreateOrderDto dto)
        {
            var bakeryId = _userService.GetMyBakeryId();
            var order = _mapper.Map<Order>(dto);
            order.BakeryId = bakeryId;
            order.CreatedAt = DateTime.UtcNow;

            decimal totalCost = 0;
            decimal totalSalePrice = 0;
            decimal totalYield = 0; 

            foreach (var op in order.OrderProducts)
            {
                var product = await _productRepo.GetByIdAsync(op.ProductId);
                if (product == null || product.BakeryId != bakeryId) throw new Exception("Invalid Product.");

                op.CostSnapshot = product.BaseCost ?? 0;
                op.UnitPriceSnapshot = product.SalePrice ?? 0;

                totalCost += (op.CostSnapshot.Value * op.Quantity);
                totalSalePrice += (op.UnitPriceSnapshot.Value * op.Quantity);
                totalYield += op.Quantity;
            }

            foreach (var or in order.OrderRecipes)
            {
                var recipe = await _recipeRepo.GetByIdWithComponentsAsync(or.RecipeId);
                if (recipe == null || recipe.BakeryId != bakeryId) throw new Exception("Invalid Recipe.");

                decimal recipeCost = 0;
                recipeCost += recipe.RecipeIngredients.Sum(ri => (ri.UnitPriceSnapshot ?? 0) * ri.Quantity);
                recipeCost += recipe.RecipeServices.Sum(rs => (rs.UnitPriceSnapshot ?? 0) * rs.Quantity);
                decimal recipeUnitCost = (recipe.YieldQuantity > 0) ? (recipeCost / recipe.YieldQuantity) : 0;
                recipeUnitCost *= (1 + recipe.AdditionalCostPercent / 100);

                or.CostSnapshot = recipeUnitCost;
                or.UnitPriceSnapshot = 0;
                totalYield += or.Quantity;

                totalCost += (or.CostSnapshot.Value * or.Quantity);
            }

            order.TotalCost = totalCost;
            order.SalePrice = totalSalePrice;
            order.Profit = totalSalePrice - totalCost;
            order.TotalYield = totalYield;

            return await _orderRepo.AddAsync(order);
        }

        public async Task<Order?> UpdateOrderAsync(int id, UpdateOrderDto dto)
        {
            var bakeryId = _userService.GetMyBakeryId();
            var existingOrder = await _orderRepo.GetByIdAsync(id);

            if (existingOrder == null || existingOrder.BakeryId != bakeryId)
            {
                return null;
            }

            _mapper.Map(dto, existingOrder);

            decimal totalCost = 0;
            decimal totalSalePrice = 0;
            decimal totalYield = 0;

            foreach (var op in existingOrder.OrderProducts)
            {
                var product = await _productRepo.GetByIdAsync(op.ProductId);
                if (product != null)
                {
                    op.CostSnapshot = product.BaseCost ?? 0;
                    op.UnitPriceSnapshot = product.SalePrice ?? 0;

                    totalCost += (op.CostSnapshot.Value * op.Quantity);
                    totalSalePrice += (op.UnitPriceSnapshot.Value * op.Quantity);
                    totalYield += op.Quantity;
                }
            }

            existingOrder.TotalCost = totalCost;
            existingOrder.SalePrice = totalSalePrice;
            existingOrder.Profit = totalSalePrice - totalCost;
            existingOrder.TotalYield = totalYield;

            return await _orderRepo.UpdateAsync(existingOrder);
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var bakeryId = _userService.GetMyBakeryId();
            var existingOrder = await _orderRepo.GetByIdAsync(id);

            if (existingOrder == null || existingOrder.BakeryId != bakeryId)
            {
                return false;
            }

            var result = await _orderRepo.DeleteAsync(id);
            return result != null;
        }

        public async Task<(bool IsSuccess, string Message)> BulkDeleteOrdersAsync(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any()) return (false, "Lista vazia.");

            var userBakeryId = _userService.GetMyBakeryId();
            var ordersToDelete = await _orderRepo.GetByIdsAsync(ids);

            var authorizedToDelete = ordersToDelete.Where(o => o.BakeryId == userBakeryId).ToList();

            if (!authorizedToDelete.Any()) return (true, "Nenhum pedido válido encontrado.");

            var success = await _orderRepo.DeleteRangeAsync(authorizedToDelete);

            return success
                ? (true, $"Sucesso! {authorizedToDelete.Count} pedidos excluídos.")
                : (false, "Erro ao salvar no banco.");
        }
    }
}