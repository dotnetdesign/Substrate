using System;
using Autofac;
using Moq;
using Xunit;
using FluentAssertions;

namespace DotNetDesign.Substrate.Tests
{
    public class ConcurrencyTests
    {
        private IContainer _container;

        #region Setup

        public ConcurrencyTests()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new TestModule());
            _container = builder.Build();
        }

        ~ConcurrencyTests()
        {
            _container.Dispose();
        }

        #endregion
        
        [Fact]
        public void EntityWithoutConcurrencyModeAttribute_ShouldDefaultToMergeMode()
        {
            // to test this out, we need a IEntityData without a concurrency mode attribute
            // we then need to get a merge conflict to cause an exception to be thrown and 
            // then we can validate that exception's concurrency mode attribute

            var commonId = Guid.NewGuid();

            var person = _container.Resolve<IPerson>();
            var personData = _container.Resolve<IPersonData>();
            personData.Id = commonId;
            personData.FirstName = "John";
            personData.LastName = "Doe";
            personData.Version = 1;
            person.Initialize(personData);

            var updatedPerson = _container.Resolve<IPerson>();
            var updatedPersonData = _container.Resolve<IPersonData>();
            updatedPersonData.Id = commonId;
            updatedPersonData.FirstName = "John";
            updatedPersonData.LastName = "Dough";
            updatedPersonData.Version = 2;
            updatedPerson.Initialize(updatedPersonData);

            // update the person lastname to the lastname of the updated person
            person.LastName = updatedPerson.LastName;

            var personRepositoryMock = new Mock<IPersonRepository>(MockBehavior.Strict);
            personRepositoryMock.Setup(x => x.GetById(commonId, true)).Returns(updatedPerson);

            var concurrencyManager = _container.Resolve<IConcurrencyManager<IPerson, IPersonData, IPersonRepository>>();
            concurrencyManager.EntityRepository = personRepositoryMock.Object;
            person.EntityConcurrencyManagerFactory = () => concurrencyManager;

            var exceptionThrown = Assert.Throws<ConcurrencyConflictException>(() => person.Save());
            exceptionThrown.ConcurrencyMode.Should().Be(ConcurrencyMode.Merge);
        }

        [Fact]
        public void MergeConcurrencyMode_WithoutConflictingPropertyChanges_ShouldSucceed()
        {
            var commonId = Guid.NewGuid();

            var person = _container.Resolve<IPerson>();
            var personData = _container.Resolve<IPersonData>();
            personData.Id = commonId;
            personData.FirstName = "John";
            personData.LastName = "Doe";
            personData.Version = 1;
            person.Initialize(personData);

            var updatedPerson = _container.Resolve<IPerson>();
            var updatedPersonData = _container.Resolve<IPersonData>();
            updatedPersonData.Id = commonId;
            updatedPersonData.FirstName = "John";
            updatedPersonData.LastName = "Dough";
            updatedPersonData.Version = 2;
            updatedPerson.Initialize(updatedPersonData);

            person.FirstName = "Jon";
            
            var newPerson = _container.Resolve<IPerson>();
            var newEntityData = person.EntityData;

            var personRepositoryMock = new Mock<IPersonRepository>(MockBehavior.Strict);
            personRepositoryMock.Setup(x => x.GetById(commonId, true)).Returns(updatedPerson);
            personRepositoryMock.Setup(x => x.Save(person))
                .Returns(() =>
                             {
                                 newPerson.Initialize(newEntityData);
                                 return newPerson;
                             });

            var concurrencyManager = _container.Resolve<IConcurrencyManager<IPerson, IPersonData, IPersonRepository>>();
            concurrencyManager.EntityRepository = personRepositoryMock.Object;
            person.EntityConcurrencyManagerFactory = () => concurrencyManager;

            person.EntityRepositoryFactory = () => personRepositoryMock.Object;

            IPerson savedPerson;
            person.Save(out savedPerson).Should().BeTrue();
            savedPerson.FirstName.Should().Be(person.FirstName);
            savedPerson.LastName.Should().Be(updatedPersonData.LastName);
            savedPerson.Version.Should().Be(updatedPerson.Version + 1);
        }

        [Fact]
        public void OverwriteConcurrencyMode_WithConflictingPropertyChanges_ShouldSucceed()
        {
            var commonId = Guid.NewGuid();

            var overwriteConcurrency = _container.Resolve<IOverwriteConcurrency>();
            var overwriteConcurrencyData = _container.Resolve<IOverwriteConcurrencyData>();
            overwriteConcurrencyData.Id = commonId;
            overwriteConcurrencyData.FirstName = "John";
            overwriteConcurrencyData.LastName = "Doe";
            overwriteConcurrencyData.Version = 1;
            overwriteConcurrency.Initialize(overwriteConcurrencyData);

            var updatedOverwriteConcurrency = _container.Resolve<IOverwriteConcurrency>();
            var updatedOverwriteConcurrencyData = _container.Resolve<IOverwriteConcurrencyData>();
            updatedOverwriteConcurrencyData.Id = commonId;
            updatedOverwriteConcurrencyData.FirstName = "John";
            updatedOverwriteConcurrencyData.LastName = "Dough";
            updatedOverwriteConcurrencyData.Version = 2;
            updatedOverwriteConcurrency.Initialize(updatedOverwriteConcurrencyData);

            overwriteConcurrency.FirstName = "Jon";
            overwriteConcurrency.LastName = "Dooo!";
            var newVersion = overwriteConcurrency.Version + 1;

            var newOverwriteConcurrency = _container.Resolve<IOverwriteConcurrency>();
            var newEntityData = overwriteConcurrency.EntityData;

            var overwriteConcurrencyRepositoryMock = new Mock<IOverwriteConcurrencyRepository>(MockBehavior.Strict);
            overwriteConcurrencyRepositoryMock.Setup(x => x.GetById(commonId, true)).Returns(updatedOverwriteConcurrency);
            overwriteConcurrencyRepositoryMock.Setup(x => x.Save(overwriteConcurrency))
                .Returns(() =>
                             {
                                 newOverwriteConcurrency.Initialize(newEntityData);
                                 return newOverwriteConcurrency;
                             });

            var concurrencyManager = _container.Resolve<IConcurrencyManager<IOverwriteConcurrency, IOverwriteConcurrencyData, IOverwriteConcurrencyRepository>>();
            concurrencyManager.EntityRepository = overwriteConcurrencyRepositoryMock.Object;
            overwriteConcurrency.EntityConcurrencyManagerFactory = () => concurrencyManager;

            overwriteConcurrency.EntityRepositoryFactory = () => overwriteConcurrencyRepositoryMock.Object;

            IOverwriteConcurrency savedOverwriteConcurrency;
            overwriteConcurrency.Save(out savedOverwriteConcurrency).Should().BeTrue();
            savedOverwriteConcurrency.FirstName.Should().Be("Jon");
            savedOverwriteConcurrency.LastName.Should().Be("Dooo!");
            savedOverwriteConcurrency.Version.Should().Be(newVersion);
        }

        [Fact]
        public void FailConcurrencyMode_WithoutConflictingPropertyChanges_ShouldThrowException()
        {
            var commonId = Guid.NewGuid();

            var failConcurrency = _container.Resolve<IFailConcurrency>();
            var failConcurrencyData = _container.Resolve<IFailConcurrencyData>();
            failConcurrencyData.Id = commonId;
            failConcurrencyData.FirstName = "John";
            failConcurrencyData.LastName = "Doe";
            failConcurrencyData.Version = 1;
            failConcurrency.Initialize(failConcurrencyData);

            var updatedFailConcurrency = _container.Resolve<IFailConcurrency>();
            var updatedFailConcurrencyData = _container.Resolve<IFailConcurrencyData>();
            updatedFailConcurrencyData.Id = commonId;
            updatedFailConcurrencyData.FirstName = "John";
            updatedFailConcurrencyData.LastName = "Dough";
            updatedFailConcurrencyData.Version = 2;
            updatedFailConcurrency.Initialize(updatedFailConcurrencyData);

            failConcurrency.FirstName = "Jon";

            var failConcurrencyRepositoryMock = new Mock<IFailConcurrencyRepository>(MockBehavior.Strict);
            failConcurrencyRepositoryMock.Setup(x => x.GetById(commonId, true)).Returns(updatedFailConcurrency);

            var concurrencyManager = _container.Resolve<IConcurrencyManager<IFailConcurrency, IFailConcurrencyData, IFailConcurrencyRepository>>();
            concurrencyManager.EntityRepository = failConcurrencyRepositoryMock.Object;
            failConcurrency.EntityConcurrencyManagerFactory = () => concurrencyManager;

            var exceptionThrown = Assert.Throws<ConcurrencyConflictException>(() => failConcurrency.Save());
            exceptionThrown.ConcurrencyMode.Should().Be(ConcurrencyMode.Fail);
        }
    }
}