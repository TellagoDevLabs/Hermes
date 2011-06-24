using System.Linq;
using MongoDB.Bson;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.DataAccess.MongoDB.Queries
{
    public class TopicsStatistics : MongoDbRepository, ITopicsStatistics
    {
        public TopicsStatistics(string connectionString) : base(connectionString)
        {
        }

        #region ITopicsStatistics Members

        public TopicsStatisticsResults Execute()
        {
            var queryResult = DB.GetCollectionByType<Topic>()
                .MapReduce("function() { var getCount = 'db.msg_' + this._id + '.count()'; emit( this._id, { name: this.Name, count: eval(getCount) } );}", 
                           "function(key, vals) { var ret = { name: vals[0].name, count: vals[0].count }; return ret; }");
            var result = queryResult.InlineResults.SelectMany(b => b.Values
                                                                       .OfType<BsonDocument>()
                                                                       .Select(bd => new TopicStatisticsSingleResults
                                                                                         (bd["name"].AsString, (int)bd["count"].AsDouble)))
                .OrderByDescending(r => r.MessageCount)
                .Take(10).ToList();

            return new TopicsStatisticsResults(result, result);
        }

        #endregion
    }
}