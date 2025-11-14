using LogicaNegocio.ValueObjects.LogicaNegocio.ValueObjects;

namespace LogicaNegocio.Dominio
{
    public class User
    {
        public int Id { get; private set; }
        public string Username { get; private set; } = string.Empty;
        public Email Email { get; private set; } = null!; // TODO Definir formato a validar
        public string Role { get; private set; } = string.Empty; //TODO Ver roles que hay creados en Active Directory
        public DateTime LastLogin { get; private set; }

        private User()
        {
        }
    }

}