using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Weather_WebAPI.Models
{
    /// <summary>
    /// something quick to indicate back to the client if their action was successful
    /// </summary>
    public class API_Response_Object
    {
        public API_Response_Object(bool success)
        {
            this.success = success;
        }

        public bool success { get; set; }
    }
}