namespace App.Core.Constants
{
    public class Messages
    {
        #region Account
        public const string Account_SignIn_Failed = "Account_SignIn_Failed";
        public const string Account_SignIn_Successful = "Account_SignIn_Successful";
        public const string Account_SignOut_Successful = "Account_SignOut_Successful";

        public const string Account_ActivateAccount_Failed = "Account_ActivateAccount_Failed";
        public const string Account_ActivateAccount_Successful = "Account_ActivateAccount_Successful";

        public const string Account_ConfirmEmail_Failed = "Account_ConfirmEmail_Failed";
        public const string Account_ConfirmEmail_Successful = "Account_ConfirmEmail_Successful";

        public const string Account_ResetPassword_Failed = "Account_ResetPassword_Failed";
        public const string Account_ResetPassword_Successful = "Account_ResetPassword_Successful";

        public const string Account_Was_Not_Found = "Account_Was_Not_Found";
        public const string Account_Was_Found = "Account_Was_Found";

        public const string Account_Has_Already_Been_Activated = "Account_Has_Already_Been_Activated";
        public const string Account_Has_Not_Activated = "Account_Has_Not_Activated";

        public const string Account_Email_Has_Already_Been_Confirmed = "Account_Email_Has_Already_Been_Confirmed";
        public const string Account_Email_Has_Not_Confirmed = "Account_Email_Has_Not_Confirmed";
        public const string Account_Email_Was_Confirmed = "Account_Email_Was_Confirmed";
        public const string Account_Email_Has_Already_Existed = "Account_Email_Has_Already_Existed";
        public const string Account_Email_Is_Invalid = "Account_Email_Is_Invalid";

        public const string Account_Username_Has_Already_Existed = "Account_Username_Has_Already_Existed";
        public const string Account_Role_Was_Not_Found_For_IdentityUser = "Account_Role_Was_Not_Found_For_IdentityUser";

        public const string Account_Please_Enter_Your_Name = "Account_Please_Enter_Your_Name";
        public const string Account_Digits_ForName_Cannot_Be_Used = "Account_Digits_ForName_Cannot_Be_Used";
        public const string Account_Name_Cannot_Be_LessThan_2Characters = "Account_Name_Cannot_Be_LessThan_2Characters";
        public const string Account_Name_Cannot_Be_GreaterThan_50Characters = "Account_Name_Cannot_Be_GreaterThan_50Characters";

        public const string Account_Please_Enter_Your_Surname = "Account_Please_Enter_Your_Surname";
        public const string Account_Surname_Cannot_Be_LessThan_2Characters = "Account_Surname_Cannot_Be_LessThan_2Characters";
        public const string Account_Surname_Cannot_Be_GreaterThan_50Characters = "Account_Surname_Cannot_Be_GreaterThan_50Characters";

        public const string Account_Please_Enter_Your_Username = "Account_Please_Enter_Your_Username";
        public const string Account_Username_Cannot_Be_LessThan_5Characters = "Account_Username_Cannot_Be_LessThan_5Characters";
        public const string Account_Username_Cannot_Be_GreaterThan_30Characters = "Account_Username_Cannot_Be_GreaterThan_30Characters";

        public const string Account_Please_Enter_Your_Email = "Account_Please_Enter_Your_Email";
        public const string Account_Please_Enter_Your_Email_With_Correct_Format = "Account_Please_Enter_Your_Email_With_Correct_Format";
        public const string Account_Email_Cannot_Be_LessThan_5Characters = "Account_Email_Cannot_Be_LessThan_5Characters";
        public const string Account_Email_Cannot_Be_GreaterThan_100Characters = "Account_Email_Cannot_Be_GreaterThan_100Characters";

