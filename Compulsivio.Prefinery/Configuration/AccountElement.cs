using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Compulsivio.Prefinery.Configuration
{
    /// <summary>
    /// &lt;account&gt; element from application config file.
    /// </summary>
    public class AccountElement : ConfigurationElement
    {
        /// <summary>
        /// Gets the Prefinery account name.
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true)]
        public string AccountName
        {
            get { return this["name"] as string; }
        }

        /// <summary>
        /// Gets the API key for the account.
        /// </summary>
        [ConfigurationProperty("apiKey", IsRequired = true)]
        public string ApiKey
        {
            get { return this["apiKey"] as string; }
        }
    }
}
