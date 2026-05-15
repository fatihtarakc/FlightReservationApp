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
        public bool IsActive { get; set; }
        public string Role { get; set; } = string.Empty;
        public int BookingCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string FullName => $"{Name} {Surname}";
    }
}
