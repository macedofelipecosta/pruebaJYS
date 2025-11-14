using LogicaNegocio.InterfacesDominio;

namespace LogicaNegocio.Dominio.Rooms
{
    public class RoomStatus : IValidable
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;

        public bool IsReservable { get; private set; }

        public RoomStatus(int id, string name, bool isReservable)
        {
            Id = id;
            Name = name;
            IsReservable = isReservable;
        }

        private RoomStatus() { }

        public void Validate()
        {
            if (Name.Length > 50)
            {
                throw new InvalidOperationException("El nombre del estado de la sala no puede exceder los 50 caracteres.");
            }
            if (Name.Length == 0)
            {
                throw new InvalidOperationException("El nombre del estado de la sala no puede estar vacío.");
            }
        }
    }
}
