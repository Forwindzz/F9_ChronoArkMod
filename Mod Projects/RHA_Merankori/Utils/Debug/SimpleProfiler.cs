using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RHA_Merankori
{
    public class SimpleProfiler
    {
        Stopwatch _sw;

        public SimpleProfiler()
        {
            _sw = Stopwatch.StartNew();
        }

        public void CP(string tag)
        {
            _sw.Stop();
            long ms = _sw.ElapsedMilliseconds;
            if(ms>1)
            {
                UnityEngine.Debug.Log($"Profile > \t{ms} ms | \t{tag}");
            }
            _sw.Reset();
            _sw.Start();
        }

        public void Dispose()
        {
            _sw.Stop();
        }
    }
}
