using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Compulsivio.Prefinery.Configuration
{
    /// <summary>
    /// Collection of &lt;beta&gt; elements within the application config file.
    /// </summary>
    [ConfigurationCollection(typeof(BetaElement))]
    public class BetaCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Gets the <see cref="T:Compulsivio.Prefinery.Configuration.BetaElement"/> at the specified index location.
        /// </summary>
        /// <param name="idx">The index of the <see cref="T:Compulsivio.Prefinery.Configuration.BetaElement"/> to return.</param>
        /// <returns>The <see cref="T:Compulsivio.Prefinery.Configuration.BetaElement"/> at the given index.</returns>
        public BetaElement this[int idx]
        {
            get { return (BetaElement)BaseGet(idx); }
        }

        /// <summary>
        /// Creates a new <see cref="T:Compulsivio.Prefinery.Configuration.BetaElement"/>.
        /// </summary>
        /// <returns>A <see cref="T:Compulsivio.Prefinery.Configuration.BetaElement"/> with no properties set.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new BetaElement();
        }

        /// <summary>
        /// Gets the element key for a specified <see cref="T:Compulsivio.Prefinery.Configuration.BetaElement"/>.
        /// </summary>
        /// <param name="element">The <see cref="T:Compulsivio.Prefinery.Configuration.BetaElement"/> to return the key for.</param>
        /// <returns>The key for the given <see cref="T:Compulsivio.Prefinery.Configuration.BetaElement"/>.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((BetaElement)element).Id;
        }
    }
}
