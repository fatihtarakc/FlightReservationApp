namespace App.Entity.Enums
{
    public enum VerificationCodePurpose
    {
        EmailConfirmation = 1,
        PasswordReset = 2,
        TwoFactorAuthentication = 3,
        AccountActivation = 4,
        PhoneVerification = 5
    }
}
