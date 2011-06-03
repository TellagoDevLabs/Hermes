using System;
using Moq;
using NUnit.Framework;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Logging.Tests
{
    [TestFixture]
    public class LogServiceFixture
    {
        private LogService _logService;
        private Mock<ILogRepository> _mockedLogRepository;
       

        [TestFixtureSetUp]
        public void SetUpFixture()
        {            
            _mockedLogRepository = new Mock<ILogRepository>(MockBehavior.Loose);
            _logService = new LogService
            {
                Repository = _mockedLogRepository.Object
            };
        }
        
        [Test]        
        public void should_log_an_error()
        {            
            var entry = new LogEntry
            {
                Message = "this is my message!",
                Type = LogEntryType.Error,
                UtcTs = DateTime.Now
            };

            var id = Identity.Random();

            var createdEntry = new LogEntry
            {
                Id = id,
                Message = "this is my message!",
                Type = LogEntryType.Error,
                UtcTs = DateTime.Now
            };
                      
            _mockedLogRepository.Setup(logRepo => logRepo.Create(entry)).Returns(createdEntry);                

            var result = _logService.Create(entry);

            _mockedLogRepository.Verify(r => r.Create(entry));

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Id, createdEntry.Id);
            Assert.That(result.Type, Is.EqualTo(LogEntryType.Error));
        }

        [Test]        
        public void should_log_an_information()
        {
            var entry = new LogEntry
            {
                Message = "this is my message!",
                Type = LogEntryType.Information,
                UtcTs = DateTime.Now
            };

            var id = Identity.Random();

            var createdEntry = new LogEntry
            {
                Id = id,
                Message = "this is my message!",
                Type = LogEntryType.Information,
                UtcTs = DateTime.Now
            };


            _mockedLogRepository.Setup(logRepo => logRepo.Create(entry)).Returns(createdEntry);


            var result = _logService.Create(entry);

            _mockedLogRepository.Verify(r => r.Create(entry));

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Id, createdEntry.Id);
            Assert.That(result.Type, Is.EqualTo(LogEntryType.Information));
        }

        [Test]        
        public void should_log_a_warning()
        {
            var entry = new LogEntry
            {
                Message = "this is my message!",
                Type = LogEntryType.Warning,
                UtcTs = DateTime.Now
            };

            var id = Identity.Random();

            var createdEntry = new LogEntry
            {
                Id = id,
                Message = "this is my message!",
                Type = LogEntryType.Warning,
                UtcTs = DateTime.Now
            };

            _mockedLogRepository.Setup(logRepo => logRepo.Create(entry)).Returns(createdEntry);


            var result = _logService.Create(entry);

            _mockedLogRepository.Verify(r => r.Create(entry));

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Id, createdEntry.Id);
            Assert.That(result.Type, Is.EqualTo(LogEntryType.Warning));
        }

        [Test]        
        public void should_find_a_logentry()
        {
            var entry = new LogEntry
            {
                Message = "this is my message!",
                Type = LogEntryType.Warning,
                UtcTs = DateTime.Now
            };

            var id = Identity.Random();

            var createdEntry = new LogEntry
            {
                Id = id,
                Message = "this is my message!",
                Type = LogEntryType.Warning,
                UtcTs = DateTime.Now
            };

            _mockedLogRepository.Setup(logRepo => logRepo.Create(entry)).Returns(createdEntry);
            _mockedLogRepository.Setup(logRepo => logRepo.Get(id)).Returns(createdEntry);

            var result = _logService.Create(entry);
            _mockedLogRepository.Verify(r => r.Create(entry));

            var foundEntry = _logService.Get(id);
            _mockedLogRepository.Verify(r => r.Get(id));
                      
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Id, createdEntry.Id);
            Assert.AreSame(result, foundEntry);
            Assert.That(result.Type, Is.EqualTo(LogEntryType.Warning));
        }

        [Test]
        public void should_truncate_log()
        {
            var entry = new LogEntry
            {
                Message = "this is my message!",
                Type = LogEntryType.Warning,
                UtcTs = DateTime.Now
            };

            var id = Identity.Random();

            var createdEntry = new LogEntry
            {
                Id = id,
                Message = "this is my message!",
                Type = LogEntryType.Warning,
                UtcTs = DateTime.Now
            };

            _mockedLogRepository.Setup(logRepo => logRepo.Create(entry)).Returns(createdEntry);

            _logService.Create(entry);
            _mockedLogRepository.Verify(r => r.Create(entry));

            _logService.Truncate();
            _mockedLogRepository.Verify(r => r.Create(entry));
        }
    }
}
