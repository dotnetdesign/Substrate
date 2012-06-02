using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DotNetDesign.Substrate.Tests
{
    public interface IOverwriteConcurrency : IEntity<IOverwriteConcurrency, IOverwriteConcurrencyData, IOverwriteConcurrencyRepository>, IOverwriteConcurrencyData
    {
        
    }

    [ConcurrencyMode(ConcurrencyMode.Overwrite)]
    public interface IOverwriteConcurrencyData : IEntityData<IOverwriteConcurrencyData, IOverwriteConcurrency, IOverwriteConcurrencyRepository>
    {
        [DisplayName("First Name")]
        [Required]
        string FirstName { get; set; }
        string LastName { get; set; }
    }

    public interface IOverwriteConcurrencyRepository: IEntityRepository<IOverwriteConcurrencyRepository, IOverwriteConcurrency, IOverwriteConcurrencyData>
    {
        
    }

    public interface IOverwriteConcurrencyRepositoryService:IEntityRepositoryService<IOverwriteConcurrencyData, IOverwriteConcurrency, IOverwriteConcurrencyRepository, OverwriteConcurrencyData>
    {}

    public class OverwriteConcurrency : BaseEntity<IOverwriteConcurrency, IOverwriteConcurrencyData, IOverwriteConcurrencyRepository>, IOverwriteConcurrency
    {
        public OverwriteConcurrency(
            Func<IOverwriteConcurrencyRepository> entityRepositoryFactory, 
            Func<IOverwriteConcurrencyData> entityDataFactory,
            Func<IConcurrencyManager<IOverwriteConcurrency, IOverwriteConcurrencyData, IOverwriteConcurrencyRepository>> entityConcurrencyManager,
            IEnumerable<IEntityValidator<IOverwriteConcurrency, IOverwriteConcurrencyData, IOverwriteConcurrencyRepository>> entityValidators,
            Func<IPermissionAuthorizationManager<IOverwriteConcurrency, IOverwriteConcurrencyData, IOverwriteConcurrencyRepository>> permissionAuthorizationManagerFactory)
            : base(entityRepositoryFactory, entityDataFactory, entityConcurrencyManager, entityValidators, permissionAuthorizationManagerFactory)
        {
        }

        public string FirstName
        {
            get { return (EntityData == null) ? default(string) : EntityData.FirstName; }
            set
            {
                if (EntityData.FirstName.Equals(value)) return;

                var oldValue = EntityData.FirstName;
                OnPropertyChanging("FirstName", oldValue, value);
                EntityData.FirstName = value;
                OnPropertyChanged("FirstName", oldValue, value);
            }
        }

        public string LastName
        {
            get { return (EntityData == null) ? default(string) : EntityData.LastName; }
            set
            {
                if (EntityData.LastName.Equals(value)) return;

                var oldValue = EntityData.LastName;
                OnPropertyChanging("LastName", oldValue, value);
                EntityData.LastName = value;
                OnPropertyChanged("LastName", oldValue, value);
            }
        }
    }

    [Serializable]
    public class OverwriteConcurrencyData : BaseEntityData<IOverwriteConcurrencyData, IOverwriteConcurrency, IOverwriteConcurrencyRepository>, IOverwriteConcurrencyData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class OverwriteConcurrencyRepository : EntityRepository<IOverwriteConcurrency, IOverwriteConcurrencyData, OverwriteConcurrencyData, IOverwriteConcurrencyRepository, IOverwriteConcurrencyRepositoryService>, IOverwriteConcurrencyRepository
    {
        public OverwriteConcurrencyRepository(
            Func<IOverwriteConcurrency> entityFactory, 
            Func<IOverwriteConcurrencyData> entityDataFactory, 
            Func<IOverwriteConcurrencyRepositoryService> entityRepositoryServiceFactory, 
            IEnumerable<IEntityObserver<IOverwriteConcurrency>> entityObservers,
            Func<IScopeManager> scopeManagerFactory)
            : base(entityFactory, entityDataFactory, entityRepositoryServiceFactory, entityObservers, () => new DictionaryEntityCache<IOverwriteConcurrency, IOverwriteConcurrencyData, IOverwriteConcurrencyRepository>(), scopeManagerFactory)
        {
        }
    }

    public class OverwriteConcurrencyRepositoryService : IOverwriteConcurrencyRepositoryService
    {
        public OverwriteConcurrencyData GetById(Guid id, Dictionary<string, string> scopeContext)
        {
            throw new NotImplementedException();
        }

        public OverwriteConcurrencyData GetVersion(Guid id, int version, Dictionary<string, string> scopeContext)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OverwriteConcurrencyData> GetByIds(IEnumerable<Guid> ids, Dictionary<string, string> scopeContext)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OverwriteConcurrencyData> GetAll(Dictionary<string, string> scopeContext)
        {
            throw new NotImplementedException();
        }

        public OverwriteConcurrencyData Save(OverwriteConcurrencyData entityData, Dictionary<string, string> scopeContext)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OverwriteConcurrencyData> SaveAll(IEnumerable<OverwriteConcurrencyData> entityData, Dictionary<string, string> scopeContext)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid id, Dictionary<string, string> scopeContext)
        {
            throw new NotImplementedException();
        }

        public void DeleteAll(IEnumerable<Guid> ids, Dictionary<string, string> scopeContext)
        {
            throw new NotImplementedException();
        }
    }
}