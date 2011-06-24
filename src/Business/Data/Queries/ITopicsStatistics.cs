using System.Collections.Generic;

namespace TellagoStudios.Hermes.Business.Data.Queries
{
    public interface ITopicsStatistics
    {
        TopicStatisticsSingleResults Execute();
    }

    public class TopicsStatisticsResults
    {
        public TopicsStatisticsResults(
            IEnumerable<TopicStatisticsSingleResults> mostActiveThisWeek, 
            IEnumerable<TopicStatisticsSingleResults> mostActiveAllTime)
        {
            MostActiveThisWeek = mostActiveThisWeek;
            MostActiveAllTime = mostActiveAllTime;
        }

        public IEnumerable<TopicStatisticsSingleResults> MostActiveThisWeek { get; private set; }
        public IEnumerable<TopicStatisticsSingleResults> MostActiveAllTime { get; private set; }
    }

    public class TopicStatisticsSingleResults
    {
        public TopicStatisticsSingleResults(string name, int messageCount)
        {
            Name = name;
            MessageCount = messageCount;
        }

        public string Name      { get; private set; }
        public int MessageCount { get; private set; }
    }
}