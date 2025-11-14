using LogicaNegocio.Dominio.Rooms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityFramework.Config
{
    public class RoomConfig : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.ToTable("Rooms");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.RoomNumber)
                .IsRequired();
            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(100); // Establecer la longitud máxima para el nombre
            builder.Property(r => r.MinCapacity)
                .IsRequired();
            builder.Property(r => r.MaxCapacity)
                .IsRequired();
            // Configurar otras propiedades y relaciones según sea necesario, de esta forma se mapea la entidad Room a la tabla Rooms en la base de datos, 
            // es mejor para tipo de datos simples o primitivos
        }
    }
}
