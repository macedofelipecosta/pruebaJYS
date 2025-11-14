namespace LogicaNegocio.Dominio.Rooms
{
    public class RoomEquipment
    {
        public int Id { get; private set; }
        public int EquipmentId { get; private set; }
        public Equipment Equipment { get; private set; } = null!;
        public int RoomId { get; private set; }
        public Room Room { get; private set; } = null!;
        public int Quantity { get; private set; }

        private RoomEquipment()
        {
        }
    }
}
