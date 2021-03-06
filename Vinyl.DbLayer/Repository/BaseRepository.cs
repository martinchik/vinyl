﻿using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vinyl.DbLayer.Models;

namespace Vinyl.DbLayer.Repository
{
    public class BaseRepository : IDisposable
    {
        protected internal readonly VinylShopContext Context;
        protected internal readonly ILogger Logger;

        internal BaseRepository(VinylShopContext context, ILogger<VinylShopContext> logger)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Commit()
        {
            Context.SaveChanges(true);
        }

        public Task CommitAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Context.SaveChangesAsync(true, cancellationToken);
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}