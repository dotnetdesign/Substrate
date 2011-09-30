using System;
using Autofac;
using Moq;
using NUnit.Framework;

namespace DotNetDesign.EntityFramework.Tests
{
    [TestFixture]
    public class ConcurrencyTests
    {
        private IContainer _container;

        #region Setup

        [TestFixtureSetUp]
        public void SetUpTestFixture()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new TestModule());
            _container = builder.Build();
        }

        [TestFixtureTearDown]
        public void TearDownTestFixture()
        {
            _container.Dispose();
        }

        [SetUp]
        public void TestSetUp()
        {
        }

        #endregion
        
        [Test]
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
            Assert.AreEqual(ConcurrencyMode.Merge, exceptionThrown.ConcurrencyMode);
        }

        [Test]
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

            var personRepositoryMock = new Mock<IPersonRepository>(MockBehavior.Strict);
            personRepositoryMock.Setup(x => x.GetById(commonId, true)).Returns(updatedPerson);
            personRepositoryMock.Setup(x => x.Save(person)).Returns(person);

            var concurrencyManager = _container.Resolve<IConcurrencyManager<IPerson, IPersonData, IPersonRepository>>();
            concurrencyManager.EntityRepository = personRepositoryMock.Object;
            person.EntityConcurrencyManagerFactory = () => concurrencyManager;

            person.EntityRepositoryFactory = () => personRepositoryMock.Object;

            IPerson savedPerson;
            Assert.IsTrue(person.Save(out savedPerson));
            Assert.AreEqual(person.FirstName, savedPerson.FirstName);
            Assert.AreEqual(updatedPersonData.LastName, savedPerson.LastName);
            Assert.AreEqual(updatedPerson.Version + 1, savedPerson.Version);
        }

        [Test]
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

            var overwriteConcurrencyRepositoryMock = new Mock<IOverwriteConcurrencyRepository>(MockBehavior.Strict);
            overwriteConcurrencyRepositoryMock.Setup(x => x.GetById(commonId, true)).Returns(updatedOverwriteConcurrency);
            overwriteConcurrencyRepositoryMock.Setup(x => x.Save(overwriteConcurrency)).Returns(overwriteConcurrency);

            var concurrencyManager = _container.Resolve<IConcurrencyManager<IOverwriteConcurrency, IOverwriteConcurrencyData, IOverwriteConcurrencyRepository>>();
            concurrencyManager.EntityRepository = overwriteConcurrencyRepositoryMock.Object;
            overwriteConcurrency.EntityConcurrencyManagerFactory = () => concurrencyManager;

            overwriteConcurrency.EntityRepositoryFactory = () => overwriteConcurrencyRepositoryMock.Object;

            IOverwriteConcurrency savedOverwriteConcurrency;
            Assert.IsTrue(overwriteConcurrency.Save(out savedOverwriteConcurrency));
            Assert.AreEqual("Jon", savedOverwriteConcurrency.FirstName);
            Assert.AreEqual("Dooo!", savedOverwriteConcurrency.LastName);
            Assert.AreEqual(newVersion, savedOverwriteConcurrency.Version);
        }

        [Test]
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
            Assert.AreEqual(ConcurrencyMode.Fail, exceptionThrown.ConcurrencyMode);
        }
    }
}