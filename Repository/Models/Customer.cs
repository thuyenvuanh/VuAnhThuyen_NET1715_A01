using System;
using System.Collections.Generic;

namespace Repository.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Reservations = new HashSet<Reservation>();
        }

        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string? Telephone { get; set; }
        public string Email { get; set; } = null!;
        public DateTime? Birthday { get; set; }
        public bool? Status { get; set; }
        public string Password { get; set; } = null!;

        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
