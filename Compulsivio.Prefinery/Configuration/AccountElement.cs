using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Compulsivio.Prefinery.Configuration
{
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
