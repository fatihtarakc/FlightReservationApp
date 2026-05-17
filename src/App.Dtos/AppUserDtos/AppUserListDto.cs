namespace App.Dtos.AppUserDtos
{
    public record AppUserListDto(
        Guid Id,
        string Name,
        string Surname,
        string Email,
        string PhoneNumber,
        UserStatus UserStatus);
}