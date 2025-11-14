using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Cache
{
    public class RedisNotificationQueue : INotificationQueue
    {
        private readonly IConnectionMultiplexer _redis;
        private const string QueueKey = "notifications-queue";

        public RedisNotificationQueue(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task EnqueueAsync(string message, CancellationToken ct = default)
        {
            var db = _redis.GetDatabase();
            await db.ListLeftPushAsync(QueueKey, message);
        }

        public async Task<string?> DequeueAsync(CancellationToken ct = default)
        {
            var db = _redis.GetDatabase();
            var result = await db.ListRightPopAsync(QueueKey);
            return result.HasValue ? result.ToString() : null;
        }
    }
}
