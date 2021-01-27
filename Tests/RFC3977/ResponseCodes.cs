using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Pmx;

namespace Tests.RFC3977
{
    public class ResponseCodes
    {
        static Logger logger = new Logger(Systems.Tests, typeof(ResponseCodes).Name);

        public static void Is2XX(string line)
        {
            logger.Info(line);
            Assert.StartsWith("2", line);
        }

        public static void Is5XX(string line)
        {
            logger.Info(line);
            Assert.StartsWith("5", line);
        }

    }
}
