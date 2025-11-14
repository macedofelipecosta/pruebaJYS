using LogicaNegocio.Dominio.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LogicaNegocio.InterfacesRepositorios
{
    public interface IOutboxRepository
    {
        Task AddAsync(OutboxMessage message, CancellationToken ct = default);
    }
}
