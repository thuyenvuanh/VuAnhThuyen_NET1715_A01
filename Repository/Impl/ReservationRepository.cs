using Microsoft.EntityFrameworkCore;
using Repository.Models;

namespace Repository.Impl
{
    public class ReservationRepository : RepositoryBase<Reservation>, IReservationRepository
    {
        private readonly HotelManagementContext _context;
        private readonly DbSet<Reservation> _reservations;

        public ReservationRepository(HotelManagementContext context) : base(context)
        {
            _context = context;
            _reservations = context.Reservations;
        }

        public Reservation Create(Reservation reservation)
        {
            var id = GetLatestId() + 1;
            reservation.Id = id;
            Add(reservation);
            return GetReservationById(id)!;
        }

        public void Delete(int id)
        {
            var reservation = _reservations.FirstOrDefault(r => r.Id == id);
            if (reservation == null)
            {
                return;
            }
            reservation.Status = "CANCELLED";
            Commit();
        }

        public Reservation? GetReservationById(int id)
        {
            return _reservations.Include("Customer").Include("Room").FirstOrDefault(r => r.Id.Equals(id));
        }

        List<Reservation> IReservationRepository.GetAll()
        {
            return _reservations.Include("Room").Include("Customer").ToList();
        }

        Reservation IReservationRepository.Update(Reservation reservation)
        {
            //detach entity
            var existed = _reservations.SingleOrDefault(r => r.Id == reservation.Id);
            if (existed != null)
            {
                reservation.CustomerId = existed.CustomerId;
                reservation.RoomId = existed.RoomId;
                _context.Entry(existed).State = EntityState.Detached;
            }
            _context.Attach(reservation);
            _context.Entry(reservation).State = EntityState.Modified;
            _context.SaveChanges();
            return reservation;
        }

        public int GetLatestId()
        {
            return GetAll()
                .OrderByDescending(x => x.Id)
                .FirstOrDefault()?.Id ?? 0;
        }
    }
}
