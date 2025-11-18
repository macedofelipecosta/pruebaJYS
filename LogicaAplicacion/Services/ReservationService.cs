using AutoMapper;
using DTOs;
using LogicaAplicacion.ServiceInterfaces;
using LogicaNegocio.Dominio.Notifications;
using LogicaNegocio.Dominio.Reservations;
using LogicaNegocio.InterfacesRepositorios;
using System.ComponentModel.DataAnnotations.Schema;
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
        public ReservationDTO Create(ReservationDTO objDto)
        {
            throw new NotImplementedException();
        }

        public async Task<ReservationDTO> CreateAsync(ReservationDTO dto, CancellationToken cancellationToken)
        {
            //Mapear DTO -> entidad dominio
            var reservation = _mapper.Map<Reservation>(dto);
            foreach (var userId in dto.UsersToNotifyId)
            {
                reservation.AddParticipant(userId); // método de dominio que crea ReservationParticipant
            }
            //Validar dominio
            reservation.Validate();

            //Guardar reserva en repositorio
            await _repo.AddAsync(reservation, cancellationToken);

            //Armar el evento para la cola (outbox) con un objeto anonimo (lo que necesitará el sistema de notificación)
            var notificationEvent = new
            {
                ReservationId = reservation.Id,
                RoomId = reservation.RoomId,
                StartDate = reservation.StartDate,
                EndDate = reservation.EndDate,
                Subject = reservation.Subject,
                Description = reservation.Description,
                Duration = reservation.Duration,
                UsersToNotify = "macedofelipecosta@gmail.com" // aca paso solo un email pero hay que ver el dto para que llegue una lista con emails
                //UsersToNotify = reservation.UsersToNotifyId
            };

            var payloadJson = JsonSerializer.Serialize(notificationEvent);

            var outboxMessage = new OutboxMessage(
                type: "ReservationCreated",
                payload: payloadJson,
                occurredOn: DateTime.UtcNow
            );

            //Guardar en Outbox
            await _outboxRepository.AddAsync(outboxMessage, cancellationToken);

            //Confirmar todo en una sola transacción
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            //Devolver DTO (mapeando si querés incluir el Id generado)
            return _mapper.Map<ReservationDTO>(reservation);
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
