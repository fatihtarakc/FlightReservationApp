namespace App.Web.ViewModels.Admin
{
    public class AdminUserVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public UserStatus UserStatus { get; set; }
        public bool IsActive => UserStatus == UserStatus.Active;
        public bool EmailConfirmed { get; set; }
        public string Role { get; set; } = string.Empty;
        public int BookingCount { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("createdDate")]
        public DateTime CreatedAt { get; set; }
        public string FullName => $"{Name} {Surname}";
    }
}
