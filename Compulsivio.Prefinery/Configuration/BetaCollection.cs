using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Compulsivio.Prefinery.Configuration
{
    [ConfigurationCollection(typeof(BetaElement))]
    public class BetaCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new BetaElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((BetaElement)(element)).Id;
        }

        public BetaElement this[int idx]
        {
            get { return (BetaElement)BaseGet(idx); }
        }
    }
}
