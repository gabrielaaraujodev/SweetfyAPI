namespace SweetfyAPI.DTOs.UserDTO
{
    public record UserDto(
            string Id,
            string FullName,
            string Email,
            int BakeryId 
        );
}
