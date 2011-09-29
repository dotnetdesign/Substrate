using System.Collections.Generic;
using NUnit.Framework;

namespace DotNetDesign.EntityFramework.Tests
{
    [TestFixture]
    public class StateMachineTests
    {
        private StateMachine<int> _stateMachine;

        [SetUp]
        public void Setup()
        {
            _stateMachine =
                new StateMachine<int>(new Dictionary<int, IEnumerable<int>>
                                          {
                                              {0, new[] {1}},
                                              {1, new[] {2}},
                                              {2, new[] {3}},
                                              {3, new[] {5}}
                                          });
        }

        [Test]
        public void ValidStateMachine_InvalidChange_ShouldThrowException()
        {
            var invalidStateException = Assert.Throws<InvalidStateException<int>>(() => _stateMachine.ChangeState(3));
            Assert.AreEqual(0, invalidStateException.CurrentState);
            Assert.AreEqual(3, invalidStateException.TargetState);
            Assert.AreEqual(new[] {1}, invalidStateException.AllowedStates);
        }

        [Test]
        public void ValidStateMachine_ValidChange_ShouldTriggerChangingAndChangedEvents()
        {
            var numberOfTimesChangingWasCalled = 0;
            var numberOfTimesChangedWasCalled = 0;
            int changingOriginalValue = -1;
            int changedOriginalValue = -1;
            int changingNewValue = -1;
            int changedNewValue = -1;

            _stateMachine.StateChanging += delegate(object sender, StateChangeEventArgs<int> e)
                                               {
                                                   numberOfTimesChangingWasCalled++;
                                                   changingOriginalValue = e.OriginalState;
                                                   changingNewValue = e.NewState;
                                               };
            _stateMachine.StateChanged += delegate(object sender, StateChangeEventArgs<int> e)
                                              {
                                                  numberOfTimesChangedWasCalled++;
                                                  changedOriginalValue = e.OriginalState;
                                                  changedNewValue = e.NewState;
                                              };

            _stateMachine.ChangeState(1);
            Assert.AreEqual(1, _stateMachine.CurrentState);
            Assert.AreEqual(new []{2}, _stateMachine.GetValidNextStates());
            Assert.AreEqual(1, numberOfTimesChangingWasCalled);
            Assert.AreEqual(1, numberOfTimesChangedWasCalled);
            Assert.AreEqual(0, changingOriginalValue);
            Assert.AreEqual(1, changingNewValue);
            Assert.AreEqual(0, changedOriginalValue);
            Assert.AreEqual(1, changedNewValue);
        }
    }
}