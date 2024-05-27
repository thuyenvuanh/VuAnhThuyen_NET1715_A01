using Microsoft.EntityFrameworkCore;
using Repository.Models;

namespace Repository.Impl
{
    public class RoomTypeRepository : RepositoryBase<RoomType>, IRoomTypeRepository
    {
        private readonly HotelManagementContext context;
        private readonly DbSet<RoomType> _roomTypes;
        public RoomTypeRepository(HotelManagementContext context) : base(context)
        {
            this.context = context;
            _roomTypes = context.RoomTypes;
        }

        public RoomType Create(RoomType roomType)
        {
            int id = GetLatestId() + 1;
            roomType.Id = id;
            Add(roomType);
            return GetRoomTypeById(id)!;
        }

        public void Delete(int id)
        {
            var roomType = GetRoomTypeById(id);
            if (roomType == null)
            {
                return;
            }
            //Delete(roomType);
        }

        public RoomType? GetRoomTypeById(int id)
        {
            return _roomTypes.SingleOrDefault(rt => rt.Id == id);
        }

        List<RoomType> IRoomTypeRepository.GetAll()
        {
            return _roomTypes.ToList();
        }

        RoomType IRoomTypeRepository.Update(RoomType roomType)
        {
            //detach entity
            var existed = _roomTypes.Local.FirstOrDefault(rt => rt.Id == roomType.Id);
            if (existed != null)
            {
                context.Entry(existed).State = EntityState.Detached;
            }
            context.Attach(roomType);
            context.Entry(roomType).State = EntityState.Modified;
            context.SaveChanges();
            return roomType;
        }

        private int GetLatestId()
        {
            return GetAll()
                .OrderByDescending(p => p.Id).FirstOrDefault()
                ?.Id ?? 0;
        }
    }
}
