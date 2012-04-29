using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using DotNetDesign.EntityFramework;

namespace PerformanceTestConsole
{
    public class PerformanceTestModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<Person>().As<IPerson>();
            builder.RegisterType<PersonData>().As<IPersonData>();
            builder.RegisterType<PersonRepository>().As<IPersonRepository>();
            builder.RegisterType<PersonRepositoryService>().As<IPersonRepositoryService>();

            builder.RegisterType<DataAnnotationEntityValidator<IPerson, IPersonData, IPersonRepository>>()
                .As<IEntityValidator<IPerson, IPersonData, IPersonRepository>>();

            builder.RegisterType<ConcurrencyManager<IPerson, IPersonData, IPersonRepository>>()
                .As<IConcurrencyManager<IPerson, IPersonData, IPersonRepository>>();

            builder.RegisterType<AnonymousPermissionAuthorizationManager<IPerson, IPersonData, IPersonRepository, Guid>>()
                .As<IPermissionAuthorizationManager<IPerson, IPersonData, IPersonRepository>>();

            builder.RegisterType<DictionaryEntityCache<IPerson, IPersonData, IPersonRepository>>()
                .As<IEntityCache<IPerson, IPersonData, IPersonRepository>>();

            builder.RegisterType<PerformanceTestScopeManager>().As<IScopeManager>();
        }
    }
}
