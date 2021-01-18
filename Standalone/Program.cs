using System;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Pmx
{

    public class Systems
    {
        public const string Standalone = "Standalone";
    }

    // TODO Ugly facade, make it so that this routes the message to the actual .net core logging API
    public class Logger
    {

        private readonly string system;
        private readonly string source;

        public Logger(string system, string source)
        {
            this.system = system;
            this.source = source;
        }

        public void Error(string error)
        {
            DateTime now = DateTime.Now;
            Console.WriteLine($"{now}, {system}, {source}, ERROR: {error}");
        }

        public void Info(string info)
        {
            DateTime now = DateTime.Now;
            Console.WriteLine($"{now}, {system}, {source}, INFO: {info}");
        }

    }


}

namespace Pmx.Standalone
{

    class Program
    {
        static Logger logger = new Logger(Systems.Standalone, typeof(Program).Name);

        static void Main(string[] args)
        {
            logger.Info("Starting");

            // TODO: Configure
            TcpListener myListener = new TcpListener(IPAddress.Any, 5000);
            myListener.Start();

            bool quit = false;
            while (!quit)
            {
                // TODO This is just tutorial-level bullshit
                logger.Info("Accepting a connection!");
                Socket mySocket = myListener.AcceptSocket();
                logger.Info("Connection accepted!");
                Stream myStream = new NetworkStream(mySocket);
                StreamReader reader = new StreamReader(myStream);
                StreamWriter writer = new StreamWriter(myStream) { AutoFlush = true };
                string text = reader.ReadLine();
                if (text != null && text.Trim().ToLower() == "quit")
                {
                    quit = true;
                }
                writer.WriteLine("Hello I'm a TCP server");
                myStream.Close();
                mySocket.Close();
                logger.Info("Connection closed!");
            }

            logger.Info("Goodbye!");
        }
    }
}