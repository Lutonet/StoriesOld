using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stories.Model
{
    public class Club_Users
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int ClubId { get; set; }

        public User User { get; set; }

        public Club Club { get; set; }
    }
}
