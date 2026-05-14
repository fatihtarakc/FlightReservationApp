namespace App.Dtos.AppUserDtos
{
    public record AppUserDto(
        Guid Id,
        string Name,
        string Surname,
        string Email,
        string PhoneNumber,
        DateTime BirthDate,
        UserStatus UserStatus,
        NotificationChannel PreferredNotificationChannel,
        string? Nationality,
        string? PassportNumber);
}
