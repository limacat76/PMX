using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;
using static Pmx.Hindrance;
namespace Pmx.Server
{
    // TODO implement IDisposable() for the Complete() 2021-01-27
    public class Session
    {
        static Logger logger = new Logger(Systems.Library, typeof(Session).Name);
        private Pipe input;
        private Pipe output;

        public PipeReader Reader
        {
            get { 
                return output.Reader;
            }
        }

        public PipeWriter Writer
        {
            get
            {
                return input.Writer;
            }
        }

        public Session()
        {
            input = new Pipe();
            output = new Pipe();
        }

        public async Task Execute()
        {
            WriteLine(output.Writer, "200 Welcome");
            bool quit = false;
            string line;
            while (!quit)
            {
                // TODO session should be closed if input.Reader is closed 2021-01-27
                line = await ReadLineAsync(input.Reader);
                logger.Info(line);
                if (line.Trim().ToUpper().Equals("QUIT"))
                {
                    quit = true;
                }
                else
                {
                    WriteLine(output.Writer, "500 Not Implemented");
                }
            }
            // TODO since we broke or are outside the loop we should close output.Writer 2021-01-27
            WriteLine(output.Writer, "205 Goodbye");
        }

    }
}
