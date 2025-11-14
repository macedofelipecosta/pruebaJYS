using Infrastructure.Persistence.EntityFramework;
using LogicaNegocio.Dominio;
using LogicaNegocio.InterfacesRepositorios;

namespace Infrastructure.Persistence.EntityFramework.Repositorios
{
    public class LocationRepository : ILocationRepository
    {
        private readonly GestorSalasContext _context;

        public LocationRepository(GestorSalasContext context)
        {
            _context = context;
        }

        public void Add(Location location)
        {
            location.Validate();
            _context.Locations.Add(location);
            _context.SaveChanges();
        }

        public IEnumerable<Location> GetAll()
        {
            return _context.Locations.ToList();
        }

        public Location GetById(int id)
        {
            return _context.Locations.FirstOrDefault(l => l.Id == id);
        }
        public void Update(Location obj)
        {
            obj.Validate();
            _context.Locations.Update(obj);
            _context.SaveChanges();
        }

        public void Remove(Location location)
        {
            _context.Locations.Remove(location);
            _context.SaveChanges();
        }
    }
}
