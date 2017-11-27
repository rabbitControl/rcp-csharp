using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCP.Model
{
    public class RCPDataErrorException: Exception
    {
		public RCPDataErrorException(string message):base(message)
    	{}
    }

    public class RCPUnsupportedFeatureException: Exception
    {

    }
}
