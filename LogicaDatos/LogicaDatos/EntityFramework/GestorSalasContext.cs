using Infrastructure.Cache;
using LogicaNegocio.Dominio;
using LogicaNegocio.Dominio.Notifications;
using LogicaNegocio.Dominio.Reservations;
using LogicaNegocio.Dominio.Rooms;
using LogicaNegocio.InterfacesDominio;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Infrastructure.Persistence.EntityFramework
{
    public class GestorSalasContext : DbContext
    {
        public DbSet<Parameter> Parameters { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Equipment> Equipaments { get; set; }
        public DbSet<RoomEquipment> RoomEquipment { get; set; }
        //public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationParticipant> ReservationParticipants { get; set; }
        public DbSet<RoomStatus> RoomStatus { get; set; }


        //Notifications / Outbox
        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<OutboxMessage> OutboxMessages => Set <OutboxMessage>();

        //Auditorias, Estadisticas de Uso, no se si es necesario, ya que se crean y leen pero no se modifican ni eliminan
        //Usuarios, no se si es necesario, ya que siempre trabajaremos con el Active Directory para esos datos

        public GestorSalasContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .OwnsOne(u => u.Email, e =>
                {
                    e.Property(em => em.Value)
                        .HasColumnName("Email");
                });

            modelBuilder.Entity<ReservationParticipant>()
                .OwnsOne(rp => rp.UserEmail, e =>
                {
                    e.Property(em => em.Value)
                        .HasColumnName("UserEmail");
                });

            modelBuilder.Entity<RoomStatus>().HasData(
                new RoomStatus(1, "Available", true),
                new RoomStatus(2, "Reserved", false),
                new RoomStatus(3, "OutOfService", false)
            );

            modelBuilder.Entity<ReservationStatus>().HasData(
                new ReservationStatus(1, "Pending"),
                new ReservationStatus(2, "Confirmed"),
                new ReservationStatus(3, "Cancelled")
            );

            modelBuilder.Entity<ParticipantStatus>().HasData(
                new ParticipantStatus(1, "Pending"),
                new ParticipantStatus(2, "Accepted"),
                new ParticipantStatus(3, "Declined")

            );

            modelBuilder.Entity<Parameter>().HasData(
                new Parameter(1, "TiempoEntreReservas", 15, "Tiempo mínimo entre reservas en minutos"),
                new Parameter(2, "TiempoMaximoExtension", 60, "Tiempo máximo permitido de extensión de una reserva en minutos"),
                new Parameter(3, "MaximoDiasAnticipacion", 30, "Días máximos de anticipación para reservar")
            );
            modelBuilder.Entity<OutboxMessage>(b =>
            {
                b.ToTable("OutboxMessages");
                b.HasKey(x => x.Id);

                b.Property(x => x.Type)
                    .IsRequired()
                    .HasMaxLength(200);

                b.Property(x => x.Payload)
                    .IsRequired();

                b.Property(x => x.OccurredOn)
                    .IsRequired();
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // 1) Buscar entidades con eventos de dominio
            var domainEntities = ChangeTracker
                .Entries<IHasDomainEvent>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            // 2) Extraer eventos
            var domainEvents = domainEntities
                .SelectMany(x => x.DomainEvents)
                .ToList();

            // 3) Limpiar eventos (ya los vamos a persistir como Outbox)
            domainEntities.ForEach(entity => entity.ClearDomainEvents());

            // 4) Crear un OutboxMessage por cada evento
            foreach (var domainEvent in domainEvents)
            {
                var type = domainEvent.GetType().Name;
                var payload = JsonSerializer.Serialize(domainEvent);

                var outbox = new OutboxMessage(
                    type: type,
                    payload: payload,
                    occurredOn: domainEvent.OccurredOn);

                await OutboxMessages.AddAsync(outbox, cancellationToken);
            }

            // 5) La misma transacción guarda entidades + Outbox
            return await base.SaveChangesAsync(cancellationToken);
        }


    }
}
