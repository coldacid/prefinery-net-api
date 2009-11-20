using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Compulsivio.Prefinery.Configuration
{
    /// <summary>
    /// Processes Prefinery settings stored in application's config file.
    /// </summary>
    public class PrefineryConfigHandler : ConfigurationSection
    {
        /// <summary>
        /// Gets the object representing the &lt;account&gt; element in the application config file.
        /// </summary>
        [ConfigurationProperty("account", IsRequired = true)]
        public AccountElement Account
        {
            get { return (AccountElement)this["account"]; }
        }

        /// <summary>
        /// Gets the object representing the &lt;betas&gt; element in the application config file.
        /// </summary>
        [ConfigurationProperty("betas")]
        [ConfigurationCollection(typeof(BetaCollection), AddItemName = "beta")]
        public BetaCollection Betas
        {
            get { return (BetaCollection)this["betas"]; }
        }
    }
}