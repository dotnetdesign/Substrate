using System;
using System.Collections.Generic;

namespace DotNetDesign.EntityFramework.Tests
{
    public interface IPerson : IEntity<IPerson, IPersonData, IPersonRepository>, IPersonData
    {
        
    }

    public interface IPersonData : IEntityData<IPersonData, IPerson, IPersonRepository>
    {
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
        public Person(IPersonRepository entityRepository, Func<IPersonData> entityDataFactory, IEnumerable<IEntityValidator<IPerson, IPersonData, IPersonRepository>> entityValidators)
            : base(entityRepository, entityDataFactory, entityValidators)
        {
        }
        public Person(IPersonRepository entityRepository, Func<IPersonData> entityDataFactory)
            : base(entityRepository, entityDataFactory, new List<IEntityValidator<IPerson, IPersonData, IPersonRepository>>())
        {
        }

        public string FirstName
        {
            get { return (EntityData == null) ? default(string) : EntityData.FirstName; }
            set
            {
                if (EntityData.FirstName.Equals(value)) return;

                OnPropertyChanging("FirstName");
                EntityData.FirstName = value;
                OnPropertyChanged("FirstName");
            }
        }

        public string LastName
        {
            get { return (EntityData == null) ? default(string) : EntityData.LastName; }
            set
            {
                if (EntityData.LastName.Equals(value)) return;

                OnPropertyChanging("LastName");
                EntityData.LastName = value;
                OnPropertyChanged("LastName");
            }
        }
    }

    public class PersonData : BaseEntityData<IPersonData,IPerson,IPersonRepository>, IPersonData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class PersonRepository : EntityRepository<IPerson, IPersonData, PersonData, IPersonRepository, IPersonRepositoryService>, IPersonRepository
    {
        public PersonRepository(Func<IPerson> entityFactory, Func<IPersonData> entityDataFactory, IPersonRepositoryService entityRepositoryService, IEnumerable<IEntityObserver<IPerson>> entityObservers)
            : base(entityFactory, entityDataFactory, entityRepositoryService, entityObservers)
        {
        }
    }

    public class PersonRepositoryService : IPersonRepositoryService
    {
        public PersonData GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public PersonData GetVersion(PersonData entityData, int version)
        {
            throw new NotImplementedException();
        }

        public PersonData GetPreviousVersion(PersonData entityData)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PersonData> GetByIds(IEnumerable<Guid> ids)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PersonData> GetAll()
        {
            throw new NotImplementedException();
        }

        public PersonData Save(PersonData entityData)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PersonData> SaveAll(IEnumerable<PersonData> entityData)
        {
            throw new NotImplementedException();
        }

        public void Delete(PersonData entityData)
        {
            throw new NotImplementedException();
        }

        public void DeleteAll(IEnumerable<PersonData> entityData)
        {
            throw new NotImplementedException();
        }
    }
}