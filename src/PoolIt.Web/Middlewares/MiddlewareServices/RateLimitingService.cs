namespace PoolIt.Web.Middlewares.MiddlewareServices
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Contracts;

    public class RateLimitingService : IRateLimitingService
    {
        private const int AllowedRequestsInTimeInterval = 50;
        private const int TimeIntervalInMinutes = 1;

        private readonly ConcurrentDictionary<string, ClientInfo> clients;

        public RateLimitingService()
        {
            this.clients = new ConcurrentDictionary<string, ClientInfo>();
        }

        public bool RegisterClientRequest(string clientIp)
        {
            var client = this.clients.GetOrAdd(clientIp, s => new ClientInfo());

            if (client.IsLocked)
            {
                return true;
            }

            lock (client.LockObj)
            {
                var requestsQueue = client.Requests;

                requestsQueue.Enqueue(DateTime.UtcNow.AddMinutes(TimeIntervalInMinutes));
                RemoveExpired(requestsQueue);

                if (requestsQueue.Count > AllowedRequestsInTimeInterval)
                {
                    client.IsLocked = true;
                }
            }

            return client.IsLocked;
        }

        public bool IsClientLocked(string clientIp)
        {
            var clientExists = this.clients.TryGetValue(clientIp, out var client);

            if (!clientExists)
            {
                return false;
            }

            return client.IsLocked;
        }

        public void UnlockClient(string clientIp)
        {
            var clientExists = this.clients.TryGetValue(clientIp, out var client);

            if (!clientExists)
            {
                return;
            }

            lock (client.LockObj)
            {
                client.Requests.Clear();
                client.IsLocked = false;
            }
        }

        private static void RemoveExpired(Queue<DateTime> queue)
        {
            var currTime = DateTime.UtcNow;

            while (queue.Count > 0 && queue.Peek() <= currTime)
            {
                queue.Dequeue();
            }
        }

        private class ClientInfo
        {
            public ClientInfo()
            {
                this.Requests = new Queue<DateTime>();
                this.IsLocked = false;
                this.LockObj = new object();
            }

            public Queue<DateTime> Requests { get; }
            public bool IsLocked { get; set; }
            public object LockObj { get; }
        }
    }
}