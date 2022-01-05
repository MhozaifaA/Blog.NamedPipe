using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeService
{
    public delegate void PipeServiceEvent<T>(T e) where T: PipeServiceInfoBase;
}
