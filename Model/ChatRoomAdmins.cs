using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stories.Model
{
    public class ChatRoomAdmins
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        [MaxLength(64)]
        public string RoomName { get; set; }
        [Column(TypeName = "varchar(128)")]
        public string AdminHash { get; set; }
        public bool isElevated { get; set; } = false;
    }
}