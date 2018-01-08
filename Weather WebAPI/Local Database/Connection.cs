using Local_Redis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Weather_Models;

namespace Local_Database
{
    public class Connection
    {
        // define the SQL connection
        private SqlConnection conn;

        // find a home for our SQL server
        private string SQLPath;

        /// <summary>
        /// create the database, this will ensure that we don't have
        /// any issues with the |DataDirectory|
        /// between our test projects and ASP.NET project
        /// </summary>
        public Connection()
        {
            // get a temp folder
            SQLPath = Path.GetTempPath();

            // create the database
            CreateDatabase();
        }

        /// <summary>
        /// method to create the database "Locations"
        /// </summary>
        private void CreateDatabase()
        {
            // create a database name, strips the extension
            string DBName = "LocationDB";

            // create the database file path, full with extension
            string DBPath = string.Format("{0}{1}.mdf", SQLPath, DBName);
            string LogPath = string.Format("{0}{1}_log.ldf", SQLPath, DBName);

            // if the database exists, delete it
            if (File.Exists(DBPath))
            {
                // delete the database
                File.Delete(DBPath);
            }

            // if the log file exists, delete it
            if (File.Exists(LogPath))
            {
                // delete the logs
                File.Delete(LogPath);
            }

            // create a connection to build the database, if necessary
            conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True;Initial Catalog=master");
            
            // open the connection
            conn.Open();

            // check to see if the database exists
            using (SqlCommand command = new SqlCommand("SELECT database_id FROM sys.databases WHERE Name=@p0", conn))
            {
                // add the param
                command.Parameters.AddWithValue("@p0", DBName);

                // check to see if the database exists
                if (command.ExecuteScalar() == null)
                {
                    // we do not have the database, we need to create it

                    // reset the command with the create database values
                    command.Parameters.Clear();
                    command.CommandText = string.Format("CREATE DATABASE {0} ON PRIMARY (NAME={0}, FILENAME=\"{1}\")", DBName, DBPath);

                    // execute the command to create the database
                    command.ExecuteNonQuery();

                    // check to see if the table exists
                    command.Parameters.Clear();
                    command.CommandText = "SELECT COUNT (*) FROM information_schema.tables WHERE table_name = 'locations'";

                    // check to see if we have the table
                    if ((int)command.ExecuteScalar() == 0)
                    {
                        // next, we need to build the locations table

                        // reset the command, and add the command text to build the locations table
                        command.Parameters.Clear();
                        command.CommandText = "CREATE TABLE [locations] ([Id] INT NOT NULL, [Name] NVARCHAR(50) NOT NULL, [Zipcode] INT NULL, [State] NVARCHAR(2)  NOT NULL, [Country] NVARCHAR(2) NULL, PRIMARY KEY CLUSTERED([Id] ASC))";

                        // execute the command to create the table
                        command.ExecuteNonQuery();
                    }
                    
                    // add some initial values for Phoenix and Denver
                    AddLocation(new Location("5308655", "Phoenix", "85044", "AZ", "US"));
                    AddLocation(new Location("5419384", "Denver", "80014", "CO", "US"));
                }
            }            
        }

        /// <summary>
        /// initialize a connection to the database
        /// </summary>
        private void ConnectToDatabase()
        {
            try
            {
                // build the connection
                //  having |DataDirectory| issue => skipping for now
                conn = new SqlConnection(string.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0}\LocationDB.mdf;Integrated Security=True;User Instance=False", SQLPath));

                // open the connection
                conn.Open();
            }
            catch (SqlException e)
            {
                // we failed to create the connection
                conn = null;

                throw e;
            }
        }

        /// <summary>
        /// Make sure that we have successfully connected
        /// to the database so we can make our queries
        /// </summary>
        private void EnsureConnection()
        {
            try
            {
                // check to see if the conn is null, if so we need to connect
                if (conn == null)
                    ConnectToDatabase();

                // make sure that we are connected, otherwise continue
                // as we have already connected
                if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)
                    ConnectToDatabase();

                // if the connection is not yet open, wait
                if (conn.State != ConnectionState.Open)
                    Thread.Sleep(100);
            }
            catch (Exception e)
            {
                // we failed to connect
                // sleep and try again
                Thread.Sleep(1000);

                // try again
                EnsureConnection();
            }
        }

