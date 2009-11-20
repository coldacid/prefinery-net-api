using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Compulsivio.Prefinery.Configuration
{
    /// <summary>
    /// &lt;beta&gt; element from application config file.
    /// </summary>
    public class BetaElement : ConfigurationElement
    {
        /// <summary>
        /// Gets the identification number of the beta, as used by the Prefinery API.
        /// </summary>
        [ConfigurationProperty("id", IsRequired = true)]
        public int Id
        {
            get { return Convert.ToInt32(this["id"]); }
        }

        /// <summary>
        /// Gets the user-set beta name.
        /// </summary>
        [ConfigurationProperty("name", IsRequired = false)]
        public string Name
        {
            get { return this["name"] as string; }
        }

        /// <summary>
        /// Gets the decoding key for processing valid tester e-mail addresses.
        /// </summary>
        [ConfigurationProperty("decodeKey", IsRequired = true)]
        public string DecodeKey
        {
            get { return this["decodeKey"] as string; }
        }
    }
}
