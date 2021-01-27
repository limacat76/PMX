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
using System.Buffers;
using System.IO.Pipelines;
using System.Text;

namespace Pmx
{

    public class Systems
    {
        public const string Standalone = "Standalone";
        public const string Client = "Client";
        public const string Library = "Library";
        public const string Tests = "Tests";
    }

    // TODO Ugly facade, make it so that this routes the message to the actual net core logging API 2021-01-27
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

    public class Hindrance
    {

        // TODO manage completions 2021-01-27
        // await writer.CompleteAsync();
        // By completing PipeWriter, tell the PipeReader that there's no more data coming.
        // await writer.CompleteAsync();
        // By completing PipeWriter, tell the PipeReader that there's no more data coming.
        // await reader.CompleteAsync();

        // TODO Return a thing with Message, ConnectionStatus and ServerStatus 2021-01-27
        public static async System.Threading.Tasks.Task<string> ReadLineAsync(PipeReader from)
        {
            string toReturn = null;
            // TODO I am not sure this should work but I am leaving it for now 2021-01-27
            ReadResult result = await from.ReadAsync();
            ReadOnlySequence<byte> buffer = result.Buffer;
            while (toReturn == null)
            {
                toReturn = TryReadLine(ref buffer);
            }
            // Tell the PipeReader how much of the buffer has been consumed.
            from.AdvanceTo(buffer.Start, buffer.End);
            return toReturn;
        }

        // TODO Return a thing with Message, ConnectionStatus and ServerStatus 2021-01-27
        public static async System.Threading.Tasks.Task<string> RoundTrip(PipeWriter to, string value, PipeReader from)
        {
            WriteLine(to, value);
            return await ReadLineAsync(from);
        }

        public async static void WriteLine(PipeWriter to, string value)
        {
            await to.WriteAsync(Encoding.UTF8.GetBytes($"{value}\r\n"));
        }

        static string ProcessLine(ReadOnlySequence<byte> line)
        {
            byte[] arr = line.ToArray();
            string utfString = Encoding.UTF8.GetString(arr, 0, arr.Length);
            return utfString;
        }

        static string TryReadLine(ref ReadOnlySequence<byte> buffer)
        {
            // Look for a EOL in the buffer.
            SequencePosition? position = buffer.PositionOf((byte)'\n');

            if (position == null)
            {
                return null;
            }
            // TODO Test for \r before \n, see if there's a syntax error for invalid newlines

            // Skip the line + the \n.
            ReadOnlySequence<byte> line = buffer.Slice(0, position.Value);
            buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
            return ProcessLine(line);
        }

    }

}
