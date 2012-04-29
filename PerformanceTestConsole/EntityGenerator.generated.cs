using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.ServiceModel;
using DotNetDesign.EntityFramework;

namespace PerformanceTestConsole
{
	#region Entity Data Interfaces
	public partial interface IPersonData : IEntityData<IPersonData, IPerson, IPersonRepository>
	{
		[Required]
		[DisplayName("Name")]
		string Name { get; set; }
		[Required]
		[DisplayName("Email Address")]
		string Email { get; set; }
	}
	#endregion Entity Data Interfaces

	#region Entity Interfaces
	public partial interface IPerson : IEntity<IPerson, IPersonData, IPersonRepository>, IPersonData {}
	#endregion Entity Interfaces

	#region Entity Repository Interfaces
	public partial interface IPersonRepository : IEntityRepository<IPersonRepository, IPerson, IPersonData> {}
	#endregion Entity Repository Interfaces

	#region Entity Repository Service Interfaces
	public partial interface IPersonRepositoryService : IEntityRepositoryService<IPersonData, IPerson, IPersonRepository, PersonData> {}
	#endregion Entity Repository Service Interfaces

	#region Entity Data Implementations
	[DataContract]
	[Serializable]
	public partial class PersonData : BaseEntityData<IPersonData, IPerson, IPersonRepository>, IPersonData
	{
		#region IPersonData Properties
		[DataMember]
		public virtual string Name { get; set; }
		[DataMember]
		public virtual string Email { get; set; }
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
			IEnumerable<IEntityValidator<IPerson, IPersonData, IPersonRepository>> entityValidators,
            Func<IPermissionAuthorizationManager<IPerson, IPersonData, IPersonRepository>> permissionAuthorizationManagerFactory)
			: base(entityRepositoryFactory, entityDataFactory, entityConcurrencyManagerFactory, entityValidators, permissionAuthorizationManagerFactory)
		{
		}

		#region IPersonData Properties
		public virtual string Name 
		{
			get { return EntityData.Name; }
			set
			{
				if (EntityData.Name == value) return;

				var originalValue = EntityData.Name;
				OnPropertyChanging("Name", originalValue, value);
				EntityData.Name = value;
				OnPropertyChanged("Name", originalValue, value);
			}
		}
		public virtual string Email 
		{
			get { return EntityData.Email; }
			set
			{
				if (EntityData.Email == value) return;

				var originalValue = EntityData.Email;
				OnPropertyChanging("Email", originalValue, value);
				EntityData.Email = value;
				OnPropertyChanged("Email", originalValue, value);
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
			Func<IEntityCache<IPerson, IPersonData, IPersonRepository>> entityCacheFactory,
			Func<IScopeManager> scopeManagerFactory)
			: base(entityFactory, entityDataFactory, entityRepositoryServiceFactory, entityObservers, entityCacheFactory, scopeManagerFactory)
		{
		}
	}
	#endregion Entity Repository Implementations
}