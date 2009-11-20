using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compulsivio.Prefinery
{
    /// <summary>
    /// A beta whose testers are managed by Prefinery.
    /// </summary>
    public interface IBeta : ITesterRepository
    {
        /// <summary>
        /// Gets the identification number of the beta.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Gets or sets a user-identifiable name for the beta.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets the decode key for comparing e-mails and invite codes.
        /// </summary>
        string DecodeKey { get; }
    }
}
