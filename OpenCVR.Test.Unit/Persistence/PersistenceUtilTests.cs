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
            var converted = PersistenceUtil.UnixTimeStampToDateTime(PersistenceUtil.DateTimeToUnixTimestamp(original));
            Assert.AreEqual(original, converted);
        }
    }
}
