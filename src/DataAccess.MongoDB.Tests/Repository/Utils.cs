using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using TellagoStudios.Hermes.Business.Model;

namespace DataAccess.Tests.Repository
{
    static class Utils
    {
        /// <summary>
        /// The function determines whether the current operating system is a 
        /// 64-bit operating system.
        /// </summary>
        /// <returns>
        /// The function returns true if the operating system is 64-bit; 
        /// otherwise, it returns false.
        /// </returns>
        public static bool Is64BitOperatingSystem()
        {
            if (IntPtr.Size == 8)  // 64-bit programs run only on Win64
            {
                return true;
            }
            else  // 32-bit programs run on both 32-bit and 64-bit Windows
            {
                // Detect whether the current process is a 32-bit process 
                // running on a 64-bit system.
                bool flag;
                return ((DoesWin32MethodExist("kernel32.dll", "IsWow64Process") &&
                    IsWow64Process(GetCurrentProcess(), out flag)) && flag);
            }
        }

        /// <summary>
        /// The function determins whether a method exists in the export 
        /// table of a certain module.
        /// </summary>
        /// <param name="moduleName">The name of the module</param>
        /// <param name="methodName">The name of the method</param>
        /// <returns>
        /// The function returns true if the method specified by methodName 
        /// exists in the export table of the module specified by moduleName.
        /// </returns>
        static bool DoesWin32MethodExist(string moduleName, string methodName)
        {
            IntPtr moduleHandle = GetModuleHandle(moduleName);
            if (moduleHandle == IntPtr.Zero)
            {
                return false;
            }
            return (GetProcAddress(moduleHandle, methodName) != IntPtr.Zero);
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr GetModuleHandle(string moduleName);

        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule,
            [MarshalAs(UnmanagedType.LPStr)]string procName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWow64Process(IntPtr hProcess, out bool wow64Process);

        static public void AreEquivalent(Dictionary<string, string> target, Dictionary<string, string> other)
        {
            if (target == null && other == null) return;
            Assert.IsTrue(target != null && other != null);
            Assert.AreEqual(target.Count, other.Count);
            foreach (var item in target)
            {
                Assert.IsTrue(other.ContainsKey(item.Key));
                Assert.AreEqual(item.Value.ToBsonValue().ToJson(), other[item.Key]);
            }
        }

        static public void AreEquivalent<T, U>(Dictionary<T, U[]> target, Dictionary<T, U[]> other)
        {
            if (target == null && other == null) return;
            Assert.IsTrue(target != null && other != null);
            Assert.AreEqual(target.Count, other.Count);
            foreach (var item in target)
            {
                Assert.IsTrue(other.ContainsKey(item.Key));
                var otherItemValue = other[item.Key];
                if (item.Value != null)
                {
                    CollectionAssert.AreEqual(item.Value, otherItemValue);
                }
            }
        }

        static public string RandomString(string prefix = null)
        {
            return (prefix ?? "") + Guid.NewGuid().ToString("N");
        }

        static public string RandomQuery(string prefix = null)
        {
            var doc = new QueryDocument((prefix ?? "") + "Field", Guid.NewGuid().ToString("N"));
            return doc.ToJson();
        }

        static public string RandomUri(string prefix = null)
        {
            return "http://localhost/random/" + (prefix ?? "") + Identity.Random(Utils.MongoObjectId);
        }

        static public IEnumerable<byte> MongoObjectId()
        {
            var id = ObjectId.GenerateNewId();
            return ObjectId.Pack(id.Timestamp, id.Machine, id.Pid, id.Increment);
        }
    }
}
