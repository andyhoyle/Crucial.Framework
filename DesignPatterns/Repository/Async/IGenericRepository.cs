using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crucial.Framework.DesignPatterns.Repository.Async
{
    public interface IGenericRepository<TEntity> : Crucial.Framework.DesignPatterns.Repository.Async.IReadOnlyRepository<TEntity>,
        IUpdateRepositoryAsync<TEntity>,
        ICreateRepositoryAsync<TEntity, TEntity>,
        IDeleteRepositoryAsync<TEntity>
        where TEntity : BaseEntities.ProviderEntityBase
    {

    }
}
