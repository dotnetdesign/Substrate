using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DotNetDesign.Substrate.Tests
{
    public interface IPerson : IEntity<IPerson, IPersonData, IPersonRepository>, IPersonData
    {
        
    }

    public interface IPersonData : IEntityData<IPersonData, IPerson, IPersonRepository>
    {
        [DisplayName("First Name")]
        [Required]
        string FirstName { get; set; }
        string LastName { get; set; }
    }

    public interface IPersonRepository: IEntityRepository<IPersonRepository, IPerson, IPersonData>
    {
        
    }

    public interface IPersonRepositoryService:IEntityRepositoryService<IPersonData, IPerson, IPersonRepository, PersonData>
    {}

    public class Person : BaseEntity<IPerson, IPersonData, IPersonRepository>, IPerson
    {
        public Person(
            Func<IPersonRepository> entityRepositoryFactory, 
            Func<IPersonData> entityDataFactory, 
            Func<IConcurrencyManager<IPerson, IPersonData, IPersonRepository>> entityConcurrencyManager,
            IEnumerable<IEntityValidator<IPerson, IPersonData, IPersonRepository>> entityValidators,
            Func<IPermissionAuthorizationManager<IPerson, IPersonData, IPersonRepository>> permissionAuthorizationManagerFactory)
            : base(entityRepositoryFactory, entityDataFactory, entityConcurrencyManager, entityValidators, permissionAuthorizationManagerFactory)
        {
        }

        public string FirstName
        {
            get
            {
                return (EntityData == null) ? default(string) : EntityData.FirstName;
            }
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
            get
            {
                return (EntityData == null) ? default(string) : EntityData.LastName;
            }
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
    public class PersonData : BaseEntityData<IPersonData, IPerson, IPersonRepository>, IPersonData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class PersonRepository : EntityRepository<IPerson, IPersonData, PersonData, IPersonRepository, IPersonRepositoryService>, IPersonRepository
    {
        public PersonRepository(
            Func<IPerson> entityFactory, 
            Func<IPersonData> entityDataFactory, 
            Func<IPersonRepositoryService> entityRepositoryServiceFactory, 
            IEnumerable<IEntityObserver<IPerson>> entityObservers,
            Func<IScopeManager> scopeManagerFactory)
            : base(entityFactory, entityDataFactory, entityRepositoryServiceFactory, entityObservers, () => new DictionaryEntityCache<IPerson, IPersonData, IPersonRepository>(), scopeManagerFactory)
        {
        }
    }

    public class PersonRepositoryService : IPersonRepositoryService
    {
        public PersonData GetById(Guid id, Dictionary<string, string> scopeContext)
        {
            throw new NotImplementedException();
        }

        public PersonData GetVersion(Guid id, int version, Dictionary<string, string> scopeContext)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PersonData> GetByIds(IEnumerable<Guid> ids, Dictionary<string, string> scopeContext)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PersonData> GetAll(Dictionary<string, string> scopeContext)
        {
            throw new NotImplementedException();
        }

        public PersonData Save(PersonData entityData, Dictionary<string, string> scopeContext)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PersonData> SaveAll(IEnumerable<PersonData> entityData, Dictionary<string, string> scopeContext)
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