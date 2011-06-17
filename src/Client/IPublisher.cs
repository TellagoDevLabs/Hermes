using System;
using TellagoStudios.Hermes.Facade;

namespace TellagoStudios.Hermes.Client
{
    public interface IPubliser
    {
        void CreateTopic(TopicPost topicPost);        
        void UpdateTopic(TopicPut topicPut);
        void DeleteTopic(Identity topicId);
        Topic[] GetTopicsByGroup(Identity groupId);
        
        void CreateGroup(GroupPost groupPost);
        void UpdateGroup(GroupPut groupPut);
        void DeleteGroup(Identity groupId);
        Group[] GetGroups();

        Uri PostMessage(Message message);
    }
}