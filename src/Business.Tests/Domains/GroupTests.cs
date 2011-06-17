using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business.Model;

namespace Business.Tests.Domains
{
    [TestFixture]
    public class GroupTests
    {
        [Test]
        public void WhenBothGroupsAreTRansientAndRefEquals_ThenAreequals()
        {
            var foo = new Group {Id = null};
            
            foo.Equals(foo)
                .Should().Be.True();
        }

        [Test]
        public void WhenBothGroupsArePersistedAndHasSameId_ThenAreequals()
        {
            var foo = new Group { Id = new Identity("4de7e38617b6c420a45a84c4") };
            var bar = new Group { Id = new Identity("4de7e38617b6c420a45a84c4") };

            foo.Equals(bar)
                .Should().Be.True();
        }

        [Test]
        public void WhenOtherIsTransientAndThisIsPersisted_ThenAreNotEquals()
        {
            var @this = new Group { Id = new Identity("4de7e38617b6c420a45a84c4") };
            var other = new Group { Id = null};

            @this.Equals(other)
                .Should().Be.False();
        }

        [Test]
        public void WhenOtherIsPersistedAndThisIsTransient_ThenAreNotEquals()
        {
            var foo = new Group { Id = null };
            var bar = new Group { Id = new Identity("4de7e38617b6c420a45a84c4") };

            foo.Equals(bar)
                .Should().Be.False();
        }

        [Test]
        public void WhenBothHaveDifferentKeys_ThenAreequals()
        {
            var foo = new Group { Id = new Identity("Fde7e38617b6c420a45a84c4") };
            var bar = new Group { Id = new Identity("Ade7e38617b6c420a45a84c4") };

            foo.Equals(bar)
                .Should().Be.False();
        }

        [Test]
        public void WhenBothGroupsAreTRansientAndNotRefEquals_ThenAreNotEquals()
        {
            var foo = new Group { Id = null };
            var bar = new Group { Id = null };

            foo.Equals(bar)
                .Should().Be.False();
        }
    }
}