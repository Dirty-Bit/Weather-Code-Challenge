using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Local_Redis.Test
{
    [TestClass]
    public class Redis_Tests
    {
        private static Redis_Connection redis;
        private static string test_key;

        [TestInitialize]
        public void SetUp()
        {
            // instantiate redis
            redis = new Redis_Connection();

            // create a random and non-repeatable key for this test
            test_key = Guid.NewGuid().ToString("N");
        }

        [TestMethod]
        public void RedisSetAndExists()
        {
            // set something for the key
            redis.set(test_key, "{ test: true }");

            // this key should exist
            Assert.IsTrue(redis.exists(test_key));
        }

        [TestMethod]
        public void RedisSetAndGet()
        {
            // create an object to return
            string set_value = "{ test: true }";

            // set something for the key
            redis.set(test_key, set_value);

            // get the value
            string get_value = redis.get(test_key);

            // the value that is returned should be the same as the value that was set
            Assert.IsTrue(get_value == set_value);
        }

        [TestMethod]
        public void RedisSetAndDelete()
        {
            // set something for the key
            redis.set(test_key, "{ test: true }");

            // delete from the redis cache
            redis.delete(test_key);

            // this key should not exist
            Assert.IsTrue(!redis.exists(test_key));
        }

        [TestMethod]
        public void RedisExpiration()
        {
            // set something for the key with a 5 sec time to live
            redis.set(test_key, "{ test: true }", .083);

            // it should exist now
            Assert.IsTrue(redis.exists(test_key));

            // wait for over 5sec
            Thread.Sleep(7500);

            // this key should not exist
            Assert.IsTrue(!redis.exists(test_key));
        }

        [TestCleanup]
        public void TearDown()
        {
            redis.Dispose();
        }
    }
}
