using System;

namespace DotNetDesign.Substrate
{
    /// <summary>
    /// Defines methods for observing observable entities.
    /// </summary>
    /// <typeparam name="TObservableEntity">The type of the observable entity.</typeparam>
    public interface IEntityObserver<in TObservableEntity> : IEntityObserver<TObservableEntity, Guid>
        where TObservableEntity : class, IObservableEntity
    {}


    /// <summary>
    /// Defines methods for observing observable entities.
    /// </summary>
    /// <typeparam name="TObservableEntity">The type of the observable entity.</typeparam>
    /// <typeparam name="TId">The type of the id.</typeparam>
    public interface IEntityObserver<in TObservableEntity, TId>
        where TObservableEntity : class, IObservableEntity<TId>
    {
        /// <summary>
        /// Attaches this entity observer to the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Attach(TObservableEntity entity);

        /// <summary>
        /// Detaches this entity observer from the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Detach(TObservableEntity entity);
    }
}