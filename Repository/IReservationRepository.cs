using Repository.Models;

namespace Repository
{
    public interface IReservationRepository
    {
        public Reservation? GetReservationById(int id);
        public List<Reservation> GetAll();
        public Reservation Create(Reservation reservation);
        public Reservation Update(Reservation reservation);
        public void Delete(int id);
    }
}
