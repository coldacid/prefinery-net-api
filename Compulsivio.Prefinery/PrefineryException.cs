using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compulsivio.Prefinery
{
    /// <summary>
    /// Exceptions caused by conditions in the Prefinery API.
    /// </summary>
    public class PrefineryException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the PrefineryException class.
        /// </summary>
        /// <param name="message">The error message from the API.</param>
        public PrefineryException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the PrefineryException class.
        /// </summary>
        /// <param name="innerException">The exception responsible for this one.</param>
        public PrefineryException(Exception innerException)
            : base("See inner exception", innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the PrefineryException class.
        /// </summary>
        /// <param name="message">The error message from the API.</param>
        /// <param name="innerException">The exception responsible for this one.</param>
        public PrefineryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
