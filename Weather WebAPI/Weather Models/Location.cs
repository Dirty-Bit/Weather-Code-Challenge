using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Weather_Models
{
    public class Location
    {
        public Location() { }

        /// <summary>
        /// Create a location object, this is used for SQL results
        /// </summary>
        /// <param name="Id">string representation of the Id of a location</param>
        /// <param name="Name">the name of the location in the database</param>
        /// <param name="Zipcode">a string representation of the zipcode from the database</param>
        /// <param name="State">2 character string for the state</param>
        /// <param name="Country">2 character string for the country</param>
        public Location(string Id, string Name, string Zipcode, string State, string Country)
        {
            // initialize with an invalid ID
            // which will be a pessimistic look
            // at the validation
            this.Id = -1;

            // validate the Id
            if (string.IsNullOrEmpty(Id))
                return;
            
            // parse the Id into an int, and validates
            int _id = -1;
            if (!int.TryParse(Id, out _id))
                return;

            // looks good
            this.Id = _id;
            this.Name = Name;
            this.Zipcode = Zipcode;
            this.State = State;
            this.Country = Country;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Zipcode { get; set; }
        public string State { get; set; }
        public string Country { get; set; }

        /// <summary>
        /// make sure that we have a valid location object
        /// that will be accepted by our Local SQL Database
        /// </summary>
        /// <returns>true/false: whether or not this is valid</returns>
        public bool CanAddToDB()
        {
            // check all the criteria we have

            // make sure the ID is not -1
            if (Id == -1)
                return false;

            // we need to have a name
            if (string.IsNullOrEmpty(Name))
                return false;

            // we need to have a state
            if (string.IsNullOrEmpty(State))
                return false;

            // we have passed the tests, we can add this
            return true;
        }
    }
}