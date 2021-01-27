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
using System.IO.Pipelines;
using System.Threading.Tasks;
using static Pmx.Hindrance;

namespace Pmx.Communication
{

    // TODO find a better use case 2021-01-27
    public class Pair
    {
        public Pair(PipeReader Reader, PipeWriter Writer)
        {
            this.Reader = Reader;
            this.Writer = Writer;
        }

        public PipeReader Reader
        {
            get; private set;
        }

        public PipeWriter Writer
        {
            get; private set;
        }
    }

    public class IPCConnector
    {
        // motivation, a standalone server process needs only half of two pipes
        public IPCConnector()
        {
            Pipe input = new Pipe();
            Pipe output = new Pipe();
            ServerSide = new Pair(input.Reader, output.Writer);
            ClientSide = new Pair(output.Reader, input.Writer);
        }

        public Pair ServerSide
        {
            get; private set;
        }

        public Pair ClientSide
        {
            get; private set;
        }
    }

}

namespace Pmx.Server
{

    // TODO implement IDisposable() for the Complete() 2021-01-27
    public class Session
    {
        private readonly static Logger logger = new Logger(Systems.Library, nameof(Session));

        public async Task Execute(PipeReader Reader, PipeWriter Writer)
        {
            WriteLine(Writer, "200 PMX");
            bool quit = false;
            string line;
            while (!quit)
            {
                // TODO session should be closed if input.Reader is closed 2021-01-27
                line = await ReadLineAsync(Reader);
                logger.Info($"Line Received: {line}");
                if (line.Trim().ToUpper().Equals("QUIT"))
                {
                    quit = true;
                }
                else
                {
                    WriteLine(Writer, "500 Not Implemented");
                }
            }
            // TODO since we broke or are outside the loop we should close output.Writer 2021-01-27
            WriteLine(Writer, "205 Goodbye");
        }

    }
}
