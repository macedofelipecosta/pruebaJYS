using AutoMapper;
using DTOs;
using LogicaAplicacion.ServiceInterfaces;
using LogicaNegocio.Dominio.Notifications;
using LogicaNegocio.Dominio.Reservations;
using LogicaNegocio.InterfacesRepositorios;

using System.Text.Json;

namespace LogicaAplicacion.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutboxRepository _outboxRepository;
        private readonly IMapper _mapper;

        public ReservationService(
            IReservationRepository repo,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IOutboxRepository outboxRepository)
        {
            _repo = repo;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _outboxRepository = outboxRepository;
        }

        // Si querés dejar el Create sync, podés simplemente llamar al async:
        //public ReservationDTO Create(ReservationDTO objDto)
        //{
        //    // en ASP.NET lo ideal es solo async, pero te lo dejo simple
        //    return CreateAsync(objDto, CancellationToken.None)
        //        .GetAwaiter()
        //        .GetResult();
        //}

        public async Task<ReservationDTO> CreateAsync(
            ReservationDTO dto,
            CancellationToken cancellationToken)
        {
            // 1) Mapear y validar
            var reservation = _mapper.Map<Reservation>(dto);
            reservation.Validate();

            // 2) Agregar la reserva al repositorio (NO doble Add)
            await _repo.AddAsync(reservation, cancellationToken);

            // 3) Crear el domain event (ya lo tenés en tu dominio)
            var evt = new ReservationCreatedDomainEvent(
                reservation.Id,
                reservation.RoomId,
                reservation.CreatedByUserId,
                reservation.StartDate,
                reservation.EndDate,
                reservation.Subject,
                reservation.Description
            // agrega lo que pida tu constructor real
            );

            // 4) Crear un registro de Outbox
            var outboxMessage = new OutboxMessage
            {
                Type = nameof(ReservationCreatedDomainEvent),
                Payload = JsonSerializer.Serialize(evt),
                OccurredOn = DateTime.UtcNow
            };

            await _outboxRepository.AddAsync(outboxMessage, cancellationToken);

            // 5) Confirmar TODO junto (reserva + outbox) en una sola transacción
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 6) Devolver DTO
            dto.Id = reservation.Id;
            return dto;
        }

        public ReservationDTO FindById(int id)
        {
            var result = _repo.GetById(id);
            return _mapper.Map<ReservationDTO>(result);
        }

        public IEnumerable<ReservationDTO> List()
        {
            IEnumerable<Reservation> reservations = _repo.GetAll();
            return _mapper.Map<IEnumerable<ReservationDTO>>(reservations);
        }

        public ReservationDTO Update(ReservationDTO obj)
        {
            var r = _mapper.Map<Reservation>(obj);
            r.Validate();
            _repo.Update(r);
            return _mapper.Map<ReservationDTO>(r);
        }

        public void Delete(int id)
        {
            var reservation = _repo.GetById(id);
            _repo.Remove(reservation);
        }
    }
}
