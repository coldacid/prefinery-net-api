using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compulsivio.Prefinery
{
    public interface ITesterRepository
    {
        ITester GetTester(int id);
        ITester GetTester(string email);
        IEnumerable<ITester> GetTesters();

        void AddTester(ITester tester);
        void UpdateTester(ITester tester);
        void DeleteTester(ITester tester);

        bool ValidateTesterCode(ITester tester, string code);
        void CheckinTester(ITester tester);
    }
}
