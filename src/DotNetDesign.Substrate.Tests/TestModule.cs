using System;
using Autofac;

namespace DotNetDesign.Substrate.Tests
{
    public class TestModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            builder.RegisterType<Person>().As<IPerson>();
            builder.RegisterType<PersonData>().As<IPersonData>();
            builder.RegisterType<PersonRepository>().As<IPersonRepository>();
            builder.RegisterType<PersonRepositoryService>().As<IPersonRepositoryService>();
            builder.RegisterType<DataAnnotationEntityValidator<IPerson, IPersonData, IPersonRepository>>().As
                <IEntityValidator<IPerson, IPersonData, IPersonRepository>>();
            builder.RegisterType<ConcurrencyManager<IPerson, IPersonData, IPersonRepository>>().As<IConcurrencyManager<IPerson, IPersonData, IPersonRepository>>();
            builder.RegisterType<AnonymousPermissionAuthorizationManager<IPerson, IPersonData, IPersonRepository, Guid>>().As<IPermissionAuthorizationManager<IPerson, IPersonData, IPersonRepository>>();
			
            builder.RegisterType<FailConcurrency>().As<IFailConcurrency>();
            builder.RegisterType<FailConcurrencyData>().As<IFailConcurrencyData>();
            builder.RegisterType<FailConcurrencyRepository>().As<IFailConcurrencyRepository>();
            builder.RegisterType<FailConcurrencyRepositoryService>().As<IFailConcurrencyRepositoryService>();
            builder.RegisterType<DataAnnotationEntityValidator<IFailConcurrency, IFailConcurrencyData, IFailConcurrencyRepository>>().As
                <IEntityValidator<IFailConcurrency, IFailConcurrencyData, IFailConcurrencyRepository>>();
            builder.RegisterType<ConcurrencyManager<IFailConcurrency, IFailConcurrencyData, IFailConcurrencyRepository>>()
                .As<IConcurrencyManager<IFailConcurrency, IFailConcurrencyData, IFailConcurrencyRepository>>();
            builder.RegisterType<AnonymousPermissionAuthorizationManager<IFailConcurrency, IFailConcurrencyData, IFailConcurrencyRepository, Guid>>()
                .As<IPermissionAuthorizationManager<IFailConcurrency, IFailConcurrencyData, IFailConcurrencyRepository>>();

            builder.RegisterType<OverwriteConcurrency>().As<IOverwriteConcurrency>();
            builder.RegisterType<OverwriteConcurrencyData>().As<IOverwriteConcurrencyData>();
            builder.RegisterType<OverwriteConcurrencyRepository>().As<IOverwriteConcurrencyRepository>();
            builder.RegisterType<OverwriteConcurrencyRepositoryService>().As<IOverwriteConcurrencyRepositoryService>();
            builder.RegisterType<DataAnnotationEntityValidator<IOverwriteConcurrency, IOverwriteConcurrencyData, IOverwriteConcurrencyRepository>>().As
                <IEntityValidator<IOverwriteConcurrency, IOverwriteConcurrencyData, IOverwriteConcurrencyRepository>>();
            builder.RegisterType<ConcurrencyManager<IOverwriteConcurrency, IOverwriteConcurrencyData, IOverwriteConcurrencyRepository>>()
                .As<IConcurrencyManager<IOverwriteConcurrency, IOverwriteConcurrencyData, IOverwriteConcurrencyRepository>>();
            builder.RegisterType<AnonymousPermissionAuthorizationManager<IOverwriteConcurrency, IOverwriteConcurrencyData, IOverwriteConcurrencyRepository, Guid>>()
                .As<IPermissionAuthorizationManager<IOverwriteConcurrency, IOverwriteConcurrencyData, IOverwriteConcurrencyRepository>>();

            builder.RegisterType<DictionaryScopeManager>().As<IScopeManager>();
        }
    }
}