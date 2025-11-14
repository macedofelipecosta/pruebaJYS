namespace LogicaNegocio.Dominio.Reservations
{
    public class ParticipantStatus
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;

        public ParticipantStatus(int id, string name)
        {
            Id = id;
            Name = name;
        }

        private ParticipantStatus() { }
    }
}
