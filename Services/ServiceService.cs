using AutoMapper;
using SweetfyAPI.DTOs.ServiceDTO;
using SweetfyAPI.Repositories;

namespace SweetfyAPI.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepo;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ICostPropagationService _costPropagationService;

        public ServiceService(
            IServiceRepository serviceRepo,
            IUserService userService,
            IMapper mapper,
            ICostPropagationService costPropagationService)
        {
            _serviceRepo = serviceRepo;
            _userService = userService;
            _mapper = mapper;
            _costPropagationService = costPropagationService;
        }

        public async Task<IEnumerable<Service>> GetServicesForUserAsync()
        {
            var bakeryId = _userService.GetMyBakeryId();
            return await _serviceRepo.GetByBakeryIdAsync(bakeryId);
        }

        public async Task<Service?> GetServiceByIdForUserAsync(int id)
        {
            var bakeryId = _userService.GetMyBakeryId();
            var service = await _serviceRepo.GetByIdAsync(id);

            if (service == null || service.BakeryId != bakeryId)
            {
                return null;
            }

            return service;
        }

        public async Task<Service> CreateServiceAsync(CreateServiceDto dto)
        {
            var bakeryId = _userService.GetMyBakeryId();

            var service = _mapper.Map<Service>(dto);
            service.BakeryId = bakeryId;
            service.CreatedAt = DateTime.UtcNow;

            return await _serviceRepo.AddAsync(service);
        }

        public async Task<Service?> UpdateServiceAsync(int id, UpdateServiceDto dto)
        {
            var bakeryId = _userService.GetMyBakeryId();
            var existingService = await _serviceRepo.GetByIdAsync(id);

            if (existingService == null || existingService.BakeryId != bakeryId)
            {
                return null;
            }

            _mapper.Map(dto, existingService);

            await _serviceRepo.UpdateAsync(existingService);

            await _costPropagationService.PropagateServiceChangesAsync(id, bakeryId);

            return existingService;
        }

        public async Task<bool> DeleteServiceAsync(int id)
        {
            var bakeryId = _userService.GetMyBakeryId();
            var existingService = await _serviceRepo.GetByIdAsync(id);

            if (existingService == null || existingService.BakeryId != bakeryId)
            {
                return false;
            }

            var result = await _serviceRepo.DeleteAsync(id);
            return result != null;
        }

        public async Task<(bool IsSuccess, string Message)> BulkDeleteServicesAsync(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any()) return (false, "Nenhum ID fornecido.");

            var userBakeryId = _userService.GetMyBakeryId();
            var servicesToDelete = await _serviceRepo.GetByIdsAsync(ids); 

            var authorizedToDelete = servicesToDelete.Where(s => s.BakeryId == userBakeryId).ToList();

            if (!authorizedToDelete.Any()) return (true, "Nenhum serviço válido encontrado.");

            var success = await _serviceRepo.DeleteRangeAsync(authorizedToDelete); 

            if (success) return (true, $"Sucesso! {authorizedToDelete.Count} serviços excluídos.");
            else return (false, "Erro ao salvar alterações.");
        }

        public async Task<(bool IsSuccess, string Message)> BulkUpdatePricesAsync(List<BulkUpdateServicePriceDto> updates)
        {
            var bakeryId = _userService.GetMyBakeryId();
            int count = 0;

            foreach (var item in updates)
            {
                var service = await _serviceRepo.GetByIdAsync(item.Id);

                if (service != null && service.BakeryId == bakeryId)
                {
                    // 1. Atualiza preço
                    service.UnitPrice = item.NewPrice;

                    await _serviceRepo.UpdateAsync(service);

                    // 2. Dispara a propagação
                    await _costPropagationService.PropagateServiceChangesAsync(item.Id, bakeryId);

                    count++;
                }
            }

            return (true, $"{count} preços de serviços atualizados.");
        }
    }
}