using DTOs;

namespace LogicaAplicacion.ServiceInterfaces
{
    public interface IReservationService : IService<ReservationDTO>
    {
        public Task<ReservationDTO> CreateAsync(ReservationDTO dto, CancellationToken cancellationToken);
    }
}
