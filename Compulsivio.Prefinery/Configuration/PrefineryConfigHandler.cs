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
        /// The &lt;account&gt; element in the application config file.
        /// </summary>
        [ConfigurationProperty("account", IsRequired = true)]
        public AccountElement Account
        {
            get { return (AccountElement)this["account"]; }
        }

        [ConfigurationProperty("betas")]
        [ConfigurationCollection(typeof(BetaCollection), AddItemName = "beta")]
        public BetaCollection Betas
        {
            get { return ((BetaCollection)(this["betas"])); }
        }
    }
}