        public const string Account_Please_Enter_Your_Password = "Account_Please_Enter_Your_Password";
        public const string Account_Password_Must_Include_1UpperLetter_AtLeast = "Account_Password_Must_Include_1UpperLetter_AtLeast";
        public const string Account_Password_Must_Include_1LowerLetter_AtLeast = "Account_Password_Must_Include_1LowerLetter_AtLeast";
        public const string Account_Password_Must_Include_1Digit_AtLeast = "Account_Password_Must_Include_1Digit_AtLeast";
        public const string Account_Password_Must_Include_1Symbol_AtLeast = "Account_Password_Must_Include_1Symbol_AtLeast";
        public const string Account_Password_Cannot_Be_LessThan_8Characters = "Account_Password_Cannot_Be_LessThan_8Characters";
        public const string Account_Password_Cannot_Be_GreaterThan_50Characters = "Account_Password_Cannot_Be_GreaterThan_50Characters";

        public const string Account_Please_Enter_Your_Birthdate = "Account_Please_Enter_Your_Birthdate";
        public const string Account_Birthdate_Age_Cannot_Be_LessThan_12YearsOld = "Account_Birthdate_Age_Cannot_Be_LessThan_12YearsOld";

        public const string Account_Please_Enter_Your_PhoneNumber = "Account_Please_Enter_Your_PhoneNumber";
        public const string Account_PhoneNumber_Is_Invalid = "Account_PhoneNumber_Is_Invalid";

        public const string Account_Please_Select_NotificationPreference = "Account_Please_Select_NotificationPreference";

        public const string Account_Please_Take_New_VerificationCode = "Account_Please_Take_New_VerificationCode";
        public const string Account_Please_Enter_Your_VerificationCode = "Account_Please_Enter_Your_VerificationCode";
        public const string Account_VerificationCode_Is_Invalid = "Account_VerificationCode_Is_Invalid";
        public const string Account_VerificationCode_Is_Expired = "Account_VerificationCode_Is_Expired";
        #endregion

        #region Token
        public const string Token_Could_Not_Generated = "Token_Could_Not_Generated";
        public const string Token_Was_Generated_Successfully = "Token_Was_Generated_Successfully";
        public const string Token_Is_Invalid = "Token_Is_Invalid";
        public const string Token_Is_Expired = "Token_Is_Expired";
        #endregion

        #region AppUser
        public const string AppUser_HasBeen_Added = "AppUser_HasBeen_Added";
        public const string AppUser_CouldNotBe_Added = "AppUser_CouldNotBe_Added";
        public const string AppUser_Was_Not_Found = "AppUser_Was_Not_Found";
        public const string AppUser_Was_Found = "AppUser_Was_Found";
        public const string AppUser_Was_Updated = "AppUser_Was_Updated";
        public const string AppUser_Could_Not_Be_Updated = "AppUser_Could_Not_Be_Updated";
        #endregion

        #region Airline
        public const string Airline_Was_Not_Found = "Airline_Was_Not_Found";
        public const string Airline_Was_Found = "Airline_Was_Found";
        public const string Airline_HasBeen_Added = "Airline_HasBeen_Added";
        public const string Airline_CouldNotBe_Added = "Airline_CouldNotBe_Added";
        public const string Airline_Was_Updated = "Airline_Was_Updated";
        public const string Airline_Could_Not_Be_Updated = "Airline_Could_Not_Be_Updated";
        public const string Airline_Was_Deleted = "Airline_Was_Deleted";
        public const string Airline_Could_Not_Be_Deleted = "Airline_Could_Not_Be_Deleted";
        public const string Airline_IataCode_Already_Exists = "Airline_IataCode_Already_Exists";
        #endregion

        #region Airport
        public const string Airport_Was_Not_Found = "Airport_Was_Not_Found";
        public const string Airport_Was_Found = "Airport_Was_Found";
        public const string Airport_HasBeen_Added = "Airport_HasBeen_Added";
        public const string Airport_CouldNotBe_Added = "Airport_CouldNotBe_Added";
        public const string Airport_Was_Updated = "Airport_Was_Updated";
        public const string Airport_Was_Deleted = "Airport_Was_Deleted";
        public const string Airport_IataCode_Already_Exists = "Airport_IataCode_Already_Exists";
        #endregion

