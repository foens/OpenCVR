using System;
using System.Collections.Generic;
using System.Data.Common;
using NUnit.Framework;
using OpenCVR.Persistence;

namespace OpenCVR.Test.Integration
{
    [Ignore]
    [TestFixture]
    public class PersistencePerformanceTest
    {
        private readonly string[] queries = new[] { "j", "j.", "j. ", "j. skriver", "kasper", "A/S", "q", "ab", "12", "145", "3313", "compugroup" };
        private DbConnection connection;

        [SetUp]
        public void Setup()
        {
            connection = PersistenceUtil.CreateConnection(@"Data Source=..\..\cvr.sqlite;Version=3;");
            connection.Open();
        }

        [TearDown]
        public void TearDown()
        {
            connection.Dispose();
        }

        [Test]
        public void TestLikeQueryPerformance()
        {
            double total = 0;
            foreach (var query in queries)
            {
                var start = DateTime.Now;
                using (var command = PersistenceUtil.CreateCommand(connection, "SELECT * FROM Company WHERE Vat LIKE @vat OR Name LIKE @name", new Dictionary<string, object>
            {
                {"@vat", query + "%"},
                {"@name", query + "%"}
            }))
                {
                    using (var r = command.ExecuteReader())
                    {
                        while (r.HasRows && r.Read())
                        {

                        }
                    }
                }
                var end = DateTime.Now;
                var milliseconds = (end - start).TotalMilliseconds;
                total += milliseconds;
                Console.WriteLine("Query " + query + " took " + milliseconds);
            }
            Console.WriteLine("Total " + total);
        }

        [Test]
        public void TestLikeQueryPerformanceWithLimit()
        {
            double total = 0;
            foreach (var query in queries)
            {
                var start = DateTime.Now;
                using (var command = PersistenceUtil.CreateCommand(connection, "SELECT * FROM Company WHERE Vat LIKE @vat OR Name LIKE @name LIMIT 1", new Dictionary<string, object>
            {
                {"@vat", query + "%"},
                {"@name", query + "%"}
            }))
                {
                    using (var r = command.ExecuteReader())
                    {
                        while (r.HasRows && r.Read())
                        {

                        }
                    }
                }
                var end = DateTime.Now;
                var milliseconds = (end - start).TotalMilliseconds;
                total += milliseconds;
                Console.WriteLine("Query " + query + " took " + milliseconds);
            }
            Console.WriteLine("Total " + total);
        }

        [Test]
        public void TestRangeQueryPerformanceWithLimit()
        {
            double total = 0;
            foreach (var query in queries)
            {
                var start = DateTime.Now;
                using (var command = PersistenceUtil.CreateCommand(connection, "SELECT * FROM Company WHERE (Vat > @vatStart AND Vat < @vatEnd) OR (Name > @nameStart AND Name < @nameEnd) LIMIT 1", new Dictionary<string, object>
            {
                {"@vatStart", query},
                {"@vatEnd", query + "}"},
                {"@nameStart", query},
                {"@nameEnd", query + "}"}
            }))
                {
                    using (var r = command.ExecuteReader())
                    {
                        while (r.HasRows && r.Read())
                        {

                        }
                    }
                }
                var end = DateTime.Now;
                var milliseconds = (end - start).TotalMilliseconds;
                total += milliseconds;
                Console.WriteLine("Query " + query + " took " + milliseconds);
            }
            Console.WriteLine("Total " + total);
        }

        [Test]
        public void TestRangeQueryPerformance()
        {
            double total = 0;
            foreach (var query in queries)
            {
                var start = DateTime.Now;
                using (var command = PersistenceUtil.CreateCommand(connection, "SELECT * FROM Company WHERE (Vat > @vatStart AND Vat < @vatEnd) OR (Name > @nameStart AND Name < @nameEnd)", new Dictionary<string, object>
            {
                {"@vatStart", query},
                {"@vatEnd", query + "}"},
                {"@nameStart", query},
                {"@nameEnd", query + "}"}
            }))
                {
                    using (var r = command.ExecuteReader())
                    {
                        while (r.HasRows && r.Read())
                        {

                        }
                    }
                }
                var end = DateTime.Now;
                var milliseconds = (end - start).TotalMilliseconds;
                total += milliseconds;
                Console.WriteLine("Query " + query + " took " + milliseconds);
            }
            Console.WriteLine("Total " + total);
        }

