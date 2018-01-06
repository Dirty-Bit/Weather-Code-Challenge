using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Weather_Models;

namespace Local_Database.Tests
{
    [TestClass]
    public class UnitTest1
    {
        // define the connection
        Connection SQLConn;
        
        [TestInitialize]
        public void SetUp()
        {
            // instantiate the SQL server
            // it will connect when we are ready to make a query
            SQLConn = new Connection();
        }

        [TestMethod]
        public void GetAllLocations()
        {
            // get all the locations from the database
            List<Location> locations = SQLConn.GetAllLocations();

            // we should have a location in here for Phoenix
            Assert.IsTrue(locations.Any(x => x.Name == "Phoenix"));            
        }

        [TestMethod]
        public void GetLocation()
        {
            // get the location from the database
            Location location = SQLConn.GetLocation(5308655);

            // this should be Phoenix
            Assert.IsTrue(location != null && location.Name == "Phoenix");
        }

        [TestMethod]
        public void GetFakeLocation()
        {
            // attempt to get a non-existant location from the database
            Location location = SQLConn.GetLocation(-1);

            // this should not exist
            Assert.IsNull(location);
        }

        [TestMethod]
        public void AddAndRemoveLocation()
        {
            // a little more testing here than what would be desired

            // make sure we don't have LA already
            SQLConn.RemoveLocation(5368361);

            // create a real location
            Assert.IsTrue(SQLConn.AddLocation(new Location("5368361", "Los Angeles", "90210", "CA", "US")));

            // we just added it, now let's remove it again
            Assert.IsTrue(SQLConn.RemoveLocation(5368361));

            // try to get the LA location, it should be null
            Assert.IsNull(SQLConn.GetLocation(5368361));
        }

        [TestCleanup]
        public void TearDown()
        {
            // clean up the connection
            SQLConn.Disconnect();
        }
    }
}