        #region Aircraft
        public const string Aircraft_Was_Not_Found = "Aircraft_Was_Not_Found";
        public const string Aircraft_Was_Found = "Aircraft_Was_Found";
        public const string Aircraft_HasBeen_Added = "Aircraft_HasBeen_Added";
        public const string Aircraft_CouldNotBe_Added = "Aircraft_CouldNotBe_Added";
        public const string Aircraft_Was_Updated = "Aircraft_Was_Updated";
        public const string Aircraft_Was_Deleted = "Aircraft_Was_Deleted";
        public const string Aircraft_TailNumber_Already_Exists = "Aircraft_TailNumber_Already_Exists";
        #endregion

        #region Manufacturer
        public const string Manufacturer_Was_Not_Found = "Manufacturer_Was_Not_Found";
        public const string Manufacturer_Was_Found = "Manufacturer_Was_Found";
        public const string Manufacturer_HasBeen_Added = "Manufacturer_HasBeen_Added";
        public const string Manufacturer_CouldNotBe_Added = "Manufacturer_CouldNotBe_Added";
        public const string Manufacturer_Was_Updated = "Manufacturer_Was_Updated";
        public const string Manufacturer_Was_Deleted = "Manufacturer_Was_Deleted";
        public const string Manufacturer_Name_Already_Exists = "Manufacturer_Name_Already_Exists";
        #endregion

        #region Model
        public const string Model_Was_Not_Found = "Model_Was_Not_Found";
        public const string Model_Was_Found = "Model_Was_Found";
        public const string Model_HasBeen_Added = "Model_HasBeen_Added";
        public const string Model_CouldNotBe_Added = "Model_CouldNotBe_Added";
        public const string Model_Was_Updated = "Model_Was_Updated";
        public const string Model_Was_Deleted = "Model_Was_Deleted";
        #endregion

        #region Route
        public const string Route_Was_Not_Found = "Route_Was_Not_Found";
        public const string Route_Was_Found = "Route_Was_Found";
        public const string Route_HasBeen_Added = "Route_HasBeen_Added";
        public const string Route_CouldNotBe_Added = "Route_CouldNotBe_Added";
        public const string Route_Was_Updated = "Route_Was_Updated";
        public const string Route_Was_Deleted = "Route_Was_Deleted";
        public const string Route_Already_Exists = "Route_Already_Exists";
        #endregion

        #region Schedule
        public const string Schedule_Was_Not_Found = "Schedule_Was_Not_Found";
        public const string Schedule_Was_Found = "Schedule_Was_Found";
        public const string Schedule_HasBeen_Added = "Schedule_HasBeen_Added";
        public const string Schedule_CouldNotBe_Added = "Schedule_CouldNotBe_Added";
        public const string Schedule_Was_Updated = "Schedule_Was_Updated";
        public const string Schedule_Was_Deleted = "Schedule_Was_Deleted";
        #endregion

        #region Flight
        public const string Flight_Was_Not_Found = "Flight_Was_Not_Found";
        public const string Flight_Was_Found = "Flight_Was_Found";
        public const string Flight_HasBeen_Added = "Flight_HasBeen_Added";
        public const string Flight_CouldNotBe_Added = "Flight_CouldNotBe_Added";
        public const string Flight_Was_Updated = "Flight_Was_Updated";
        public const string Flight_Was_Deleted = "Flight_Was_Deleted";
        public const string Flight_Was_Cancelled = "Flight_Was_Cancelled";
        public const string Flight_Number_Already_Exists = "Flight_Number_Already_Exists";
        public const string Flight_No_Available_Seats = "Flight_No_Available_Seats";
        public const string Flight_Seat_Is_Already_Booked = "Flight_Seat_Is_Already_Booked";
        #endregion

        #region Seat
        public const string Seat_Was_Not_Found = "Seat_Was_Not_Found";
        public const string Seat_Was_Found = "Seat_Was_Found";
        public const string Seat_HasBeen_Added = "Seat_HasBeen_Added";
        public const string Seat_CouldNotBe_Added = "Seat_CouldNotBe_Added";
        public const string Seat_Was_Updated = "Seat_Was_Updated";
        public const string Seat_Was_Deleted = "Seat_Was_Deleted";
        #endregion

