using Microsoft.EntityFrameworkCore;
using Repository.Models;

namespace Repository.Impl
{
    public class RoomRepository : RepositoryBase<Room>, IRoomRepository
    {
        private readonly HotelManagementContext _context;
        private readonly DbSet<Room> _rooms;
        public RoomRepository(HotelManagementContext context) : base(context)
        {
            _context = context;
            _rooms = context.Rooms;
        }

        public Room Create(Room room)
        {
            var id = GetLatestId() + 1;
            room.Id = id;
            Add(room);
            return GetRoomById(id)!;
        }

        public void Delete(int id)
        {
            var room = _rooms.SingleOrDefault(r => r.Id == id);
            if (room == null)
            {
                return;
            }
            room.Status = true;
            Commit();
        }

        public Room? GetRoomById(int id)
        {
            return _rooms.Include("Type").SingleOrDefault(r => r.Id == id);
        }

        List<Room> IRoomRepository.GetAll()
        {
            return _rooms.Include("Type").ToList();
        }

        Room IRoomRepository.Update(Room room)
        {
            var existed = _rooms.FirstOrDefault(r => r.Id == room.Id);
            if (existed != null)
            {
                room.TypeId = existed.TypeId;
                _context.Entry(existed).State = EntityState.Detached;
            }
            _context.Attach(room);
            _context.Entry(room).State = EntityState.Modified;
            _context.SaveChanges();
            return room;
        }

        private int GetLatestId()
        {
            return GetAll()
                .OrderByDescending(p => p.Id).FirstOrDefault()
                ?.Id ?? 0;
        }
    }
}
