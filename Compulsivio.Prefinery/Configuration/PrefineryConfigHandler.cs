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
        /// The &lt;beta&gt; element in the application config file.
        /// </summary>
        [ConfigurationProperty("beta", IsRequired = true)]
        public BetaElement Beta
        {
            get { return (BetaElement)this["beta"]; }
        }
    }
}