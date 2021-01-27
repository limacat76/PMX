using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Pmx.Server;
using static Pmx.Hindrance;
using static Tests.RFC3977.ResponseCodes;
using System.IO.Pipelines;

namespace Tests.RFC3977.C03BasicConcepts
{
    public class P0301CommandsAndResponses
    {
        [Fact]
        /// <summary>
        /// When the connection is established, the NNTP server host MUST send a greeting.
        /// </summary>
        public async void ConnectionEstablished()
        {
            Session server = new Session();
            PipeReader reader = server.Reader;
            PipeWriter writer = server.Writer;

            _ = server.Execute();
            Is2XX(await ReadLineAsync(reader));
            Is5XX(await RoundTrip(writer, "QUACK", reader));
            Is2XX(await RoundTrip(writer, "QUIT", reader));
        }

        // TODO RIMUOVERE
        /*
        [Theory]
        [InlineData(42)]
        public void Test2(int value)
        {
            Assert.Equal(value, Regression.Fn());
        }
        */

    }
}
