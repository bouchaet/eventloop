using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace eventloop
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Welcome. My process id: {Process.GetCurrentProcess().Id}.");
            Console.WriteLine("Press enter to start.");
            Console.ReadLine();

            var activeClient = 0;
            var clientId = 0;
            var tasks = new List<Task>();
            var read = Task.Run(
                () => { Console.Read(); Console.WriteLine("STOP requested."); });
            tasks.Add(read);

            /* MAIN LOOP */
            do
            {
                /*await*/ 
                Task.Delay(500).Wait(); // accept sock
                Interlocked.Increment(ref activeClient);
                Func<int, Task> a = async (i) =>
                {
                    Console.WriteLine($"client {i} connected. ({activeClient} are active)");
                    var count = 0;
                    while ((DateTime.Now.Second + i) % 59 != 0) // clientsock.Connected;
                    {
                        await Task.Delay(1500); // sock.read
                        await Task.Delay(50); // get response
                        await Task.Delay(200); // sock.write
                        ++count;
                    }
                    Console.WriteLine($"client {i} disconnected after {count} requests.");
                    Interlocked.Decrement(ref activeClient);
                };
                tasks.Add(a.Invoke(++clientId));

                tasks.RemoveAll(t => t.IsCompleted);
            }
            while (!read.IsCompleted);

            Console.WriteLine($"Task count: {tasks.Count}");
        }
    }
}
