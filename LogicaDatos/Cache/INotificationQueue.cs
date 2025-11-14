using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Cache
{
    public interface INotificationQueue
    {
        Task EnqueueAsync(string message, CancellationToken ct = default);
        Task<string?> DequeueAsync(CancellationToken ct = default); // útil para el Worker
    }
}

