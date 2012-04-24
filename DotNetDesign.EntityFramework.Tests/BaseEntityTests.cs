using System;
using System.Linq;
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
            _person.Initialize(_personData);

            Assert.AreEqual(_personData.FirstName, _person.FirstName);
            Assert.AreEqual(_personData.LastName, _person.LastName);
            Assert.IsFalse(_person.IsDirty);
        }

        [Test]
        public void Person_NotInitialized_ShouldThrowExceptionWhenPropertyGetSet()
        {
            Assert.Throws<InvalidOperationException>(() =>
                                                         {
                                                             var firstName = _person.FirstName;
                                                         });
            Assert.Throws<InvalidOperationException>(() =>
                                                         {
                                                             _person.FirstName = "First Name";
                                                         });
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
            _person.Initialize(_personData);

            var personRepositoryMock = new Mock<IPersonRepository>(MockBehavior.Strict);
            _person.EntityRepositoryFactory = () => personRepositoryMock.Object;

            personRepositoryMock.Setup(x => x.Save(_person)).Returns(_person);
            _person.Save();
        }

        [Test]
        public void InvalidPersonShouldNotCallSaveOnRepository()
        {
            _person.Initialize(_personData);
            var personRepositoryMock = new Mock<IPersonRepository>(MockBehavior.Strict);
            _person.EntityRepositoryFactory = () => personRepositoryMock.Object;
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
            _person.EntityRepositoryFactory = () => personRepositoryMock.Object;
            personRepositoryMock.Setup(x => x.Save(_person)).Returns(_person);

            var concurrencyManagerMock = new Mock<IConcurrencyManager<IPerson, IPersonData, IPersonRepository>>(MockBehavior.Strict);
            _person.EntityConcurrencyManagerFactory = () => concurrencyManagerMock.Object;
            concurrencyManagerMock.Setup(x => x.Verify(_person));

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
            _person.EntityRepositoryFactory = () => personRepositoryMock.Object;

            const int version = 1;
            personRepositoryMock.Setup(x => x.GetVersion(_person.Id, version, false)).Returns(_person);

            _person.GetVersion(version);
        }

        [Test]
        public void CallToDeleteShouldPassInstanceToEntityRepository()
        {
            _person.Initialize(_personData);

            var personRepositoryMock = new Mock<IPersonRepository>(MockBehavior.Strict);
            _person.EntityRepositoryFactory = () => personRepositoryMock.Object;

            personRepositoryMock.Setup(x => x.Delete(_person));

            _person.Delete();   
        }

        [Test]
        public void CallToRevertChangesShouldResetValuesAndSetIsDirtyToFalse()
        {
            _person.Initialize(_personData);

            var originalFirstName = _person.FirstName;
            var newFirstName = "NewFirstName";

            var originalLastName = _person.LastName;
            var newLastName = "NewLastName";

            Assert.IsFalse(_person.IsDirty);

            var originalValidationResults = _person.Validate();

            _person.FirstName = newFirstName;
            _person.LastName = newLastName;

            Assert.IsTrue(_person.IsDirty);
            Assert.AreEqual(newFirstName, _person.FirstName);
            Assert.AreEqual(newLastName, _person.LastName);

            _person.RevertChanges();

            Assert.IsFalse(_person.IsDirty);
            Assert.AreEqual(originalFirstName, _person.FirstName);
            Assert.AreEqual(originalLastName, _person.LastName);

            Assert.AreEqual(originalValidationResults.Count(), _person.Validate().Count());
        }

        [Test]
        public void CallToInitializeShouldCallInitializingAndInitializeOnceEach()
        {
            var initializingCallCount = 0;
            var initializedCallCount = 0;

            _person.Initializing += (sender, e) => initializingCallCount++;
            _person.Initialized += (sender, e) => initializedCallCount++;

            _person.Initialize(_personData);

            Assert.AreEqual(1, initializingCallCount);
            Assert.AreEqual(1, initializedCallCount);
        }

        [Test]
        public void WhenPropertyValueChangesPropertyChangingAndPropertyChangedEventsShouldBeTriggered()
        {
            var propertyChangingCallCount = 0;
            var propertyChangedCallCount = 0;

            var propertyChangingPropertyName = string.Empty;
            var propertyChangedPropertyName = string.Empty;

            _person.PropertyChanging +=
                (sender, e) =>
                    {
                        propertyChangingCallCount++;
                        propertyChangingPropertyName = e.PropertyName;
                    };
            _person.PropertyChanged +=
                (sender, e) =>
                    {
                        propertyChangedCallCount++;
                        propertyChangedPropertyName = e.PropertyName;
                    };

            _person.Initialize(_personData);

            _person.FirstName = _personData.FirstName;

            Assert.AreEqual(0, propertyChangingCallCount);
            Assert.AreEqual(0, propertyChangedCallCount);
            Assert.IsEmpty(propertyChangingPropertyName);
            Assert.IsEmpty(propertyChangedPropertyName);

            _person.FirstName = _personData.FirstName + " more info";

            Assert.AreEqual(1, propertyChangingCallCount);
            Assert.AreEqual(1, propertyChangedCallCount);
            Assert.AreEqual("FirstName", propertyChangingPropertyName);
            Assert.AreEqual("FirstName", propertyChangedPropertyName);
        }
    }
}