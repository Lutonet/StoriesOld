using System.ComponentModel;
using static Stories.Tools.Settings;

namespace Stories.Model
{
    public class UserSettings
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

        // Privacy  0 = public 1 = registered user 2 friends 3 private
        [DisplayName("Who can see your email")]
        public AccessRights EmailPrivacy { get; set; } = AccessRights.Private;

        [DisplayName("Who can see your real name")]
        public AccessRights NamePrivacy { get; set; } = AccessRights.Private;

        [DisplayName("Who can see your friends list")]
        public AccessRights FrindsPrivacy { get; set; } = AccessRights.Public;

        [DisplayName("Who can see your phone number")]
        public AccessRights PhonePrivacy { get; set; } = AccessRights.Private;

        [DisplayName("Who can see your Social Media links")]
        public AccessRights SocialMediaLinksPrivacy { get; set; } = AccessRights.Public;

        [DisplayName("Who can see your country?")]
        public AccessRights CountryPrivacy { get; set; } = AccessRights.Public;

        [DisplayName("Who can see your birthday?")]
        public AccessRights BirthDatePrivacy { get; set; } = AccessRights.Public;

        [DisplayName("Who can see your picture?")]
        public AccessRights PictureFilePrivacy { get; set; } = AccessRights.Public;

        [DisplayName("Who can see you online?")]
        public AccessRights LastSeenPrivacy { get; set; } = AccessRights.Public;
    }
}