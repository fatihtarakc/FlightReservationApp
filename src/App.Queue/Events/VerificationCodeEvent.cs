namespace App.Queue.Events
{
    public record VerificationCodeEvent
    {
        public string? Name { get; init; }
        public string? Email { get; init; }
        public string? PhoneNumber { get; init; }
        public string? Code { get; init; }
        public VerificationCodePurpose Purpose { get; init; }
        public VerificationCodeChannel Channel { get; init; }
    }
}
