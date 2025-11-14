using LogicaNegocio.InterfacesDominio;

namespace LogicaNegocio.Dominio.Rooms
{
    public class Equipment : IValidable
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; } = string.Empty;

        private Equipment() { }

        public void Validate()
        {
            if (!string.IsNullOrWhiteSpace(Name)) throw new ArgumentException("El nombre del equipamiento no puede estar vacío.");
            if (Name.Length > 50) throw new ArgumentException("El nombre del equipamiento no puede exceder los 50 caracteres.");
            if (Description != null && Description.Length > 250) throw new ArgumentException("La descripción del equipamiento no puede exceder los 250 caracteres.");
        }
    }
}