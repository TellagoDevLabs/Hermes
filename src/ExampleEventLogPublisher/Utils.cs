using System;

namespace ExampleEventLogPublisher
{
    static  class Utils
    {
        static ulong _counter;
        static readonly object Sync = new object();

        static public void WriteOnScrollableFrame(string data)
        {
            if (data == null) return;

            const int height = 10;
            var top = Console.WindowHeight - height;

            lock (Sync)
            {
                var lines = data.Length / Console.BufferHeight;
                if (data.Length % Console.BufferWidth > 0) lines++;

                Console.SetCursorPosition(0, top - 1);
                Console.Write(new string('-', Console.BufferWidth));
                Console.MoveBufferArea(0, top + lines, Console.BufferWidth, top + height - lines, 0, top);
                Console.SetCursorPosition(0, top + height - lines);
                Console.Write("{0} --> {1}", ++_counter, data);
            }
        }

    }
}
