using Repository.Models;

namespace Repository
{
    public interface IRoomRepository
    {
        public Room? GetRoomById(int id);
        public List<Room> GetAll();
        public Room Create(Room room);
        public Room Update(Room room);
        public void Delete(int id);
    }
}
