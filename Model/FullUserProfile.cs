using System;
using System.Collections.Generic;
using static Stories.API.GetUserProfile;
using static Stories.Tools.Settings;
using Stories.Pages.Profile;

namespace Stories.Model
{
    public class FullUserProfile
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string DisplayedName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime RegistrationTime { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime LastSeen { get; set; }
        public string CountryName { get; set; }
        public string PhoneNumber { get; set; }
        public Gender Gender { get; set; }
        public string Info { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Google { get; set; }
        public string Microsoft { get; set; }
        public int Articles { get; set; }
        public int Critics { get; set; }
        public int Age { get; set; }
        public List<Friendslist> Friends { get; set; }
        public string JsonFriendsList { get; set; }
        public string PictureUrl { get; set; }
        public bool ShowEmail { get; set; } = false;
        public bool ShowName {get; set; } = false;
        public bool ShowFriends {get; set;} = false;
        public bool ShowPhone {get; set; } = false;
        public bool ShowSocialMedia {get; set;} = false;
        public bool ShowCountry {get; set;} = false;
        public bool ShowBirthDate {get; set;} = false;
        public bool ShowPicture {get; set;} = false;
        public bool ShowLastSeen {get; set;} = false;
    }
}