using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using DotNetDesign.EntityFramework;

namespace DotNetFramework.EntityFramework.ConsoleTester
{
    public class ConsoleModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Person>().As<IPerson>();
            builder.RegisterType<PersonData>().As<IPersonData>();
            builder.RegisterType<PersonRepository>().As<IPersonRepository>();
            builder.RegisterType<PersonRepositoryService>().As<IPersonRepositoryService>();
            builder.RegisterType<ConcurrencyManager<IPerson, IPersonData, IPersonRepository>>().As<IConcurrencyManager<IPerson, IPersonData, IPersonRepository>>();
            builder.RegisterType<DictionaryEntityCache<IPerson, IPersonData, IPersonRepository>>().As<IEntityCache<IPerson, IPersonData, IPersonRepository>>();
        }
    }
}
