﻿/*
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
    class Program
    {
        static Logger logger = new Logger(Systems.Client, typeof(Program).Name);

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

                string message = reader.ReadLine();
                logger.Info($"Server: {message}"); 

                writer.WriteLine("QUIT");
                message = reader.ReadLine();
                logger.Info($"Server: {message}");

            };// The client and stream will close as control exits the using block (Equivilent but safer than calling Close();
        }
    }
}
