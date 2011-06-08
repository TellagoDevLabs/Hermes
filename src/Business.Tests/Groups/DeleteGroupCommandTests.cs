using Business.Tests.Util;
using Moq;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Groups;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.Business.Data.Queries;

namespace Business.Tests.Groups
{
    [TestFixture]
    public class DeleteGroupCommandTests 
    {
        public DeleteGroupCommand CreateCommand(IEntityById queryEntityById = null, 
                                                IRepository<Group> cudGroup = null, 
                                                IChildGroupsOfGroup childGroupsOfGroup = null, 
                                                ITopicsByGroup queryTopicsByGroup = null) 
        {
            return new DeleteGroupCommand(
                                queryEntityById ?? Mock.Of<IEntityById>(), 
                                cudGroup ?? Mock.Of<IRepository<Group>>(),
                                childGroupsOfGroup ?? Mock.Of<IChildGroupsOfGroup>(),
                                queryTopicsByGroup ?? Mock.Of<ITopicsByGroup>());    
        }

        [Test]
        public void WhenGroupDoesNotExist_ThenThrowEntityNotFound()
        {
            var command = CreateCommand(Mock.Of<IEntityById>(q => q.Exist<Group>(It.IsAny<Identity>()) == false));

            command.Executing(c => c.Execute(Identity.Random(12)))
                .Throws<EntityNotFoundException>();
        }

        [Test]
        public void WhenGroupHasChildGroups_ThenThrow()
        {
            var command = CreateCommand(queryEntityById: Mock.Of<IEntityById>(q => q.Exist<Group>(It.IsAny<Identity>()) == true),
                                        childGroupsOfGroup: Mock.Of<IChildGroupsOfGroup>(qg => qg.HasChilds(It.IsAny<Identity>()) == true));
            var groupId = Identity.Random(12);

            command.Executing(c => c.Execute(groupId))
                .Throws<ValidationException>()
                .And.Exception.Message.Should().Be.EqualTo(string.Format(Messages.GroupContainsChildGroups, groupId));
        }

        [Test]
        public void WhenGroupHasChildTopics_ThenThrow()
        {
            var command = CreateCommand(queryEntityById: Mock.Of<IEntityById>(q => q.Exist<Group>(It.IsAny<Identity>()) == true),
                                        queryTopicsByGroup: Mock.Of<ITopicsByGroup>(qg => qg.HasTopics(It.IsAny<Identity>()) == true));

            var groupId = Identity.Random(12);

            command.Executing(c => c.Execute(groupId))
                .Throws<ValidationException>()
                .And.Exception.Message.Should().Be.EqualTo(string.Format(Messages.GroupContainsChildTopics, groupId));
        }

        [Test]
        public void WhenGroupExists_ThenDelete()
        {
            var groupId = Identity.Random(12);
            var repository = new StubRepository<Group>(new Group { Id = groupId});
            var command = CreateCommand(Mock.Of<IEntityById>(q => q.Exist<Group>(It.IsAny<Identity>()) == true),
                                        repository);
              
            command.Execute(groupId);
                
            repository.Entities.Should().Be.Empty();
        }
    }
}