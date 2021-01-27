using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;
using static Pmx.Hindrance;

namespace Pmx.Communication
{

    // TODO find a better name 2021-01-27
    public class Pepi
    {
        public Pepi(PipeReader Reader, PipeWriter Writer)
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
        static Logger logger = new Logger(Systems.Library, typeof(IPCConnector).Name);

        // motivation, a standalone server process needs only half of two pipes
        public IPCConnector()
        {
            Pipe input = new Pipe();
            Pipe output = new Pipe();
            ServerSide = new Pepi(input.Reader, output.Writer);
            ClientSide = new Pepi(output.Reader, input.Writer);
        }

        public Pepi ServerSide
        {
            get; private set;
        }

        public Pepi ClientSide
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
        static Logger logger = new Logger(Systems.Library, typeof(Session).Name);

        public async Task Execute(PipeReader Reader, PipeWriter Writer)
        {
            WriteLine(Writer, "200 Welcome");
            bool quit = false;
            string line;
            while (!quit)
            {
                // TODO session should be closed if input.Reader is closed 2021-01-27
                line = await ReadLineAsync(Reader);
                logger.Info(line);
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
