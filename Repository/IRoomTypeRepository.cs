using Repository.Models;

namespace Repository
{
    public interface IRoomTypeRepository
    {
        public RoomType? GetRoomTypeById(int id);
        public List<RoomType> GetAll();
        public RoomType Create(RoomType roomType);
        public RoomType Update(RoomType roomType);
        public void Delete(int id);
    }
}