        #region Booking
        public const string Booking_Was_Not_Found = "Booking_Was_Not_Found";
        public const string Booking_Was_Found = "Booking_Was_Found";
        public const string Booking_HasBeen_Added = "Booking_HasBeen_Added";
        public const string Booking_CouldNotBe_Added = "Booking_CouldNotBe_Added";
        public const string Booking_Was_Updated = "Booking_Was_Updated";
        public const string Booking_Was_Cancelled = "Booking_Was_Cancelled";
        public const string Booking_Could_Not_Be_Cancelled = "Booking_Could_Not_Be_Cancelled";
        public const string Booking_Already_Exists_For_This_Flight = "Booking_Already_Exists_For_This_Flight";
        public const string Booking_Cannot_Cancel_Departed = "Booking_Cannot_Cancel_Departed";
        public const string Booking_CheckedIn_Successfully = "Booking_CheckedIn_Successfully";
        public const string Booking_Cannot_CheckIn_NotConfirmed = "Booking_Cannot_CheckIn_NotConfirmed";
        public const string Booking_Already_CheckedIn = "Booking_Already_CheckedIn";
        #endregion

        #region Email
        public const string Email_SendingProcess_Was_Failed = "Email_SendingProcess_Was_Failed";
        public const string Email_SendingProcess_Was_Successful = "Email_SendingProcess_Was_Successful";

        public const string EmailTitle_WelcomeNewUser = "EmailTitle_WelcomeNewUser";
        public const string EmailSubject_WelcomeNewUser = "EmailSubject_WelcomeNewUser";
        public const string EmailContent_WelcomeNewUser = "EmailContent_WelcomeNewUser";

        public const string EmailTitle_BookingConfirmation = "EmailTitle_BookingConfirmation";
        public const string EmailSubject_BookingConfirmation = "EmailSubject_BookingConfirmation";
        public const string EmailContent_BookingConfirmation = "EmailContent_BookingConfirmation";

        public const string EmailTitle_BookingCancellation = "EmailTitle_BookingCancellation";
        public const string EmailSubject_BookingCancellation = "EmailSubject_BookingCancellation";
        public const string EmailContent_BookingCancellation = "EmailContent_BookingCancellation";

        public const string EmailTitle_FlightReminder_7Days = "EmailTitle_FlightReminder_7Days";
        public const string EmailSubject_FlightReminder_7Days = "EmailSubject_FlightReminder_7Days";
        public const string EmailContent_FlightReminder_7Days = "EmailContent_FlightReminder_7Days";

        public const string EmailTitle_FlightReminder_24Hours = "EmailTitle_FlightReminder_24Hours";
        public const string EmailSubject_FlightReminder_24Hours = "EmailSubject_FlightReminder_24Hours";
        public const string EmailContent_FlightReminder_24Hours = "EmailContent_FlightReminder_24Hours";

        public const string EmailTitle_FlightCancelled = "EmailTitle_FlightCancelled";
        public const string EmailSubject_FlightCancelled = "EmailSubject_FlightCancelled";
        public const string EmailContent_FlightCancelled = "EmailContent_FlightCancelled";

        public const string EmailTitle_VerificationCode = "EmailTitle_VerificationCode";
        public const string EmailSubject_VerificationCode = "EmailSubject_VerificationCode";
        public const string EmailContent_VerificationCode = "EmailContent_VerificationCode";

        public const string EmailTitle_PasswordReset = "EmailTitle_PasswordReset";
        public const string EmailSubject_PasswordReset = "EmailSubject_PasswordReset";
        public const string EmailContent_PasswordReset = "EmailContent_PasswordReset";

        public const string EmailTitle_PasswordChanged = "EmailTitle_PasswordChanged";
        public const string EmailSubject_PasswordChanged = "EmailSubject_PasswordChanged";
        public const string EmailContent_PasswordChanged = "EmailContent_PasswordChanged";

