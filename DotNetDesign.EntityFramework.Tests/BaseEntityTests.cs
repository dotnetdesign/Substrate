using Autofac;
using NUnit.Framework;
using NUnit.Mocks;

namespace DotNetDesign.EntityFramework.Tests
{
    [TestFixture]
    public class BaseEntityTests
    {
        private IContainer _container;

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

        private IPerson _person;
        private IPersonData _personData;
        [SetUp]
        public void TestSetUp()
        {
            _person = _container.Resolve<IPerson>();
            _personData = _container.Resolve<IPersonData>();
            _personData.FirstName = "John";
            _personData.LastName = "Doe";
        }

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
    }
}