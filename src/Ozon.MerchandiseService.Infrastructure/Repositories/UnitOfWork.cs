using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Npgsql;
using Ozon.MerchandiseService.Domain.Seedwork;
using Ozon.MerchandiseService.Infrastructure.Repositories.Interfaces;
using OzonEdu.StockApi.Infrastructure.Repositories.Infrastructure.Exceptions;

namespace Ozon.MerchandiseService.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private NpgsqlTransaction _npgsqlTransaction;
        
        private readonly IDbConnectionFactory<NpgsqlConnection> _dbConnectionFactory = null;
        private readonly IPublisher _publisher;
        private readonly IChangeTracker _changeTracker;

        public UnitOfWork(
            IDbConnectionFactory<NpgsqlConnection> dbConnectionFactory,
            IPublisher publisher,
            IChangeTracker changeTracker)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _publisher = publisher;
            _changeTracker = changeTracker;
        }

        public async ValueTask StartTransaction(CancellationToken token)
        {
            if (_npgsqlTransaction is not null)
            {
                return;
            }
            var connection = await _dbConnectionFactory.CreateConnection(token);
            _npgsqlTransaction = await connection.BeginTransactionAsync(token);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            if (_npgsqlTransaction is null)
            {
                throw new NoActiveTransactionStartedException();
            }

            var domainEvents = new Queue<INotification>(
                _changeTracker.TrackedEntities
                    .Where(x=>x.DomainEvents != null)
                    .SelectMany(x =>
                    {
                        var events = x.DomainEvents.ToList();
                        x.ClearDomainEvents();
                        return events;
                    }));
            // Можно отправлять все и сразу через Task.WhenAll.
            while (domainEvents.TryDequeue(out var notification))
            {
                await _publisher.Publish(notification, cancellationToken);
            }

            await _npgsqlTransaction.CommitAsync(cancellationToken);
        }

        void IDisposable.Dispose()
        {
            _npgsqlTransaction?.Dispose();
            _dbConnectionFactory?.Dispose();
        }
    }
}