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

        public ServiceService(
            IServiceRepository serviceRepo,
            IUserService userService,
            IMapper mapper)
        {
            _serviceRepo = serviceRepo;
            _userService = userService;
            _mapper = mapper;
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

            return await _serviceRepo.UpdateAsync(existingService);
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
    }
}