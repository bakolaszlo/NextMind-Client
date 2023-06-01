public class Constants
{
    public const string API_URL = "http://localhost:7058/"; //"https://nextmindbe.azurewebsites.net/"; //"https://localhost:7058/";
    private const string AUTH = "api/Auth/";

    public const string LOGIN_ENDPOINT = AUTH + "login";
    public const string REGISTER_ENDPOINT = AUTH + "register";
    public const string PING_ENDPOINT = "api/pings";
    public const string TRIGGER_ENDPOINT = "api/trigger";

    public static string JWT = string.Empty;

    public static string PUBLIC_KEY = "api/DH";
}
