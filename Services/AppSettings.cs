namespace Stories.Services
{
    public class AppSettings
    {
        public string ServerAddress { get; set; }
        public int ServerPort { get; set; }
        public bool RequiresAuthentication { get; set; }
        public string EmailUser { get; set; }
        public string Password { get; set; }
        public string Security { get; set; }
    }
}