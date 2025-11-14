using LogicaNegocio.Dominio.Reservations;

namespace LogicaNegocio.InterfacesRepositorios
{
    public interface IReservationRepository : IRepository<Reservation>
    {
        Task AddAsync(Reservation entity, CancellationToken cancellationToken);
    }
}
