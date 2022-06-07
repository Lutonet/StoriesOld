namespace Stories.Tools
{
    public static class Settings
    {
        public const string Administrator = "Administrator";
        public const string Editor = "Editor";
        public const string Redactor = "Redactor";
        public const string Moderator = "Moderator";
        public const string Subscriber = "Subscriber";
        public const int ageLimit = 13;
        public const string UserPicturesPath = "/img/authors/";

        public enum AccessRights
        {
            Public,
            Registered,
            Friends,
            Private
        };

        public enum Gender
        {
            Female,
            Male,
            Other
        };

        public enum RoomType
        {
            Room,
            Private
        };

        public const string DefaultTheme = "Light";
      
        public const string DefaultChatroom = "Living room";
    }
}