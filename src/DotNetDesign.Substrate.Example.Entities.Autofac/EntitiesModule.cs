using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetDesign.Substrate.Example.Entities.Autofac
{
    public class EntitiesModule : Module 
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<User>().As<IUser>().InstancePerDependency();
            builder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerLifetimeScope();
            builder.RegisterType<UserData>().As<IUserData>().InstancePerDependency();
            builder.RegisterType<DictionaryEntityCache<IUser, IUserData, IUserRepository>>()
                .As<IEntityCache<IUser, IUserData, IUserRepository>>().InstancePerLifetimeScope();
            builder.RegisterType<ConcurrencyManager<IUser, IUserData, IUserRepository>>()
                .As<IConcurrencyManager<IUser, IUserData, IUserRepository>>();
            builder.RegisterType<DataAnnotationEntityValidator<IUser, IUserData, IUserRepository>>()
                .As<IEntityValidator<IUser, IUserData, IUserRepository>>();
            builder.RegisterType<AnonymousPermissionAuthorizationManager<IUser, IUserData, IUserRepository>>()
                .As<IPermissionAuthorizationManager<IUser, IUserData, IUserRepository>>()
                .As<IPermissionAuthorizationManager<IUser, IUserData, IUserRepository>>();
        }
    }
}
