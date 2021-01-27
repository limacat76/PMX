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
using System.IO;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Pmx.Server;

namespace Pmx.Standalone
{
    class Program
    {
        private readonly static Logger logger = new Logger(Systems.Standalone, nameof(Program));

        static async Task Main(string[] args)
        {
            logger.Info("Starting");

            // TODO Make configurable 2021-01-27
            TcpListener myListener = new TcpListener(IPAddress.Any, 5000);
            myListener.Start();

            bool shutdown = false;
            while (!shutdown)
            {
                logger.Info("Accepting a connection!");
                Socket mySocket = await myListener.AcceptSocketAsync();
                _ = RunSession(mySocket);
            }

            logger.Info("Goodbye!");
        }

        private static async Task RunSession(Socket mySocket)
        {
            logger.Info("Connection accepted!");
            Stream myStream = new NetworkStream(mySocket);
            PipeReader reader = PipeReader.Create(myStream);
            PipeWriter writer = PipeWriter.Create(myStream);

            Session session = new Session();
            await session.Execute(reader, writer);
            myStream.Close();
            mySocket.Close();
            logger.Info("Connection closed!");
        }
    }
}