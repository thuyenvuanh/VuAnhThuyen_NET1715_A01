using System;
using System.Collections.Generic;

namespace Repository.Models
{
    public partial class RoomType
    {
        public RoomType()
        {
            Rooms = new HashSet<Room>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Note { get; set; }

        public virtual ICollection<Room> Rooms { get; set; }
    }
}
