using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Repository.Models
{
    public partial class Reservation
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public DateTime OnDate { get; set; }
        [Required]
        public int? RoomId { get; set; }
        [Required]
        public int? CustomerId { get; set; }
        [Required]
        public string Status { get; set; } = null!;

        public virtual Customer? Customer { get; set; }
        public virtual Room? Room { get; set; }
    }
}
