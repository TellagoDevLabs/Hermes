using System;
using System.Linq;
using NUnit.Framework;
using TellagoStudios.Hermes.Logging;

namespace DataAccess.Tests.Repository
{
    public class MongoDBLogRepositoryFixture :  MongoDbBaseFixture
    {
        private ILogRepository repository;

        public override void FixtureSetUp()
        {
            base.FixtureSetUp();

            repository = new MongoDbLogRepository(connectionString);
        }

        [Test]
        public void Create_a_logentry()
        {
            var entry = new LogEntry
                            {
                                Message = "this is my message!",
                                UtcTs = DateTime.UtcNow,
                                Type = LogEntryType.Information
                            };
            var result = repository.Create(entry);

            Assert.IsNotNull(result.Id);
            Assert.That(result.Message, Is.EqualTo(entry.Message));
            Assert.That(result.UtcTs, Is.EqualTo(entry.UtcTs));
            Assert.That(result.Type, Is.EqualTo(entry.Type));
        }

        [Test]
        public void Get_a_logentry()
        {
            var entry = new LogEntry
            {
                Message = "this is my message!",
                UtcTs = DateTime.UtcNow,
                Type = LogEntryType.Information
            };
            var result = repository.Create(entry);

            var found = repository.Get(result.Id.Value);

            Assert.IsNotNull(result.Id);
            Assert.That(result.Id, Is.EqualTo(found.Id));
            Assert.That(result.Message, Is.EqualTo(found.Message));
            // It may have a few Ticks of difference
            Assert.IsTrue((result.UtcTs - found.UtcTs).Duration() < TimeSpan.FromMilliseconds(1));
            Assert.That(result.Type, Is.EqualTo(found.Type));
        }

        [Test]
        public void Truncate_log()
        {
            var entry = new LogEntry
            {
                Message = "this is my message!",
                UtcTs = DateTime.UtcNow,
                Type = LogEntryType.Information
            };
            repository.Create(entry);

            var entries1 = repository.Find(string.Empty, null, null).Count();

            repository.Truncate();

            var entries2 = repository.Find(string.Empty, null, null).Count();
            
            Assert.That(entries1 > 0);
            Assert.That(entries2 == 0);
        }
    }
}
