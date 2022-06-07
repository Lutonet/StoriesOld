using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stories.Model
{
    public class ChatRooms
    {
        [Key]
        public string RoomName { get; set; }

        public string UserId { get; set; }
        public bool IsActive { get; set; } = true;
        [Column(TypeName = "varchar(16)")]
        public string RoomType { get; set; }
        public DateTime Created { get; set; }
        public ICollection<UserInRoom> UsersInRooms { get; set; }
        public ICollection<ChatRoomAdmins> ChatRoomAdmins { get; set; }
    }
}