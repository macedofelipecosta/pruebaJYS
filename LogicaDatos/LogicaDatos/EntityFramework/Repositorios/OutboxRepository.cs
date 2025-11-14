using Infrastructure.Persistence.EntityFramework;
using LogicaNegocio.Dominio.Notifications;
using LogicaNegocio.InterfacesRepositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.LogicaDatos.EntityFramework.Repositorios
{
    public class OutboxRepository:IOutboxRepository
    {
        private readonly GestorSalasContext _context;

        public OutboxRepository(GestorSalasContext context)
        {
            _context = context;
        }

        public async Task AddAsync(OutboxMessage message, CancellationToken ct = default)
        {
            await _context.OutboxMessages.AddAsync(message, ct);
        }
    }
}
