using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SweetfyAPI.DTOs.ServiceDTO;
using SweetfyAPI.Services; // Injecting the Service

namespace SweetfyAPI.Controllers
{
    /// <summary>
    /// Manages services (labor, fixed costs) for the logged-in user's bakery.
    /// </summary>
    [ApiController]
    [Route("api/services")]
    [Authorize]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _serviceService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceController"/>.
        /// </summary>
        /// <param name="serviceService">The service responsible for service logic.</param>
        /// <param name="mapper">The AutoMapper instance for DTO conversion.</param>
        public ServiceController(IServiceService serviceService, IMapper mapper)
        {
            _serviceService = serviceService;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all services associated with the logged-in user's bakery.
        /// </summary>
        /// <returns>A list of services.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ServiceDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ServiceDto>>> GetMyServices()
        {
            var services = await _serviceService.GetServicesForUserAsync();
            return Ok(_mapper.Map<IEnumerable<ServiceDto>>(services));
        }

        /// <summary>
        /// Gets a specific service by its ID.
        /// </summary>
        /// <param name="id">The ID of the service to retrieve.</param>
        /// <returns>The found service.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ServiceDto>> GetService(int id)
        {
            var service = await _serviceService.GetServiceByIdForUserAsync(id);
            if (service == null)
                return NotFound(new { Message = "Service not found." });

            return Ok(_mapper.Map<ServiceDto>(service));
        }

        /// <summary>
        /// Creates a new service for the user's bakery.
        /// </summary>
        /// <param name="dto">The data for the new service.</param>
        /// <returns>The newly created service.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<ServiceDto>> CreateService([FromBody] CreateServiceDto dto)
        {
            var newService = await _serviceService.CreateServiceAsync(dto);
            var readDto = _mapper.Map<ServiceDto>(newService);
            return CreatedAtAction(nameof(GetService), new { id = readDto.Id }, readDto);
        }

        /// <summary>
        /// Updates an existing service.
        /// </summary>
        /// <param name="id">The ID of the service to update.</param>
        /// <param name="dto">The updated data for the service.</param>
        /// <returns>No content (204) on success.</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateService(int id, [FromBody] UpdateServiceDto dto)
        {
            var updatedService = await _serviceService.UpdateServiceAsync(id, dto);
            if (updatedService == null)
                return NotFound(new { Message = "Service not found." });

            return NoContent();
        }

        /// <summary>
        /// Deletes a service by its ID.
        /// </summary>
        /// <param name="id">The ID of the service to delete.</param>
        /// <returns>No content (204) on success.</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteService(int id)
        {
            var result = await _serviceService.DeleteServiceAsync(id);
            if (!result)
                return NotFound(new { Message = "Service not found." });

            return NoContent();
        }
    }
}