        // Shared email labels
        public const string Email_Dear = "Email_Dear";
        public const string Email_Footer = "Email_Footer";
        public const string Email_Label_PnrNumber = "Email_Label_PnrNumber";
        public const string Email_Label_Flight = "Email_Label_Flight";
        public const string Email_Label_Route = "Email_Label_Route";
        public const string Email_Label_Departure = "Email_Label_Departure";
        public const string Email_Label_ScheduledDeparture = "Email_Label_ScheduledDeparture";
        public const string Email_Label_Seat = "Email_Label_Seat";
        public const string Email_Label_TotalAmount = "Email_Label_TotalAmount";
        public const string Email_Label_CancellationReason = "Email_Label_CancellationReason";
        public const string Email_Label_FullName = "Email_Label_FullName";
        public const string Email_Label_Email = "Email_Label_Email";
        public const string Email_Label_OperationTime = "Email_Label_OperationTime";

        // BookingConfirmed email body
        public const string Email_BookingConfirmed_HeaderSubtitle = "Email_BookingConfirmed_HeaderSubtitle";
        public const string Email_BookingConfirmed_Heading = "Email_BookingConfirmed_Heading";
        public const string Email_BookingConfirmed_Intro = "Email_BookingConfirmed_Intro";
        public const string Email_BookingConfirmed_SectionTitle = "Email_BookingConfirmed_SectionTitle";
        public const string Email_BookingConfirmed_Note = "Email_BookingConfirmed_Note";

        // BookingCancelled email body
        public const string Email_BookingCancelled_HeaderSubtitle = "Email_BookingCancelled_HeaderSubtitle";
        public const string Email_BookingCancelled_Heading = "Email_BookingCancelled_Heading";
        public const string Email_BookingCancelled_Intro = "Email_BookingCancelled_Intro";
        public const string Email_BookingCancelled_SectionTitle = "Email_BookingCancelled_SectionTitle";
        public const string Email_BookingCancelled_ContactNote = "Email_BookingCancelled_ContactNote";

        // FlightCancelled email body
        public const string Email_FlightCancelled_HeaderSubtitle = "Email_FlightCancelled_HeaderSubtitle";
        public const string Email_FlightCancelled_Heading = "Email_FlightCancelled_Heading";
        public const string Email_FlightCancelled_Intro = "Email_FlightCancelled_Intro";
        public const string Email_FlightCancelled_SectionTitle = "Email_FlightCancelled_SectionTitle";
        public const string Email_FlightCancelled_Warning = "Email_FlightCancelled_Warning";

        // FlightReminder email body
        public const string Email_FlightReminder_HeaderSubtitle = "Email_FlightReminder_HeaderSubtitle";
        public const string Email_FlightReminder_SectionTitle = "Email_FlightReminder_SectionTitle";
        public const string Email_FlightReminder_Closing = "Email_FlightReminder_Closing";
        public const string Email_FlightReminder_7Days_Badge = "Email_FlightReminder_7Days_Badge";
        public const string Email_FlightReminder_7Days_Desc = "Email_FlightReminder_7Days_Desc";
        public const string Email_FlightReminder_24Hours_Badge = "Email_FlightReminder_24Hours_Badge";
        public const string Email_FlightReminder_24Hours_Desc = "Email_FlightReminder_24Hours_Desc";

        // VerificationCode email body
        public const string Email_VerificationCode_HeaderSubtitle = "Email_VerificationCode_HeaderSubtitle";
        public const string Email_VerificationCode_Intro = "Email_VerificationCode_Intro";
        public const string Email_VerificationCode_Warning = "Email_VerificationCode_Warning";
        public const string Email_VerificationCode_NotRequested = "Email_VerificationCode_NotRequested";

