using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compulsivio.Prefinery
{
    /// <summary>
    /// A tester managed by Prefinery.
    /// </summary>
    public interface ITester
    {
        /// <summary>
        /// Gets the identification number of the tester.
        /// </summary>
        int? Id { get; }

        /// <summary>
        /// Gets or sets the e-mail address of the tester.
        /// </summary>
        string Email { get; set; }

        /// <summary>
        /// Gets or sets the invite code used by the tester.
        /// </summary>
        string InviteCode { get; set; }

        /// <summary>
        /// Gets or sets the tester's current status.
        /// </summary>
        TesterStatus Status { get; set; }

        /// <summary>
        /// Gets the time at which the tester was registered with Prefinery.
        /// </summary>
        DateTimeOffset Created { get; }

        /// <summary>
        /// Gets the time at which the tester was last updated on Prefinery.
        /// </summary>
        DateTimeOffset Updated { get; }

        /// <summary>
        /// Gets the tester's profile fields.
        /// </summary>
        IDictionary<string, string> Profile { get; }

        /// <summary>
        /// Gets the beta to which the tester is registered.
        /// </summary>
        IBeta Beta { get; }

        /// <summary>
        /// Submits changes on the tester to Prefinery.
        /// </summary>
        void Update();

        /// <summary>
        /// Removes the tester from its associated beta.
        /// </summary>
        void Delete();

        /// <summary>
        /// Checks whether a given code is a valid invite code for the tester.
        /// </summary>
        /// <param name="code">The invite code submitted by the user.</param>
        /// <returns><value>true</value> if the invite code is valid; <value>false</value> otherwise.</returns>
        bool ValidateCode(string code);

        /// <summary>
        /// Tells Prefinery that the tester has logged in.
        /// </summary>
        void Checkin();
    }
}