        [Test]
        public void TestBetweenQueryPerformanceWithNocase()
        {
            double total = 0;
            foreach (var query in queries)
            {
                var start = DateTime.Now;
                using (var command = PersistenceUtil.CreateCommand(connection, "SELECT * FROM Company WHERE (Vat COLLATE NOCASE BETWEEN @vatStart AND @vatEnd || '~~~~~~~~~~~~~~') OR (Name COLLATE NOCASE BETWEEN @nameStart AND  @nameEnd || '~~~~~~~~~~~~~~')", new Dictionary<string, object>
            {
                {"@vatStart", query},
                {"@vatEnd", query + "}"},
                {"@nameStart", query},
                {"@nameEnd", query + "}"}
            }))
                {
                    using (var r = command.ExecuteReader())
                    {
                        while (r.HasRows && r.Read())
                        {

                        }
                    }
                }
                var end = DateTime.Now;
                var milliseconds = (end - start).TotalMilliseconds;
                total += milliseconds;
                Console.WriteLine("Query " + query + " took " + milliseconds);
            }
            Console.WriteLine("Total " + total);
        }

        [Test]
        public void TestBetweenQueryPerformance()
        {
            double total = 0;
            foreach (var query in queries)
            {
                var start = DateTime.Now;
                using (var command = PersistenceUtil.CreateCommand(connection, "SELECT * FROM Company WHERE (Vat BETWEEN @vatStart AND @vatEnd || '~~~~~~~~~~~~~~') OR (Name BETWEEN @nameStart AND  @nameEnd || '~~~~~~~~~~~~~~')", new Dictionary<string, object>
            {
                {"@vatStart", query},
                {"@vatEnd", query + "}"},
                {"@nameStart", query},
                {"@nameEnd", query + "}"}
            }))
                {
                    using (var r = command.ExecuteReader())
                    {
                        while (r.HasRows && r.Read())
                        {

                        }
                    }
                }
                var end = DateTime.Now;
                var milliseconds = (end - start).TotalMilliseconds;
                total += milliseconds;
                Console.WriteLine("Query " + query + " took " + milliseconds);
            }
            Console.WriteLine("Total " + total);
        }

        [Test]
        public void TestBetweenQueryPerformanceWithNocaseAndLimit()
        {
            double total = 0;
            foreach (var query in queries)
            {                
                var start = DateTime.Now;
                using (var command = PersistenceUtil.CreateCommand(connection, "SELECT * FROM Company WHERE (Vat COLLATE NOCASE BETWEEN @vatStart AND @vatEnd || '~~~~~~~~~~~~~~') OR (Name COLLATE NOCASE BETWEEN @nameStart AND  @nameEnd || '~~~~~~~~~~~~~~') LIMIT 1", new Dictionary<string, object>
            {
                {"@vatStart", query},
                {"@vatEnd", query + "}"},
                {"@nameStart", query},
                {"@nameEnd", query + "}"}
            }))
                {
                    using (var r = command.ExecuteReader())
                    {
                        while (r.HasRows && r.Read())
                        {

                        }
                    }
                }
                var end = DateTime.Now;
                var milliseconds = (end - start).TotalMilliseconds;
                total += milliseconds;
                Console.WriteLine("Query " + query + " took " + milliseconds);
            }
            Console.WriteLine("Total " + total);
        }

        [Test]
        public void TestBetweenQueryPerformanceWithLimit()
        {
            double total = 0;
            foreach (var query in queries)
            {
                var start = DateTime.Now;
                using (var command = PersistenceUtil.CreateCommand(connection, "SELECT * FROM Company WHERE (Vat BETWEEN @vatStart AND @vatEnd || '~~~~~~~~~~~~~~') OR (Name BETWEEN @nameStart AND  @nameEnd || '~~~~~~~~~~~~~~') LIMIT 1", new Dictionary<string, object>
            {
                {"@vatStart", query},
                {"@vatEnd", query + "}"},
                {"@nameStart", query},
                {"@nameEnd", query + "}"}
            }))
                {
                    using (var r = command.ExecuteReader())
                    {
                        while (r.HasRows && r.Read())
                        {

                        }
                    }
                }
                var end = DateTime.Now;
                var milliseconds = (end - start).TotalMilliseconds;
                total += milliseconds;
                Console.WriteLine("Query " + query + " took " + milliseconds);
            }
            Console.WriteLine("Total " + total);
        }
    }
}
