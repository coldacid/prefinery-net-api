using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compulsivio.Prefinery
{
    public class PrefineryException : ApplicationException
    {
        public PrefineryException(string message)
            : base(message)
        {
        }
        public PrefineryException(Exception innerException)
            : base("See inner exception", innerException)
        {
        }
        public PrefineryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
