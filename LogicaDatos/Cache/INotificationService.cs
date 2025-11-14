using LogicaNegocio.Dominio.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Cache
{
    public interface INotificationService
    {
        Task NotifyReservationCreatedAsync(ReservationCreatedDomainEvent evt, CancellationToken ct);
    }
}

