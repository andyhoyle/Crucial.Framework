using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;
using Crucial.Framework.Logging;

namespace Crucial.Framework.Data.EntityFramework.Async
{
    /// <summary>
    /// Represents a repository of items.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of item in the repository.</typeparam>
    public abstract class BaseRepository<TContext, TEntity, TKey> :
            Crucial.Framework.DesignPatterns.Repository.Async.IReadOnlyRepository<TEntity>
        where TContext : IDbContextAsync
        where TEntity : Crucial.Framework.BaseEntities.ProviderEntityBase
        where TKey : Crucial.Framework.BaseEntities.ProviderEntityBase
    {
        private ILogger _logger;
        private readonly IContextProvider<TContext> _contextProvider;

        protected BaseRepository(IContextProvider<TContext> contextProvider, ILogger logger)
        {
            _contextProvider = contextProvider;
            _logger = logger;
        }

        public async Task<IEnumerable<TEntity>> GetAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> queryShaper, CancellationToken cancellationToken)
        {
            //using (var context = _contextProvider.DbContext)
            //{
                var query = queryShaper(_contextProvider.DbContext.Set<TEntity>());
                return await query.ToListAsync(cancellationToken);   
            //}
        }

        ///params Expression<Func<TEntity, object>>[] include)
        ///{
        ///    IQueryable<TEntity> query = Context.Set<TEntity>();
        ///    foreach (var inc in include)
        ///        query = query.Include(inc);
        ///    return query.Where(predicate);

        public async Task<IEnumerable<TEntity>> GetAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> queryShaper, CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[] include)
        {
            //using (var context = _contextProvider.DbContext)
            //{
                var query = queryShaper(_contextProvider.DbContext.Set<TEntity>());
                include.Select(_ => query.Include(_));
                return await query.ToListAsync(cancellationToken);   
            //}
        }

        public async Task<TResult> GetAsync<TResult>(Func<IQueryable<TEntity>, TResult> queryShaper, CancellationToken cancellationToken)
        {
            var factory = Task<TResult>.Factory;

            //using (var context = _contextProvider.DbContext)
            //{
                return await factory.StartNew(() => queryShaper(_contextProvider.DbContext.Set<TEntity>()), cancellationToken);
            //}
        }

        public async Task<TKey> Create(TEntity entity)
        {
            //using (var context = _contextProvider.DbContext)
            //{
                var output = _contextProvider.DbContext.Set<TEntity>();
                TEntity result = output.Add(entity);

                try
                {
                    await _contextProvider.DbContext.SaveChangesAsync();
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                {
                    _logger.LogException(ex);
                    throw;
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (var er in ex.EntityValidationErrors)
                    {
                        sb.Append(er.ToString());
                    }

                    _logger.Fatal(sb.ToString());
                    _logger.LogException(ex);
                    throw;
                }

                return result as TKey;
            //}
        }

       

        public async virtual Task<bool> Delete(TKey entity)
        {
            //using (var context = _contextProvider.DbContext)
            //{
                _contextProvider.DbContext.Set<TKey>().Attach(entity);
                _contextProvider.DbContext.Entry(entity).State = System.Data.Entity.EntityState.Deleted;

                try
                {
                    await _contextProvider.DbContext.SaveChangesAsync();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    _logger.LogException(ex);
                    throw new Exception("EF Validation failed, see inner exception for details", ex);
                }

                return true;
            //}
        }

        public async Task<bool> Update(TEntity entity)
        {
            //using (var context = _contextProvider.DbContext)
            //{
                _contextProvider.DbContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;

                try
                {
                    await _contextProvider.DbContext.SaveChangesAsync();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    _logger.LogException(ex);

                    _contextProvider.DbContext.Entry(entity).State = System.Data.Entity.EntityState.Unchanged;
                    throw;
                }

                return true;
            //}
        }
    }
}
