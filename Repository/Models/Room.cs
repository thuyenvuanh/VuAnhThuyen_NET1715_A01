using System;
using System.Collections.Generic;

namespace Repository.Models
{
    public partial class Room
    {
        public Room()
        {
            Reservations = new HashSet<Reservation>();
        }

        public int Id { get; set; }
        public int Number { get; set; }
        public string? Description { get; set; }
        public int MaxCapacity { get; set; }
        public bool? Status { get; set; }
        public decimal PricePerDate { get; set; }
        public int? TypeId { get; set; }

        public virtual RoomType? Type { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
