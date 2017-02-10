using System;

namespace TvControl.ConsoleApp
{
    class Program
    {

        private static TvServer _tvServer;

        static void Main(string[] args)
        {
            _tvServer = new TvServer(11011, Console.WriteLine);
            _tvServer.Run();

            Console.ReadLine();
        }

    }
}