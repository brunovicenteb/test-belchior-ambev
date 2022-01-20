using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmbevWeb.Pipeline
{
    public abstract class XTask<T, V>
    {
        public abstract V Execute(T pInput, ref bool pStop);
    }
}