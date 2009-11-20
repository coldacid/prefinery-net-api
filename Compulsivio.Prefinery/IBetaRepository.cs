using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compulsivio.Prefinery
{
    public interface IBetaRepository
    {
        IBeta GetBeta(int id);
        IBeta GetBeta(string name);
        IEnumerable<IBeta> GetBetas();

        // NOT IN PREFINERY API YET
        void AddBeta(IBeta beta);
        //void UpdateBeta(IBeta beta);
        //void DeleteBeta(IBeta beta);
    }
}