        /// <summary>
        /// disconnect from the local database
        /// </summary>
        public void Disconnect()
        {
            conn.Close();
        }

        /// <summary>
        /// Connect to the database to get all locations that are stored
        /// </summary>
        /// <returns></returns>
        public List<Location> GetAllLocations()
        {
            // initialize with an empty list
            List<Location> locations = new List<Location>();

            // make sure that we are connected
            EnsureConnection();

            // make a query to get all locations
            using (SqlCommand command = new SqlCommand("SELECT * FROM locations", conn))
            {
                // make the query
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    // make sure we have rows
                    if (!reader.HasRows)
                        return null;

                    // read until we don't have any data left
                    while (reader.Read())
                    {
                        // extract the current row
                        // into the constructor for a new location
                        Location location = new Location(reader["Id"].ToString(), reader["Name"].ToString(), reader["Zipcode"].ToString(), reader["State"].ToString(), reader["Country"].ToString());

                        // make sure we were valid
                        if (location.Id == -1)
                            continue;

                        // valid, add to the list
                        locations.Add(location);
                    }
                }
            }

            // return our list
            return locations;
        }

        /// <summary>
        /// Retrieve a single location from the database
        /// </summary>
        /// <param name="Id">the Id of the location to get</param>
        /// <returns>a location object, null if not found</returns>
        public Location GetLocation(int Id)
        {
            // make sure that we are connected
            EnsureConnection();

            // make a query to get the location specified
            using (SqlCommand command = new SqlCommand("SELECT * FROM locations WHERE Id=@p0", conn))
            {
                // add the parameters
                command.Parameters.AddWithValue("@p0", Id);

                // make the query
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    // make sure we have rows
                    if (!reader.HasRows)
                        return null;

                    // read until we don't have any data left
                    // there should only be one here...
                    while (reader.Read())
                    {
                        // extract the current row
                        // into the constructor for a new location
                        Location location = new Location(reader["Id"].ToString(), reader["Name"].ToString(), reader["Zipcode"].ToString(), reader["State"].ToString(), reader["Country"].ToString());

                        // make sure we were valid
                        if (location.Id == -1)
                            continue;

                        // valid, return it
                        return location;
                    }
                }
            }

            // return null 
            return null;
        }

        /// <summary>
        /// Add a location to the database
        /// </summary>
        /// <param name="location">Object defining the location to add to the database</param>
        /// <returns>true/false: whether or not the INSERT was successful</returns>
        public bool AddLocation(Location location)
        {
            // validate that we have a location that is acceptable for the database
            if (!location.CanAddToDB())
                return false;

            // make sure we are connected
            EnsureConnection();

            // we can add this, build the query
            using (SqlCommand command = new SqlCommand("INSERT INTO locations (Id, Name, Zipcode, State, Country) VALUES (@p0, @p1, @p2, @p3, @p4)", conn))
            {
                // add the parameters
                command.Parameters.AddWithValue("@p0", location.Id);
                command.Parameters.AddWithValue("@p1", location.Name);
                command.Parameters.AddWithValue("@p2", location.Zipcode);
                command.Parameters.AddWithValue("@p3", location.State);
                command.Parameters.AddWithValue("@p4", location.Country);

                // return true if the number of rows affected is 1
                return command.ExecuteNonQuery() == 1; ;
            }
        }

        /// <summary>
        /// Remove a location from the database
        /// </summary>
        /// <param name="Id">The Id of the location to remove</param>
        /// <returns>true/false: whether or not the DELETE was successful</returns>
        public bool RemoveLocation(int Id)
        {
            // make sure we are connected
            EnsureConnection();

            // build the query to remove
            using (SqlCommand command = new SqlCommand("DELETE FROM locations WHERE Id=@p0", conn))
            {
                // add the parameters
                command.Parameters.AddWithValue("@p0", Id);

                // execute the non-query and set true
                // if the number of rows affected is 1
                // return whether or not we changed something
                return command.ExecuteNonQuery() == 1;
            }
        }
    }
}
