using Infrastructure.Persistence.EntityFramework;
using LogicaNegocio.Dominio.Notifications;
using LogicaNegocio.InterfacesRepositorios;
using Microsoft.EntityFrameworkCore;
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

        public Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
        {
            return _context.OutboxMessages.AddAsync(message, cancellationToken).AsTask();
        }

        public Task<List<OutboxMessage>> GetUnprocessedAsync(int maxCount, CancellationToken cancellationToken)
        {
            return _context.OutboxMessages
                .Where(x => !x.ProcessedOn.HasValue)
                .OrderBy(x => x.OccurredOn)
                .Take(maxCount)
                .ToListAsync(cancellationToken);
        }
    }
}
