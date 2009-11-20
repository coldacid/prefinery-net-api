using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compulsivio.Prefinery
{
    public interface ITester
    {
        int? Id { get; }
        string Email { get; set; }
        string InviteCode { get; set; }
        TesterStatus Status { get; set; }
        DateTimeOffset Created { get; }
        DateTimeOffset Updated { get; }
        IDictionary<string, string> Profile { get; }
        IBeta Beta { get; }

        void Update();
        void Delete();

        bool ValidateCode(string code);
        void Checkin();
    }
}
