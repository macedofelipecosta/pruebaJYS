using LogicaNegocio.InterfacesDominio;

namespace LogicaNegocio.Dominio
{
    public class Parameter : IValidable
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public int Value { get; private set; }
        public string Description { get; private set; } = string.Empty;

        public Parameter(int id, string name, int value, string description)
        {
            Id = id;
            Name = name;
            Value = value;
            Description = description;
        }

        private Parameter() { }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new InvalidOperationException("El nombre del parámetro no puede estar vacío.");
            }
            if (Name.Length > 100)
            {
                throw new InvalidOperationException("El nombre del parámetro no puede exceder los 100 caracteres.");
            }
            if (Value < 0)
            {
                throw new InvalidOperationException("El valor del parámetro no puede ser negativo.");
            }
            if (Description.Length > 250)
            {
                throw new InvalidOperationException("La descripción del parámetro no puede exceder los 250 caracteres.");
            }
        }
    }
}
