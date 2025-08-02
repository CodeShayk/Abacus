using System.Linq.Expressions;

namespace Abacus.Core
{
    /// <summary>
    /// Generic data provider interface for CRUD operations.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IDataProvider<TEntity> where TEntity : class
    {
        /// <summary>
        /// Retrieves all entities matching the specified expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetAll(Expression<Func<TEntity, bool>> expression = null);

        /// <summary>
        /// Retrieves an entity by its Identifier.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TEntity> GetById(int id);

        /// <summary>
        /// Creates a new entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<TEntity> Create(TEntity entity);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<TEntity> Update(TEntity entity);

        /// <summary>
        /// Deletes an entity by its Identifier.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task Delete(int id);
    }
}