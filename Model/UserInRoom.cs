using System;
using System.ComponentModel.DataAnnotations;

namespace Stories.Model
{
    public class UserInRoom
    {
        public int Id { get; set; }
        [MaxLength(64)]
        public string RoomName { get; set; }

        public string UserId { get; set; }
        public string ConnectionId { get; set; }
        [MaxLength(64)]
        public string DisplayedName { get; set; }
        public bool IsAdmin { get; set; } = false;
        public bool IsOwner { get; set; } = false;
        public DateTime JoiningTime { get; set; }
        public DateTime LeavingTime { get; set; }
    }
}