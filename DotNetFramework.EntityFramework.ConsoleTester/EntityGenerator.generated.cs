using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.ServiceModel;
using DotNetDesign.EntityFramework;

namespace DotNetFramework.EntityFramework.ConsoleTester
{
	#region Entity Data Interfaces
	public partial interface IPersonData : IEntityData<IPersonData, IPerson, IPersonRepository>
	{
		[Required]
		[DisplayName("First Name")]
		string FirstName { get; set; }
		[Required]
		[DisplayName("Last Name")]
		string LastName { get; set; }
		[Required]
		[DisplayName("Age")]
		uint Age { get; set; }
	}
	#endregion Entity Data Interfaces

	#region Entity Interfaces
	public partial interface IPerson : IEntity<IPerson, IPersonData, IPersonRepository>, IPersonData {}
	#endregion Entity Interfaces

	#region Entity Repository Interfaces
	public partial interface IPersonRepository : IEntityRepository<IPersonRepository, IPerson, IPersonData> {}
	#endregion Entity Repository Interfaces

	#region Entity Repository Service Interfaces
	[ServiceContract]
	public partial interface IPersonRepositoryService : IEntityRepositoryService<IPersonData, IPerson, IPersonRepository, PersonData> {}
	#endregion Entity Repository Service Interfaces

	#region Entity Data Implementations
	[DataContract]
	[Serializable]
	public partial class PersonData : BaseEntityData<IPersonData, IPerson, IPersonRepository>, IPersonData
	{
		#region IPersonData Properties
		[DataMember]
		public virtual string FirstName { get; set; }
		[DataMember]
		public virtual string LastName { get; set; }
		[DataMember]
		public virtual uint Age { get; set; }
		#endregion IPersonData Properties
	}
	#endregion Entity Data Implementations

	#region Entity Implementations
	public partial class Person : BaseEntity<IPerson, IPersonData, IPersonRepository>, IPerson
	{
		public Person(
			Func<IPersonRepository> entityRepositoryFactory,
			Func<IPersonData> entityDataFactory,
			Func<IConcurrencyManager<IPerson, IPersonData, IPersonRepository>> entityConcurrencyManagerFactory,
			IEnumerable<IEntityValidator<IPerson, IPersonData, IPersonRepository>> entityValidators)
			: base(entityRepositoryFactory, entityDataFactory, entityConcurrencyManagerFactory, entityValidators)
		{
		}

		#region IPersonData Properties
		public virtual string FirstName 
		{
			get { return EntityData.FirstName; }
			set
			{
				if (EntityData.FirstName == value) return;

				var originalValue = EntityData.FirstName;
				OnPropertyChanging("FirstName", originalValue, value);
				EntityData.FirstName = value;
				OnPropertyChanged("FirstName", originalValue, value);
			}
		}
		public virtual string LastName 
		{
			get { return EntityData.LastName; }
			set
			{
				if (EntityData.LastName == value) return;

				var originalValue = EntityData.LastName;
				OnPropertyChanging("LastName", originalValue, value);
				EntityData.LastName = value;
				OnPropertyChanged("LastName", originalValue, value);
			}
		}
		public virtual uint Age 
		{
			get { return EntityData.Age; }
			set
			{
				if (EntityData.Age == value) return;

				var originalValue = EntityData.Age;
				OnPropertyChanging("Age", originalValue, value);
				EntityData.Age = value;
				OnPropertyChanged("Age", originalValue, value);
			}
		}
		#endregion IPersonData Properties
	}
	#endregion Entity Implementations

	#region Entity Repository Implementations
	public partial class PersonRepository : EntityRepository<IPerson, IPersonData, PersonData, IPersonRepository, IPersonRepositoryService>, IPersonRepository
	{
		public PersonRepository(
			Func<IPerson> entityFactory,
			Func<IPersonData> entityDataFactory,
			Func<IPersonRepositoryService> entityRepositoryServiceFactory,
			IEnumerable<IEntityObserver<IPerson>> entityObservers,
			IEntityCache<IPerson, IPersonData, IPersonRepository> entityCache)
			: base(entityFactory, entityDataFactory, entityRepositoryServiceFactory, entityObservers, entityCache)
		{
		}
	}
	#endregion Entity Repository Implementations
}