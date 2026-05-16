namespace App.Web.Constants
{
    public static class Messages
    {
        #region General
        public const string UnexpectedError = nameof(UnexpectedError);
        public const string Data_LoadSuccess = nameof(Data_LoadSuccess);
        #endregion

        #region Account
        public const string Account_SignIn_Successful = nameof(Account_SignIn_Successful);
        public const string Account_SignIn_Failed = nameof(Account_SignIn_Failed);
        public const string Account_SignOut_Successful = nameof(Account_SignOut_Successful);
        public const string Account_ResetPassword_Successful = nameof(Account_ResetPassword_Successful);
        public const string Account_ForgotPassword_Successful = nameof(Account_ForgotPassword_Successful);
        public const string Account_Was_Not_Found = nameof(Account_Was_Not_Found);
        #endregion

        #region AppUser
        public const string AppUser_HasBeen_Added = nameof(AppUser_HasBeen_Added);
        public const string AppUser_CouldNotBe_Added = nameof(AppUser_CouldNotBe_Added);
        public const string AppUser_Was_Not_Found = nameof(AppUser_Was_Not_Found);
        public const string AppUser_Was_Activated = nameof(AppUser_Was_Activated);
        public const string AppUser_Was_Deactivated = nameof(AppUser_Was_Deactivated);
        #endregion

        #region Flight
        public const string Flight_Was_Not_Found = nameof(Flight_Was_Not_Found);
        public const string Flight_HasBeen_Added = nameof(Flight_HasBeen_Added);
        public const string Flight_Was_Updated = nameof(Flight_Was_Updated);
        public const string Flight_Was_Deleted = nameof(Flight_Was_Deleted);
        public const string Flight_Was_Cancelled = nameof(Flight_Was_Cancelled);
        #endregion

        #region Route
        public const string Route_Was_Not_Found = nameof(Route_Was_Not_Found);
        public const string Route_HasBeen_Added = nameof(Route_HasBeen_Added);
        public const string Route_Was_Updated = nameof(Route_Was_Updated);
        public const string Route_Was_Deleted = nameof(Route_Was_Deleted);
        #endregion

        #region Airport
        public const string Airport_Was_Not_Found = nameof(Airport_Was_Not_Found);
        public const string Airport_HasBeen_Added = nameof(Airport_HasBeen_Added);
        public const string Airport_Was_Updated = nameof(Airport_Was_Updated);
        public const string Airport_Was_Deleted = nameof(Airport_Was_Deleted);
        #endregion

        #region Aircraft
        public const string Aircraft_Was_Not_Found = nameof(Aircraft_Was_Not_Found);
        public const string Aircraft_HasBeen_Added = nameof(Aircraft_HasBeen_Added);
        public const string Aircraft_Was_Updated = nameof(Aircraft_Was_Updated);
        public const string Aircraft_Was_Deleted = nameof(Aircraft_Was_Deleted);
        #endregion

        #region Booking
        public const string Booking_Was_Not_Found = nameof(Booking_Was_Not_Found);
        public const string Booking_HasBeen_Added = nameof(Booking_HasBeen_Added);
        public const string Booking_Was_Cancelled = nameof(Booking_Was_Cancelled);
        public const string Booking_CheckedIn_Successfully = nameof(Booking_CheckedIn_Successfully);
        #endregion

        #region Seat
        public const string Seat_Was_Not_Found = nameof(Seat_Was_Not_Found);
        #endregion

        #region Profile (App.Web specific)
        public const string Profile_UpdateSuccess = nameof(Profile_UpdateSuccess);
        public const string Password_ChangeSuccess = nameof(Password_ChangeSuccess);
        public const string Notif_UpdateSuccess = nameof(Notif_UpdateSuccess);
        public const string Dashboard_LoadError = nameof(Dashboard_LoadError);
        #endregion

        #region Validation
        public const string Val_UsernameOrEmail_Required = nameof(Val_UsernameOrEmail_Required);
        public const string Val_UsernameOrEmail_MinLength = nameof(Val_UsernameOrEmail_MinLength);
        public const string Val_Password_Required = nameof(Val_Password_Required);
        public const string Val_Password_TooShort = nameof(Val_Password_TooShort);
        public const string Val_Password_SignUp_TooShort = nameof(Val_Password_SignUp_TooShort);
        public const string Val_Password_Uppercase = nameof(Val_Password_Uppercase);
        public const string Val_Password_Lowercase = nameof(Val_Password_Lowercase);
        public const string Val_Password_Digit = nameof(Val_Password_Digit);
        public const string Val_Password_Special = nameof(Val_Password_Special);
        public const string Val_ConfirmPassword_Required = nameof(Val_ConfirmPassword_Required);
        public const string Val_ConfirmPassword_Match = nameof(Val_ConfirmPassword_Match);
        public const string Val_Name_Required = nameof(Val_Name_Required);
        public const string Val_Name_TooShort = nameof(Val_Name_TooShort);
        public const string Val_Name_TooLong = nameof(Val_Name_TooLong);
        public const string Val_Surname_Required = nameof(Val_Surname_Required);
        public const string Val_Surname_TooShort = nameof(Val_Surname_TooShort);
        public const string Val_Surname_TooLong = nameof(Val_Surname_TooLong);
        public const string Val_Username_Required = nameof(Val_Username_Required);
        public const string Val_Username_Format = nameof(Val_Username_Format);
        public const string Val_Email_Required = nameof(Val_Email_Required);
        public const string Val_Email_Invalid = nameof(Val_Email_Invalid);
        public const string Val_Phone_Required = nameof(Val_Phone_Required);
        public const string Val_Phone_Format = nameof(Val_Phone_Format);
        public const string Val_BirthDate_Required = nameof(Val_BirthDate_Required);
        public const string Val_BirthDate_MinAge = nameof(Val_BirthDate_MinAge);
        public const string Val_BirthDate_Valid = nameof(Val_BirthDate_Valid);
        public const string Val_Code_Required = nameof(Val_Code_Required);
        public const string Val_Code_Length = nameof(Val_Code_Length);
        public const string Val_DepartureIata_Required = nameof(Val_DepartureIata_Required);
        public const string Val_ArrivalIata_Required = nameof(Val_ArrivalIata_Required);
        public const string Val_IATA_Length = nameof(Val_IATA_Length);
        public const string Val_IATA_Different = nameof(Val_IATA_Different);
        public const string Val_Date_NotPast = nameof(Val_Date_NotPast);
        public const string Val_Passengers_Range = nameof(Val_Passengers_Range);
        public const string Val_FlightNumber_Required = nameof(Val_FlightNumber_Required);
        public const string Val_FlightNumber_MaxLength = nameof(Val_FlightNumber_MaxLength);
        public const string Val_DepartureTime_Future = nameof(Val_DepartureTime_Future);
        public const string Val_ArrivalTime_AfterDeparture = nameof(Val_ArrivalTime_AfterDeparture);
        public const string Val_EconomyPrice_Positive = nameof(Val_EconomyPrice_Positive);
        public const string Val_AircraftId_Required = nameof(Val_AircraftId_Required);
        public const string Val_RouteId_Required = nameof(Val_RouteId_Required);
        public const string Val_FlightId_Required = nameof(Val_FlightId_Required);
        public const string Val_SeatId_Required = nameof(Val_SeatId_Required);
        #endregion
    }
}
