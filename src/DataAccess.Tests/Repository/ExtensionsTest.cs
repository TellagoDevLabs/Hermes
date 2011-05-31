using System;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;

namespace DataAccess.Tests.Repository
{
    [TestFixture]
    public class ExtensionsFixture
    {
        [Test]
        public void ToQueryDocReturnsNullForNullString()
        {
            Assert.Null(((string)null).ToQueryDocument());
        }

        [Test]
        public void ToQueryDocReturnsNullForEmptyString()
        {
            Assert.Null("".ToQueryDocument());
        }

        [Test]
        public void ToQueryDocReturnsNullForWhitespace()
        {
            Assert.Null("   ".ToQueryDocument());
        }

        [Test]
        public void ToQueryDocParsesCorrectly()
        {
            var str = "{name: \"bob\", occupation: \"developer\"} ";
            var doc = str.ToQueryDocument();
            Assert.That(doc["name"].AsString, Is.EqualTo("bob"));
            Assert.That(doc["occupation"].AsString, Is.EqualTo("developer"));
        }
    }
}