namespace LogicaNegocio.Dominio.Reservations;
public class ReservationStatus
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    public ReservationStatus(int id, string name)
    {
        Id = id;
        Name = name;
    }

    private ReservationStatus() { }
}
