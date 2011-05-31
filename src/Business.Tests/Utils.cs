using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using TellagoStudios.Hermes.Business.Model;

namespace Business.Tests
{
    static class Utils
    {
        static public void AreEquivalent<T, U>(Dictionary<T, U[]> target, Dictionary<T, U[]> other)
        {
            if (target == null && other == null) return;
            Assert.IsTrue(target != null && other != null);
            Assert.AreEqual(target.Count, other.Count);
            foreach (var item in target)
            {
                Assert.IsTrue(other.ContainsKey(item.Key));
                if (target.Values != null || other.Values != null)
                {
                    Assert.IsTrue(target.Values != null && other.Values != null);
                    Assert.AreEqual(target.Values.Count, other.Values.Count);
                    foreach (var value in target.Values)
                    {
                        Assert.Contains(value, other.Values);
                    }
                }
            }
        }

        static public string RandomString(string prefix = null)
        {
            return (prefix ?? "") + Identity.Random();
        }

        static public string RandomQuery(string prefix = null)
        {
            var doc = new QueryDocument((prefix ?? "") + "Field", Guid.NewGuid().ToString("N"));
            return doc.ToJson();
        }

        static public string RandomUri(string prefix = null)
        {
            return "http://localhost/random/" + (prefix ?? "") + Identity.Random();
        }
    }
}
