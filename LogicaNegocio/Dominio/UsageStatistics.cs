using LogicaNegocio.Dominio.Rooms;

namespace LogicaNegocio.Dominio
{
    public class UsageStatistics
    {
        public int Id { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public int RoomId { get; private set; }
        public Room Room { get; private set; } = null!;
        public int TotalReservations { get; private set; }
        public int TotalCancellations { get; private set; }
        public double AverageReservationDuration { get; private set; } // Doble para representar horas con decimales
        public int ExtendedReservations { get; private set; }
        public ICollection<User> UsersWithMostUsage { get; private set; } = new List<User>();

        private UsageStatistics()
        {
        }
    }
}
