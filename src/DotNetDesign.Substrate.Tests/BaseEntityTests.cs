using System;
using System.Linq;
using Autofac;
using FluentAssertions;
using Moq;
using Xunit;

namespace DotNetDesign.Substrate.Tests
{
    public class BaseEntityTests
    {
        private IPerson _person;
        private IPersonData _personData;
        private IContainer _container;

        #region Setup

        public BaseEntityTests()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new TestModule());
            _container = builder.Build();

            _person = _container.Resolve<IPerson>();
            _personData = _container.Resolve<IPersonData>();
            _personData.FirstName = "John";
            _personData.LastName = "Doe";
            _personData.Version = 1;
        }

        ~BaseEntityTests()
        {
            _container.Dispose();
        }

        #endregion

        [Fact]
        public void PersonPropertyValuesShouldMatchDataPropertyValuesAfterInitialization()
        {
            _person.Initialize(_personData);

            _person.FirstName.Should().Be(_personData.FirstName);
            _person.LastName.Should().Be(_personData.LastName);
            _person.IsDirty.Should().BeFalse();
        }
        
        [Fact]
        public void Person_NotInitialized_ShouldThrowExceptionWhenPropertyGetSet()
        {
            Assert.Throws<InvalidOperationException>(() => { var firstName = _person.FirstName; });
            Assert.Throws<InvalidOperationException>(() => { _person.FirstName = "First Name"; });
        }

        [Fact]
        public void GetPropertyDisplayName_Should_Return_Value_In_Attribute()
        {
            _person.GetPropertyDisplayName(e => e.FirstName).Should().Be("First Name");
            _person.GetPropertyDisplayName("FirstName").Should().Be("First Name");
        }

        [Fact]
        public void GetPropertyDisplayName_Should_Return_StringEmpty_If_Attribute_Is_Missing()
        {
            _person.GetPropertyDisplayName(e => e.LastName).Should().Be(string.Empty);
            _person.GetPropertyDisplayName("LastName").Should().Be(string.Empty);
        }

        [Fact]
        public void PersonShouldNotBeDirtyAfterInitializeAndAfterPropertySetToSameValue()
        {
            _person.Initialize(_personData);

            _person.IsDirty.Should().BeFalse();
            _person.FirstName = _personData.FirstName;
            _person.IsDirty.Should().BeFalse();
        }

        [Fact]
        public void PersonShouldNotBeDirtyAfterInitializeAndShouldBeAfterPropertySetToDifferentValue()
        {
            _person.Initialize(_personData);

            _person.IsDirty.Should().BeFalse();
            var newFirstName = _personData.FirstName + " additional characters";
            _person.FirstName = newFirstName;
            _person.FirstName.Should().Be(newFirstName);
            _person.IsDirty.Should().BeTrue();
        }

        [Fact]
        public void ValidPersonShouldCallSaveOnRepository()
        {
            _person.Initialize(_personData);

            var personRepositoryMock = new Mock<IPersonRepository>(MockBehavior.Strict);
            _person.EntityRepositoryFactory = () => personRepositoryMock.Object;

            personRepositoryMock.Setup(x => x.Save(_person)).Returns(_person);
            _person.Save();
        }

        [Fact]
        public void InvalidPersonShouldNotCallSaveOnRepository()
        {
            _person.Initialize(_personData);
            var personRepositoryMock = new Mock<IPersonRepository>(MockBehavior.Strict);
            _person.EntityRepositoryFactory = () => personRepositoryMock.Object;
            _person.FirstName = null;
            _person.Save();

        }

        [Fact]
        public void VersionShouldNotIncrementIfNoChangesMade()
        {
            _person.Initialize(_personData);

            var expectedVersion = _person.Version;
            IPerson returnedPerson;
            _person.Save(out returnedPerson);
            returnedPerson.Version.Should().Be(expectedVersion);
        }

        [Fact]
        public void VersionShouldIncrementIfChangesMade()
        {
            _person.Initialize(_personData);
            _person.FirstName = _person.FirstName + " more info";
            var expectedVersion = _person.Version + 1;

            var newPerson = _container.Resolve<IPerson>();
            var newEntityData = _person.EntityData;

            var personRepositoryMock = new Mock<IPersonRepository>(MockBehavior.Strict);
            _person.EntityRepositoryFactory = () => personRepositoryMock.Object;
            personRepositoryMock.Setup(x => x.Save(_person)).Returns(() =>
                                                                         {
                                                                             newPerson.Initialize(newEntityData);
                                                                             return newPerson;
                                                                         });

            var concurrencyManagerMock = new Mock<IConcurrencyManager<IPerson, IPersonData, IPersonRepository>>(MockBehavior.Strict);
            _person.EntityConcurrencyManagerFactory = () => concurrencyManagerMock.Object;
            concurrencyManagerMock.Setup(x => x.Verify(_person));

            IPerson returnedPerson;
            _person.Save(out returnedPerson);
            returnedPerson.Version.Should().Be(expectedVersion);
        }

        [Fact]
        public void HasPropertyChangedShouldReturnFalseIfNotChanged()
        {
            _person.Initialize(_personData);
            _person.HasPropertyChanged(x => x.FirstName).Should().BeFalse();
            _person.HasPropertyChanged(x => x.LastName).Should().BeFalse();
        }

        [Fact]
        public void HasPropertyChangedShouldReturnTrueIfChanged()
        {
            _person.Initialize(_personData);
            _person.FirstName = _person.FirstName + " more info";
            _person.HasPropertyChanged(x => x.FirstName).Should().BeTrue();
            _person.HasPropertyChanged(x => x.LastName).Should().BeFalse();
        }

        [Fact]
        public void InvalidOperationExceptionShouldBeThrownIfInitializeIsCalledTwice()
        {
            _person.Initialize(_personData);
            Assert.Throws(typeof (InvalidOperationException), () => _person.Initialize(_personData));
        }

        [Fact]
        public void CallToGetVersionShouldPassInstanceAndVersionToEntityRepository()
        {
            _person.Initialize(_personData);

            var personRepositoryMock = new Mock<IPersonRepository>(MockBehavior.Strict);
            _person.EntityRepositoryFactory = () => personRepositoryMock.Object;

            const int version = 1;
            personRepositoryMock.Setup(x => x.GetVersion(_person.Id, version, false)).Returns(_person);

            _person.GetVersion(version);
        }

        [Fact]
        public void CallToDeleteShouldPassInstanceToEntityRepository()
        {
            _person.Initialize(_personData);

            var personRepositoryMock = new Mock<IPersonRepository>(MockBehavior.Strict);
            _person.EntityRepositoryFactory = () => personRepositoryMock.Object;

            personRepositoryMock.Setup(x => x.Delete(_person));

            _person.Delete();   
        }

        [Fact]
        public void CallToRevertChangesShouldResetValuesAndSetIsDirtyToFalse()
        {
            _person.Initialize(_personData);

            var originalFirstName = _person.FirstName;
            var newFirstName = "NewFirstName";

            var originalLastName = _person.LastName;
            var newLastName = "NewLastName";

            _person.IsDirty.Should().BeFalse();

            var originalValidationResults = _person.Validate();

            _person.FirstName = newFirstName;
            _person.LastName = newLastName;

            _person.IsDirty.Should().BeTrue();
            _person.FirstName.Should().Be(newFirstName);
            _person.LastName.Should().Be(newLastName);

            _person.RevertChanges();

            _person.IsDirty.Should().BeFalse();
            _person.FirstName.Should().Be(originalFirstName);
            _person.LastName.Should().Be(originalLastName);

            _person.Validate().Count().Should().Be(originalValidationResults.Count());
        }

        [Fact]
        public void CallToInitializeShouldCallInitializingAndInitializeOnceEach()
        {
            var initializingCallCount = 0;
            var initializedCallCount = 0;

            _person.Initializing += (sender, e) => initializingCallCount++;
            _person.Initialized += (sender, e) => initializedCallCount++;

            _person.Initialize(_personData);

            initializingCallCount.Should().Be(1);
            initializedCallCount.Should().Be(1);
        }

        [Fact]
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

            propertyChangingCallCount.Should().Be(0);
            propertyChangedCallCount.Should().Be(0);
            propertyChangingPropertyName.Should().BeEmpty();
            propertyChangedPropertyName.Should().BeEmpty();

            _person.FirstName = _personData.FirstName + " more info";

            propertyChangingCallCount.Should().Be(1);
            propertyChangedCallCount.Should().Be(1);
            propertyChangingPropertyName.Should().Be("FirstName");
            propertyChangedPropertyName.Should().Be("FirstName");
        }
    }
}