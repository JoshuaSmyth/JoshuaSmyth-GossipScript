using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranspileTest
{
    public class ScoppedTimer : IDisposable
    {
        Stopwatch sw = new Stopwatch();
        String m_text;
        public ScoppedTimer(String text)
        {
            m_text = text;
            sw.Start();
        }

        public void Dispose()
        {
            sw.Stop();
            Console.WriteLine(string.Format("{0}: {1}ms", m_text, sw.ElapsedMilliseconds));
        }
    }
}
