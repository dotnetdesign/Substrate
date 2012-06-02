using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DotNetDesign.Substrate.Tests
{
    public interface IFailConcurrency : IEntity<IFailConcurrency, IFailConcurrencyData, IFailConcurrencyRepository>, IFailConcurrencyData
    {
        
    }

    [ConcurrencyMode(ConcurrencyMode.Fail)]
    public interface IFailConcurrencyData : IEntityData<IFailConcurrencyData, IFailConcurrency, IFailConcurrencyRepository>
    {
        [DisplayName("First Name")]
        [Required]
        string FirstName { get; set; }
        string LastName { get; set; }
    }

    public interface IFailConcurrencyRepository: IEntityRepository<IFailConcurrencyRepository, IFailConcurrency, IFailConcurrencyData>
    {
        
    }

    public interface IFailConcurrencyRepositoryService:IEntityRepositoryService<IFailConcurrencyData, IFailConcurrency, IFailConcurrencyRepository, FailConcurrencyData>
    {}

    public class FailConcurrency : BaseEntity<IFailConcurrency, IFailConcurrencyData, IFailConcurrencyRepository>, IFailConcurrency
    {
        public FailConcurrency(
            Func<IFailConcurrencyRepository> entityRepositoryFactory, 
            Func<IFailConcurrencyData> entityDataFactory,
            Func<IConcurrencyManager<IFailConcurrency, IFailConcurrencyData, IFailConcurrencyRepository>> entityConcurrencyManager,
            IEnumerable<IEntityValidator<IFailConcurrency, IFailConcurrencyData, IFailConcurrencyRepository>> entityValidators,
            Func<IPermissionAuthorizationManager<IFailConcurrency, IFailConcurrencyData, IFailConcurrencyRepository>> permissionAuthorizationManagerFactory)
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
    public class FailConcurrencyData : BaseEntityData<IFailConcurrencyData, IFailConcurrency, IFailConcurrencyRepository>, IFailConcurrencyData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class FailConcurrencyRepository : EntityRepository<IFailConcurrency, IFailConcurrencyData, FailConcurrencyData, IFailConcurrencyRepository, IFailConcurrencyRepositoryService>, IFailConcurrencyRepository
    {
        public FailConcurrencyRepository(
            Func<IFailConcurrency> entityFactory, 
            Func<IFailConcurrencyData> entityDataFactory, 
            Func<IFailConcurrencyRepositoryService> entityRepositoryServiceFactory, 
            IEnumerable<IEntityObserver<IFailConcurrency>> entityObservers,
            Func<IScopeManager> scopeManagerFactory)
            : base(entityFactory, entityDataFactory, entityRepositoryServiceFactory, entityObservers, () => new DictionaryEntityCache<IFailConcurrency, IFailConcurrencyData, IFailConcurrencyRepository>(), scopeManagerFactory)
        {
        }
    }

    public class FailConcurrencyRepositoryService : IFailConcurrencyRepositoryService
    {
        public FailConcurrencyData GetById(Guid id, Dictionary<string, string> scopeContext)
        {
            throw new NotImplementedException();
        }

        public FailConcurrencyData GetVersion(Guid id, int version, Dictionary<string, string> scopeContext)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FailConcurrencyData> GetByIds(IEnumerable<Guid> ids, Dictionary<string, string> scopeContext)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FailConcurrencyData> GetAll(Dictionary<string, string> scopeContext)
        {
            throw new NotImplementedException();
        }

        public FailConcurrencyData Save(FailConcurrencyData entityData, Dictionary<string, string> scopeContext)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FailConcurrencyData> SaveAll(IEnumerable<FailConcurrencyData> entityData, Dictionary<string, string> scopeContext)
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