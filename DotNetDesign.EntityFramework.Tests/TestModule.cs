using Autofac;

namespace DotNetDesign.EntityFramework.Tests
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
        }
    }
}