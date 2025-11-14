using Infrastructure.Persistence.EntityFramework;
using LogicaNegocio.Dominio.Rooms;
using LogicaNegocio.InterfacesRepositorios;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.EntityFramework.Repositorios
{
    public class RoomRepository : IRoomRepository
    {

        private GestorSalasContext _context;
        public RoomRepository(GestorSalasContext context)
        {
            _context = context;
        }
        public void Add(Room room)
        {

            var status = _context.RoomStatus.FirstOrDefault(r => r.Id == room.RoomStatusId)
                ?? throw new InvalidOperationException($"No existe RoomStatus con Id {room.RoomStatusId}");

            var location = _context.Locations.FirstOrDefault(l => l.Id == room.LocationId)
                ?? throw new InvalidOperationException($"No existe Location con Id {room.LocationId}");

            room.SetStatus(status);
            room.SetLocation(location);

            room.Validate();
            _context.Rooms.Add(room);
            _context.SaveChanges();

            _context.Entry(room).Reference(r => r.Status).Load();
            _context.Entry(room).Reference(r => r.Location).Load();
        }

        public Room GetById(int id)
        {
            return _context.Rooms.Include(r => r.Location)
                                 .Include(r => r.Status)
                                 .FirstOrDefault(r => r.Id == id);
        }

        public void Update(Room obj)
        {
            var existingRoom = _context.Rooms
                .Include(r => r.Status)
                .Include(r => r.Location)
                .FirstOrDefault(r => r.Id == obj.Id)
                ?? throw new InvalidOperationException($"No existe Room con Id {obj.Id}");
            var status = _context.RoomStatus.FirstOrDefault(r => r.Id == obj.RoomStatusId)
                ?? throw new InvalidOperationException($"No existe RoomStatus con Id {obj.RoomStatusId}");
            var location = _context.Locations.FirstOrDefault(l => l.Id == obj.LocationId)
                ?? throw new InvalidOperationException($"No existe Location con Id {obj.LocationId}");
            existingRoom.SetStatus(status);
            existingRoom.SetLocation(location);
            existingRoom.Validate();
            _context.Entry(existingRoom).CurrentValues.SetValues(obj);
            _context.SaveChanges();
        }

        public IEnumerable<Room> GetAll()
        {
            return _context.Rooms
                    .Include(r => r.Location)
                    .Include(r => r.Status)
                    .ToList();
        }

        public void Remove(Room room)
        {
            _context.Rooms.Remove(room);
            _context.SaveChanges();
        }
    }
}
