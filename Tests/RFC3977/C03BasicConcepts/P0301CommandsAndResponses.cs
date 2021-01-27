using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Pmx.Server;
using Pmx.Communication;
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
            IPCConnector connector = new IPCConnector();
            Session server = new Session();
            PipeReader reader = connector.ClientSide.Reader;
            PipeWriter writer = connector.ClientSide.Writer;

            _ = server.Execute(connector.ServerSide.Reader, connector.ServerSide.Writer);
            Is2XX(await ReadLineAsync(reader));
            Is5XX(await RoundTrip(writer, "QUACK", reader));
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
