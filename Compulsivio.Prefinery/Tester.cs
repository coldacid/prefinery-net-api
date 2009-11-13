using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compulsivio.Prefinery
{
    public class Tester
    {
        internal PrefineryCore Repository { get; set; }

        public int? Id { get; set; }
        public string Email { get; set; }
        public string InviteCode { get; set; }
        public TesterStatus Status { get; set; }
        public DateTimeOffset Created { get; internal set; }
        public DateTimeOffset Updated { get; internal set; }

        public Dictionary<string, string> Profile { get; private set; }

        public Tester()
        {
            Profile = new Dictionary<string, string>();
        }

        public void Update()
        {
            if (Repository == null)
                throw new InvalidOperationException("This tester not associated with a repository");

            Repository.UpdateTester(this);
        }

        public void Delete()
        {
            if (Repository == null)
                throw new InvalidOperationException("This tester not associated with a repository");

            Repository.DeleteTester(this);
        }

        public bool IsValidInviteCode(string inviteCode)
        {
            if (Repository == null)
                throw new InvalidOperationException("This tester not associated with a repository");
            if (!this.Id.HasValue)
                throw new InvalidOperationException("Tester needs an Id before invite codes can be validated");
            if (string.IsNullOrEmpty(inviteCode))
                throw new ArgumentNullException("No invite code to verify");

            return Repository.ValidateCode(this, inviteCode);
        }

        public void Checkin()
        {
            if (Repository == null)
                throw new InvalidOperationException("This tester not associated with a repository");

            Repository.CheckinTester(this);
        }
    }

    public enum TesterStatus
    {
        Unknown = 0,
        Imported,
        Applied,
        Invited,
        Active,
        Rejected
    }
}