        // UserSignedUp email body
        public const string Email_UserSignedUp_HeaderSubtitle = "Email_UserSignedUp_HeaderSubtitle";
        public const string Email_UserSignedUp_Intro = "Email_UserSignedUp_Intro";
        public const string Email_UserSignedUp_ButtonText = "Email_UserSignedUp_ButtonText";
        public const string Email_UserSignedUp_AccountInfo = "Email_UserSignedUp_AccountInfo";
        public const string Email_UserSignedUp_Note = "Email_UserSignedUp_Note";

        // PasswordChanged email body
        public const string Email_PasswordChanged_HeaderSubtitle = "Email_PasswordChanged_HeaderSubtitle";
        public const string Email_PasswordChanged_Heading = "Email_PasswordChanged_Heading";
        public const string Email_PasswordChanged_Intro = "Email_PasswordChanged_Intro";
        public const string Email_PasswordChanged_Warning = "Email_PasswordChanged_Warning";

        // SMS / WhatsApp templates
        public const string Sms_BookingConfirmed = "Sms_BookingConfirmed";
        public const string Sms_BookingCancelled = "Sms_BookingCancelled";
        public const string Sms_FlightCancelled = "Sms_FlightCancelled";
        public const string Sms_FlightReminder = "Sms_FlightReminder";
        public const string Sms_VerificationCode = "Sms_VerificationCode";
        public const string Sms_UserSignedUp = "Sms_UserSignedUp";
        public const string Sms_PasswordChanged = "Sms_PasswordChanged";
        public const string WhatsApp_BookingConfirmed = "WhatsApp_BookingConfirmed";
        public const string WhatsApp_BookingCancelled = "WhatsApp_BookingCancelled";
        public const string WhatsApp_FlightCancelled = "WhatsApp_FlightCancelled";
        public const string WhatsApp_FlightReminder = "WhatsApp_FlightReminder";
        public const string WhatsApp_VerificationCode = "WhatsApp_VerificationCode";
        public const string WhatsApp_UserSignedUp = "WhatsApp_UserSignedUp";
        public const string WhatsApp_PasswordChanged = "WhatsApp_PasswordChanged";
        #endregion

        #region Sms
        public const string Sms_SendingProcess_Was_Failed = "Sms_SendingProcess_Was_Failed";
        public const string Sms_SendingProcess_Was_Successful = "Sms_SendingProcess_Was_Successful";
        public const string WhatsApp_SendingProcess_Was_Failed = "WhatsApp_SendingProcess_Was_Failed";
        public const string WhatsApp_SendingProcess_Was_Successful = "WhatsApp_SendingProcess_Was_Successful";
        #endregion

        #region Redis
        public const string Redis_Cache_Entity_Was_Found = "Redis_Cache_Entity_Was_Found";
        public const string Redis_Cache_Entity_Was_Not_Found = "Redis_Cache_Entity_Was_Not_Found";
        public const string Redis_Cache_Entity_Was_Added = "Redis_Cache_Entity_Was_Added";
        public const string Redis_Cache_Entity_Could_Not_Be_Added = "Redis_Cache_Entity_Could_Not_Be_Added";
        public const string Redis_Cache_Entity_Was_Deleted = "Redis_Cache_Entity_Was_Deleted";
        public const string Redis_Cache_Entity_Could_Not_Be_Deleted = "Redis_Cache_Entity_Could_Not_Be_Deleted";
        #endregion

        #region RabbitMQ / MassTransit
        public const string MassTransit_Publish_Was_Failed = "MassTransit_Publish_Was_Failed";
        public const string MassTransit_Publish_Was_Successful = "MassTransit_Publish_Was_Successful";
        public const string MassTransit_Consume_Was_Failed = "MassTransit_Consume_Was_Failed";
        public const string MassTransit_Consume_Was_Successful = "MassTransit_Consume_Was_Successful";
        #endregion

        #region General
        public const string UnexpectedError = "UnexpectedError";
        public const string ValidationFailed = "ValidationFailed";
        public const string NotFound = "NotFound";
        public const string Unauthorized = "Unauthorized";
        public const string Forbidden = "Forbidden";
        #endregion
    }
}
