using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;

namespace Spikes
{

    class Program
    {
        static async Task Main(string[] args)
        {
            var serverEndpoint = new Pipe();
            var clientEndpoint = new Pipe();
            await Task.WhenAll(Task.Run(() => DoClient(clientEndpoint.Writer, serverEndpoint.Reader)), Task.Run(() => DoServer(serverEndpoint.Writer, clientEndpoint.Reader)));
        }

        async static Task DoClient(PipeWriter writer, PipeReader reader)
        {
            Console.WriteLine("Client connecting");

            bool sent = false;
            while (true)
            {
                try
                {
                    if (!sent)
                    {
                        await writer.WriteAsync(Encoding.UTF8.GetBytes("QUACK!\n"));
                        sent = true;
                        writer.Complete();
                        Console.WriteLine("Client Written");
                    }
                }
                catch (Exception ex)
                {
                    LogError(ex);
                    break;
                }

                // Make the data available to the PipeReader.
                FlushResult result = await writer.FlushAsync();
                Console.WriteLine("Client Flushed");
                if (result.IsCompleted)
                {
                    break;
                }
            }

            // By completing PipeWriter, tell the PipeReader that there's no more data coming.
            await writer.CompleteAsync();

            while (true)
            {
                ReadResult result = await reader.ReadAsync();
                ReadOnlySequence<byte> buffer = result.Buffer;
                while (TryReadLine(ref buffer, out ReadOnlySequence<byte> line))
                {
                    // Process the line.
                    ProcessLine(line);
                }
                // Tell the PipeReader how much of the buffer has been consumed.
                reader.AdvanceTo(buffer.Start, buffer.End);
                if (result.IsCompleted)
                {
                    break;
                }
            }

            // By completing PipeWriter, tell the PipeReader that there's no more data coming.
            await reader.CompleteAsync();
            Console.WriteLine("Client stopped");
        }

        async static Task DoServer(PipeWriter writer, PipeReader reader)
        {
            Console.WriteLine("Server creation");

            while (true)
            {
                ReadResult result = await reader.ReadAsync();
                ReadOnlySequence<byte> buffer = result.Buffer;

                while (TryReadLine(ref buffer, out ReadOnlySequence<byte> line))
                {
                    // Process the line.
                    ProcessLine(line);
                }

                // Tell the PipeReader how much of the buffer has been consumed.
                reader.AdvanceTo(buffer.Start, buffer.End);
                // Stop reading if there's no more data coming.
                if (result.IsCompleted)
                {
                    break;
                }
            }
            // Mark the PipeReader as complete.
            await reader.CompleteAsync();
            await writer.WriteAsync(Encoding.UTF8.GetBytes("WHAT?\n"));
            await writer.CompleteAsync();
            Console.WriteLine("Server stopped");
        }

        static void LogError(Exception ex)
        {
            Console.Error.Write(ex);
        }

        static void ProcessLine(in ReadOnlySequence<byte> line)
        {
            byte[] arr = line.ToArray();
            string utfString = Encoding.UTF8.GetString(arr, 0, arr.Length);
            Console.Out.WriteLine(utfString);
        }

        static bool TryReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
        {
            // Look for a EOL in the buffer.
            SequencePosition? position = buffer.PositionOf((byte)'\n');

            if (position == null)
            {
                line = default;
                return false;
            }

            // Skip the line + the \n.
            line = buffer.Slice(0, position.Value);
            buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
            return true;
        }

    }
}
