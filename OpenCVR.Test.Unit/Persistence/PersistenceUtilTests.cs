using System;
using NUnit.Framework;
using OpenCVR.Persistence;

namespace OpenCVR.Test.Unit.Persistence
{
    [TestFixture]
    class PersistenceUtilTests
    {
        [Test]
        public void TestUnixTime()
        {
            DateTime original = DateTime.Today;
            var converted = PersistenceUtil.MillisecondsSinceEpochToDateTime(PersistenceUtil.DateTimeToMillisecondsSinceEpoch(original));
            Assert.AreEqual(original, converted);
        }

        [Test]
        public void TestUnixTimeStoresMillisecondsAsWell()
        {
            DateTime original = new DateTime(1, 2, 3, 4, 5, 6, DateTimeKind.Utc).AddMilliseconds(7);
            var converted = PersistenceUtil.MillisecondsSinceEpochToDateTime(PersistenceUtil.DateTimeToMillisecondsSinceEpoch(original));
            Assert.AreEqual(original, converted);
        }
    }
}
