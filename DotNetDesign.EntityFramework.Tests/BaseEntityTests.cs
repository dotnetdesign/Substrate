using System;
using Autofac;
using NUnit.Framework;
using NUnit.Mocks;
using Moq;
using Mock = Moq.Mock;

namespace DotNetDesign.EntityFramework.Tests
{
    [TestFixture]
    public class BaseEntityTests
    {
        private IPerson _person;
        private IPersonData _personData;
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
            _person = _container.Resolve<IPerson>();
            _personData = _container.Resolve<IPersonData>();
            _personData.FirstName = "John";
            _personData.LastName = "Doe";
        }

        #endregion

        [Test]
        public void PersonPropertyValuesShouldMatchDataPropertyValuesAfterInitialization()
        {
            Assert.AreNotEqual(_personData.FirstName, _person.FirstName);
            Assert.AreNotEqual(_personData.LastName, _person.LastName);
            Assert.IsFalse(_person.IsDirty);

            _person.Initialize(_personData);

            Assert.AreEqual(_personData.FirstName, _person.FirstName);
            Assert.AreEqual(_personData.LastName, _person.LastName);
            Assert.IsFalse(_person.IsDirty);
        }

        [Test]
        public void PersonShouldNotBeDirtyAfterInitializeAndAfterPropertySetToSameValue()
        {
            _person.Initialize(_personData);

            Assert.IsFalse(_person.IsDirty);
            _person.FirstName = _personData.FirstName;
            Assert.IsFalse(_person.IsDirty);
        }

        [Test]
        public void PersonShouldNotBeDirtyAfterInitializeAndShouldBeAfterPropertySetToDifferentValue()
        {
            _person.Initialize(_personData);

            Assert.IsFalse(_person.IsDirty);
            var newFirstName = _personData.FirstName + " additional characters";
            _person.FirstName = newFirstName;
            Assert.AreEqual(newFirstName, _person.FirstName);
            Assert.IsTrue(_person.IsDirty);
        }

        [Test]
        public void ValidPersonShouldCallSaveOnRepository()
        {
            var personRepositoryMock = new Mock<IPersonRepository>(MockBehavior.Strict);
            _person.EntityRepository = personRepositoryMock.Object;

            personRepositoryMock.Setup(x => x.Save(_person)).Returns(_person);
            _person.Save();
        }

        [Test]
        public void InvalidPersonShouldNotCallSaveOnRepository()
        {
            _person.Initialize(_personData);
            var personRepositoryMock = new Mock<IPersonRepository>(MockBehavior.Strict);
            _person.EntityRepository = personRepositoryMock.Object;
            _person.FirstName = null;
            _person.Save();

        }

        [Test]
        public void VersionShouldNotIncrementIfNoChangesMade()
        {
            _person.Initialize(_personData);

            var expectedVersion = _person.Version;
            IPerson returnedPerson;
            _person.Save(out returnedPerson);
            Assert.AreEqual(expectedVersion, returnedPerson.Version);
        }

        [Test]
        public void VersionShouldIncrementIfChangesMade()
        {
            _person.Initialize(_personData);
            _person.FirstName = _person.FirstName + " more info";
            var expectedVersion = _person.Version + 1;

            var personRepositoryMock = new Mock<IPersonRepository>(MockBehavior.Strict);
            _person.EntityRepository = personRepositoryMock.Object;
            personRepositoryMock.Setup(x => x.Save(_person)).Returns(_person);

            IPerson returnedPerson;
            _person.Save(out returnedPerson);
            Assert.AreEqual(expectedVersion, returnedPerson.Version);
        }

        [Test]
        public void HasPropertyChangedShouldReturnFalseIfNotChanged()
        {
            _person.Initialize(_personData);
            Assert.IsFalse(_person.HasPropertyChanged(x => x.FirstName));
            Assert.IsFalse(_person.HasPropertyChanged(x => x.LastName));
        }

        [Test]
        public void HasPropertyChangedShouldReturnTrueIfChanged()
        {
            _person.Initialize(_personData);
            _person.FirstName = _person.FirstName + " more info";
            Assert.IsTrue(_person.HasPropertyChanged(x => x.FirstName));
            Assert.IsFalse(_person.HasPropertyChanged(x => x.LastName));
        }

        [Test]
        public void InvalidOperationExceptionShouldBeThrownIfInitializeIsCalledTwice()
        {
            _person.Initialize(_personData);
            Assert.Throws(typeof (InvalidOperationException), () => _person.Initialize(_personData));
        }

        [Test]
        public void CallToGetVersionShouldPassInstanceAndVersionToEntityRepository()
        {
            _person.Initialize(_personData);

            var personRepositoryMock = new Mock<IPersonRepository>(MockBehavior.Strict);
            _person.EntityRepository = personRepositoryMock.Object;

            const int version = 1;
            personRepositoryMock.Setup(x => x.GetVersion(_person, version)).Returns(_person);

            _person.GetVersion(version);
        }
    }
}