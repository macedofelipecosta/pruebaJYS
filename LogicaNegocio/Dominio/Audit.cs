using LogicaNegocio.Dominio.Enums;

namespace LogicaNegocio.Dominio
{
    public class Audit
    {
        public int Id { get; private set; }
        public DateTime Date { get; private set; }
        public int UserId { get; private set; }
        public int EntityId { get; private set; }
        public string IpAdress { get; private set; } = string.Empty;
        public AuditAction Action { get; private set; } // 1-Create, 2-Update, 3-Delete

        private Audit()
        {
            //Para EF
        }
    }
}
