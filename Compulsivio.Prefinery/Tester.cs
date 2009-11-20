using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compulsivio.Prefinery
{
    /// <summary>
    /// Status of the tester in relation to the beta.
    /// </summary>
    public enum TesterStatus
    {
        /// <summary>Prefinery is unaware of this tester.</summary>
        Unknown = 0,

        /// <summary>Tester has been imported but not invited into the beta.</summary>
        Imported,

        /// <summary>Tester has directly applied for the beta.</summary>
        Applied,

        /// <summary>Tester has been invited to join the beta.</summary>
        Invited,

        /// <summary>Tester is an active beta user.</summary>
        Active,

        /// <summary>Tester has been rejected from the beta.</summary>
        Rejected
    }

    /// <summary>
    /// A tester to be managed by Prefinery.
    /// </summary>
    public class Tester : ITester
    {
        /// <summary>
        /// Initializes a new instance of the Tester class.
        /// </summary>
        public Tester()
        {
            this.Profile = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets or sets the ID associated with the tester for its associated beta.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Gets or sets the tester's email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the invite code used by the tester.
        /// </summary>
        public string InviteCode { get; set; }

        /// <summary>
        /// Gets or sets the tester's current status for its associated beta.
        /// </summary>
        public TesterStatus Status { get; set; }

        /// <summary>
        /// Gets the time at which the tester was originally created in the Prefinery API.
        /// </summary>
        public DateTimeOffset Created { get; internal set; }

        /// <summary>
        /// Gets the time that the tester was last updated in the Prefinery API.
        /// </summary>
        public DateTimeOffset Updated { get; internal set; }

        /// <summary>
        /// Gets profile fields associated with the tester.
        /// </summary>
        public IDictionary<string, string> Profile { get; private set; }

        /// <summary>
        /// Gets the beta to which the tester is associated.
        /// </summary>
        public IBeta Beta { get; internal set; }

        /// <summary>
        /// Send changes to the tester to Prefinery.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">The tester is not associated with any Prefinery betas.</exception>
        public void Update()
        {
            if (this.Beta == null)
            {
                throw new InvalidOperationException("This tester not associated with a beta");
            }

            this.Beta.UpdateTester(this);
        }

        /// <summary>
        /// Remove the tester from Prefinery.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">The tester is not associated with any Prefinery betas.</exception>
        public void Delete()
        {
            if (this.Beta == null)
            {
                throw new InvalidOperationException("This tester not associated with a beta");
            }

            this.Beta.DeleteTester(this);
        }

        /// <summary>
        /// Determines if a given invite code is valid for this tester.
        /// </summary>
        /// <param name="inviteCode">The invite code provided by the user.</param>
        /// <returns><value>true</value> if the invite code is valid; <value>false</value> otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException">No invite code is provided.</exception>
        /// <exception cref="T:System.InvalidOperationException">The tester is not associated with any Prefinery betas.</exception>
        /// <exception cref="T:System.InvalidOperationException">The tester does not have an ID number associated with it.</exception>
        public bool ValidateCode(string inviteCode)
        {
            if (this.Beta == null)
            {
                throw new InvalidOperationException("This tester not associated with a beta");
            }

            if (!this.Id.HasValue)
            {
                throw new InvalidOperationException("Tester needs an Id before invite codes can be validated");
            }

            if (string.IsNullOrEmpty(inviteCode))
            {
                throw new ArgumentNullException("inviteCode");
            }

            return this.Beta.ValidateTesterCode(this, inviteCode);
        }

        /// <summary>
        /// Checks in a tester with the Prefinery API.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">The tester is not associated with any Prefinery betas.</exception>
        public void Checkin()
        {
            if (this.Beta == null)
            {
                throw new InvalidOperationException("This tester not associated with a beta");
            }

            this.Beta.CheckinTester(this);
        }
    }
}
