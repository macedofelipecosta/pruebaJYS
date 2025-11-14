using LogicaNegocio.InterfacesDominio;

namespace LogicaNegocio.Dominio.Rooms
{
    public class Room : IValidable
    {
        public int Id { get; private set; }
        public int RoomNumber { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public int MaxCapacity { get; private set; }
        public int MinCapacity { get; private set; }
        public int RoomStatusId { get; private set; }
        public RoomStatus Status { get; private set; } = null!; // Available, Reserved, OutOfService
        public int LocationId { get; private set; }
        public Location Location { get; private set; } = null!;

        private Room()
        {
        }
        public void SetStatus(RoomStatus status)
        {
            if (status == null)
                throw new ArgumentNullException(nameof(status));

            RoomStatusId = status.Id;
            Status = status;
        }

        public void SetLocation(Location location)
        {
            if (location == null)
                throw new ArgumentNullException(nameof(location));

            LocationId = location.Id;
            Location = location;
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new InvalidOperationException("El nombre de la sala no puede estar vacío.");
            }
            if (Name.Length > 100)
            {
                throw new InvalidOperationException("El nombre de la sala no puede exceder los 100 caracteres.");
            }
            if (MinCapacity <= 0)
            {
                throw new InvalidOperationException("La capacidad mínima debe ser mayor que cero.");
            }
            if (MaxCapacity <= 0)
            {
                throw new InvalidOperationException("La capacidad máxima debe ser mayor que cero.");
            }
            if (MinCapacity > MaxCapacity)
            {
                throw new InvalidOperationException("La capacidad mínima no puede ser mayor que la capacidad máxima.");
            }
            if (Status != null && !Status.Name.ToLower().Equals("available") && !Status.Name.ToLower().Equals("reserved") && !Status.Name.ToLower().Equals("outofservice"))
            {
                throw new InvalidOperationException("El estado de la sala es inválido.");
            }
            if (Location == null)
            {
                throw new InvalidOperationException("La sede no puede ser nula.");
            }
            if (LocationId == 0)
            {
                throw new InvalidOperationException("El Id de la sede no puede ser 0.");
            }
            if (Location != null && LocationId != Location.Id)
            {
                throw new InvalidOperationException("El Id de la sede no coincide con el Id de la sede asociada.");
            }
        }
    }
}
