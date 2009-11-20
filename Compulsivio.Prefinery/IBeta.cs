using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compulsivio.Prefinery
{
    public interface IBeta : ITesterRepository
    {
        int Id { get; }
        string Name { get; set; }
        string DecodeKey { get; }
    }
}
