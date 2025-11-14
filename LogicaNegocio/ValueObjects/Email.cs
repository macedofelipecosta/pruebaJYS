namespace LogicaNegocio.ValueObjects
{
    using System;
    using System.Net.Mail;

    namespace LogicaNegocio.ValueObjects
    {
        public class Email
        {
            public string Value { get; }

            private Email(string value)
            {
                Value = value;
            }

            public static Email Create(string value)
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El correo electrónico no puede estar vacío.");

                value = value.Trim().ToLowerInvariant();

                if (!value.EndsWith("@dominiodejaumeysere.com")) //TODO Corregir Dominio
                    throw new ArgumentException("El dominio del correo electrónico es inválido. Debe pertenecer a 'dominiodejaumeysere.com'.");

                try
                {
                    var _ = new MailAddress(value);
                }
                catch
                {
                    throw new ArgumentException("El formato del correo electrónico no es válido.");
                }

                return new Email(value);
            }

            public override string ToString() => Value;
            public override bool Equals(object? obj) => obj is Email e && e.Value == Value;
            public override int GetHashCode() => Value.GetHashCode();
        }
    }
}
