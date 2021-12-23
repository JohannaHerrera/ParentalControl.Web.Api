using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParentalControl.Web.Api.Constants
{
    public class AppConstants
    {
        public bool Access
        {
            get
            {
                return false;
            }
        }

        public bool NoAccess
        {
            get
            {
                return true;
            }
        }
    }
}