/*
Copyright (c) 2021, Davide "limaCAT" Inglima
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

3. Neither the name of the copyright holder nor the names of its
   contributors may be used to endorse or promote products derived from
   this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace Pmx.Client
{

    public class Result
    {
        public Result(Command Next)
        {
            this.Next = Next;
        }

        public string Status;

        public string Header;

        // TODO Multiline Repsonse

        public Command Next;

    }

    public class Command
    {
        static readonly Logger logger = new Logger(Systems.Client, typeof(Command).Name);

        public TextWriter Writer;

        public TextReader Reader;

        public string Send;

        // 1xx - Informative message
        // 2xx - Command completed OK
        // 3xx - Command OK so far; send the rest of it
        public Command OnSuccess;
        /*
        public Node OnInformation;
        public Node OnContinue;
        */

        // 4xx - Command was syntactically correct but failed for some reason
        // 5xx - Command unknown, unsupported, unavailable, or syntax error
        public Command OnFailure;
        /*
        public Node OnError;
        */

        public Command(TextWriter Writer, TextReader Reader)
        {
            this.Writer = Writer;
            this.Reader = Reader;
        }

        public Result Go()
        {
            if (Send != null)
            {
                logger.Info($"Host: {Send}");
                Writer.WriteLine(Send);
            }
            string result = Reader.ReadLine();
            logger.Info($"Peer: {result}");

            Command next;
            if (result.StartsWith("1") || result.StartsWith("2") || result.StartsWith("3"))
            {
                next = OnSuccess;
            } else
            {
                next = OnFailure;
            }
            return new Result(next)
            {
                Header = result,
                Status = result.Substring(0, 3)
            };
        }

    }

    class Program
    {
        static readonly Logger logger = new Logger(Systems.Client, typeof(Program).Name);

        static void Main(string[] args)
        {
            logger.Info("Hello World!");
            System.Threading.Thread.Sleep(2000);
            logger.Info("Connecting!");

            TcpClient client = new TcpClient("127.0.0.1", 5000);
            int timeout = 5000;

            using (Stream myStream = client.GetStream())
            {
                myStream.ReadTimeout = timeout;

                StreamReader reader = new StreamReader(myStream, Encoding.UTF8);
                StreamWriter writer = new StreamWriter(myStream, Encoding.UTF8) { AutoFlush = true };

                Command header = new Command(writer, reader)
                {
                    OnSuccess = new Command(writer, reader)
                    {
                        Send = "QUIT"
                    }
                };

                Command pointer = header;
                while (pointer != null)
                {
                    Result result = pointer.Go();
                    logger.Info($"Peer: {result.Status}");
                    pointer = result.Next;
                }
            };
        }
    }
}
