using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.ServiceModel;
using DotNetDesign.Common;
using DotNetDesign.Substrate;
using Common.Logging;

namespace DotNetDesign.Substrate.Example.Entities
{
	internal static class TraceTopics
    {
        static TraceTopics()
        {
            Entities = LogManager.GetLogger("DotNetDesign.Substrate.Example.Entities");
        }

        internal static ILog Entities;
    }

	#region Entity Data Interfaces
	public partial interface IUserData : IEntityData<IUserData, IUser, IUserRepository>
	{
		[Required]
		[DisplayName("First Name")]
		string FirstName { get; set; }
		[Required]
		[DisplayName("Last Name")]
		string LastName { get; set; }
		[Required]
		[DisplayName("E-mail Address")]
		string EmailAddress { get; set; }
	}
	#endregion Entity Data Interfaces

	#region Entity Interfaces
	public partial interface IUser : IEntity<IUser, IUserData, IUserRepository>, IUserData {}
	#endregion Entity Interfaces

	#region Entity Repository Interfaces
	public partial interface IUserRepository : IEntityRepository<IUserRepository, IUser, IUserData> {}
	#endregion Entity Repository Interfaces

	#region Entity Repository Service Interfaces
	[ServiceContract]
	public partial interface IUserRepositoryService : IEntityRepositoryService<IUserData, IUser, IUserRepository, UserData> {}
	#endregion Entity Repository Service Interfaces

	#region Entity Data Implementations
	[DataContract]
	[Serializable]
	public partial class UserData : BaseEntityData<IUserData, IUser, IUserRepository>, IUserData
	{
		#region IUserData Properties
		[DataMember]
		public virtual string FirstName { get; set; }
		[DataMember]
		public virtual string LastName { get; set; }
		[DataMember]
		public virtual string EmailAddress { get; set; }
		#endregion IUserData Properties
	}
	#endregion Entity Data Implementations

	#region Entity Implementations
		public partial class User : BaseEntity<IUser, IUserData, IUserRepository>, IUser
		{
			public User(
				Func<IUserRepository> entityRepositoryFactory,
				Func<IUserData> entityDataFactory,
				Func<IConcurrencyManager<IUser, IUserData, IUserRepository>> entityConcurrencyManagerFactory,
				IEnumerable<IEntityValidator<IUser, IUserData, IUserRepository>> entityValidators,
	            Func<IPermissionAuthorizationManager<IUser, IUserData, IUserRepository>> permissionAuthorizationManagerFactory)
				: base(entityRepositoryFactory, entityDataFactory, entityConcurrencyManagerFactory, entityValidators, permissionAuthorizationManagerFactory)
			{
			}

			#region IUserData Properties
			public virtual string FirstName 
		{
			get 
			{
				using(TraceTopics.Entities.Scope())
				{ 
					return EntityData.FirstName; 
				}
			}
			set
			{
				using(TraceTopics.Entities.Scope())
				{ 
					if (EntityData.FirstName == value) return;

					var originalValue = EntityData.FirstName;
					OnPropertyChanging("FirstName", originalValue, value);
					EntityData.FirstName = value;
					OnPropertyChanged("FirstName", originalValue, value);
				}
			}
		}
		public virtual string LastName 
		{
			get 
			{
				using(TraceTopics.Entities.Scope())
				{ 
					return EntityData.LastName; 
				}
			}
			set
			{
				using(TraceTopics.Entities.Scope())
				{ 
					if (EntityData.LastName == value) return;

					var originalValue = EntityData.LastName;
					OnPropertyChanging("LastName", originalValue, value);
					EntityData.LastName = value;
					OnPropertyChanged("LastName", originalValue, value);
				}
			}
		}
		public virtual string EmailAddress 
		{
			get 
			{
				using(TraceTopics.Entities.Scope())
				{ 
					return EntityData.EmailAddress; 
				}
			}
			set
			{
				using(TraceTopics.Entities.Scope())
				{ 
					if (EntityData.EmailAddress == value) return;

					var originalValue = EntityData.EmailAddress;
					OnPropertyChanging("EmailAddress", originalValue, value);
					EntityData.EmailAddress = value;
					OnPropertyChanged("EmailAddress", originalValue, value);
				}
			}
		}
		#endregion IUserData Properties
	}
	#endregion Entity Implementations

	#region Entity Repository Implementations
		public partial class UserRepository : EntityRepository<IUser, IUserData, UserData, IUserRepository, IUserRepositoryService>, IUserRepository
		{
			public UserRepository(
				Func<IUser> entityFactory,
				Func<IUserData> entityDataFactory,
				Func<IUserRepositoryService> entityRepositoryServiceFactory,
				IEnumerable<IEntityObserver<IUser>> entityObservers,
				Func<IEntityCache<IUser, IUserData, IUserRepository>> entityCacheFactory,
				Func<IScopeManager> scopeManagerFactory)
				: base(entityFactory, entityDataFactory, entityRepositoryServiceFactory, entityObservers, entityCacheFactory, scopeManagerFactory)
			{
			}
		}
		#endregion Entity Repository Implementations
}
