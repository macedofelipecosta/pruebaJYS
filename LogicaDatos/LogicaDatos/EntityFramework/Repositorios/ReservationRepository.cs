using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Persistence.EntityFramework;
using LogicaNegocio.Dominio.Reservations;
using LogicaNegocio.InterfacesRepositorios;

namespace Infrastructure.Persistence.EntityFramework.Repositorios
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly GestorSalasContext _context;

        public ReservationRepository(GestorSalasContext context)
        {
            _context = context;
        }

        public void Add(Reservation obj)
        {
            obj.Validate();
            _context.Reservations.Add(obj);
            _context.SaveChanges();
        }

        public async Task AddAsync(Reservation entity, CancellationToken cancellationToken)
        {
            entity.Validate();
            await _context.Reservations.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public IEnumerable<Reservation> GetAll()
        {
            return _context.Reservations.ToList();
        }

        public Reservation GetById(int id)
        {
            return _context.Reservations.FirstOrDefault(r => r.Id == id);
        }
        public void Update(Reservation obj)
        {
            obj.Validate();
            _context.Reservations.Update(obj);
            _context.SaveChanges();
        }

        public void Remove(Reservation obj)
        {
            _context.Reservations.Remove(obj);
            _context.SaveChanges();
        }


    }
}
