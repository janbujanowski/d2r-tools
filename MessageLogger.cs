using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2Rbots
{
    public static class MessageLogger
    {
        private static BlockingCollection<string> m_Queue = new BlockingCollection<string>();

        static MessageLogger()
        {
            var thread = new Thread(
              () =>
              {
                  while (true) Console.WriteLine(m_Queue.Take());
              });
            thread.IsBackground = true;
            thread.Start();
        }

        public static void WriteLine(string message)
        {
            m_Queue.Add($"|{DateTime.UtcNow}| : {message}");
        }
    }
}
