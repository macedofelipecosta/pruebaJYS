using LogicaNegocio.InterfacesDominio;

namespace LogicaNegocio.Dominio
{
    public class Location : IValidable
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;

        private Location() { }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name)) throw new ArgumentException("El nombre de la sede no puede estar vacío.");
            if (Name.Length > 100) throw new ArgumentException("El nombre de la sede no puede exceder los 100 caracteres.");
        }
    }
}
