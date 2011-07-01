using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.DataAccess.MongoDB.Queries
{
    public class GetWorkingFeedForTopic : MongoDbRepository, IGetWorkingFeedForTopic
    {
        private readonly MongoCollection<Feed> feeds;
        private readonly object lck = new object();

        public GetWorkingFeedForTopic(string connectionString) : base(connectionString)
        {
            feeds = DB.GetCollectionByType<Feed>();

        }

        public Feed Execute(Identity topicId)
        {
            Feed workingFeed;
            lock (lck)
            {
                workingFeed = feeds.Find(Query.EQ("TopicId", topicId.ToBson()))
                    .SetSortOrder(SortBy.Descending("Updated"))
                    .SetLimit(1).FirstOrDefault();

                if (workingFeed == null || workingFeed.Entries.Count >= 10)
                {
                    var newFeed = new Feed
                                 {
                                     TopicId = topicId,
                                 };

                    if(workingFeed != null)
                    {
                        newFeed.PreviousFeed = workingFeed.Id;
                        feeds.Insert(newFeed);
                        workingFeed.NextFeed = newFeed.Id;
                        feeds.Save(workingFeed);
                    }else
                    {
                        feeds.Insert(newFeed);    
                    }
                    workingFeed = newFeed;
                }    
            }
            return workingFeed;
        }
    }
}