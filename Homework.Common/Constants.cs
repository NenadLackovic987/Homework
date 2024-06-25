namespace Homework.Common
{
    public class Constants
    {
        public const string DB_CONN_IDENTITY_NAME = "IdentityDatabase";

        public const string TOKEN_LIFE_TIME = "Identity:TokenLifeTime";
        public const string REFRESH_TOKEN_LIFE_TIME = "Identity:RefreshTokenLifeTime";
        public const string API_BASE_URL = "AppSettings:ApiHostAddress";

        public const string USER_REGISTRATION_ERROR = "User registration error";
        public const string USER_ALREADY_REGISTERED = "User has been already registered in database.";
        public const string USER_TOKEN_CREATION_FAILURE = "Failed to create token.";

        public const string USER_LOGIN_ERROR = "User login error.";
        public const string USER_NOT_FOUND = "User not found.";
        public const string USER_WRONG_PASSWORD = "Wrong password.";
        public const string USER_NOT_ACTIVE = "User is not active.";
        public const string USER_LOCKOUT_ERROR = "User account is locked for next 24h and need to contact WEB Administrator.";

        public const string API_REFRESH_TOKEN_PATH = "api/Users/RefreshToken";
        public const string API_LOGIN_PATH = "api/Users/Login";
        public const string API_CONFIRM_USER_PASSWORD_RESET = "api/Administration/ConsumeUserSession";
        public const string API_CREATE_RESET_SESSION = "api/Administration/CreateResetSession";


        public const string API_POST_REGISTER_USER_PATH = "api/Users/Register";
        public const string API_PUT_USER_PROFILE_PATH = "api/Users/UpdateUserProfile";
        public const string API_PUT_RESET_PASSWORD = "api/Users/ChangePassword";      

        public const string API_MEDIA_TYPE_JSON = "application/json";
        public const string API_AUTHORIZATION_HEADER_BEARER = "Bearer";
    }
}
