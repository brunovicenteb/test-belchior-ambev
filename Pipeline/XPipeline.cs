using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AmbevWeb.Pipeline
{
    public abstract class XPipeline<T, V>
    {
        private readonly List<XTask<T, V>> _Filters = new List<XTask<T, V>>();

        public void Register(XTask<T, V> pTask)
        {
            _Filters.Add(pTask);
        }

        public V Process(T pInput)
        {
            V v = default(V);
            foreach (var f in _Filters)
            {
                bool stop = false;
                v = f.Execute(pInput, ref stop);
                if (stop)
                    break;
            }
            return v;
        }
    }
}