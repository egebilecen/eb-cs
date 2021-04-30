using System;
using System.IO;
using System.IO.Pipes;

namespace EB_Utility
{
    namespace Pipe
    {
        public class NamedPipeServer
        {
            private string name;

            public NamedPipeServer(string name)
            {
                this.name = name;
            }
        }

        public class NamedPipeClient
        {
            private string                name;
            private int                   connection_timeout;
            private char[]                buffer;
            private int                   buffer_size;
            private NamedPipeClientStream client_stream;
            private StreamWriter          writer;
            private StreamReader          reader;

            public NamedPipeClient(string name, int recieve_buffer_size=256, int timeout_ms=10000)
            {
                if(recieve_buffer_size < 0) throw new System.Exception("Buffer size cannot be negative");

                this.name               = name;
                this.buffer_size        = recieve_buffer_size;
                this.buffer             = new char[recieve_buffer_size];
                this.connection_timeout = timeout_ms;

                this.client_stream = new NamedPipeClientStream(".", name, PipeDirection.InOut, PipeOptions.None);

                this.writer = new StreamWriter(this.client_stream);
                this.reader = new StreamReader(this.client_stream);
            }

            public bool start()
            {
                try
                {
                    this.client_stream.Connect(this.connection_timeout);
                }
                catch(Exception)
                {
                    return false;
                }

                return true;
            }

            public bool write(char[] bytes)
            {
                if(!this.client_stream.IsConnected) return false;

                this.writer.Write(bytes);
                return true;
            }

            public bool read()
            {
                if(!this.client_stream.IsConnected) return false;

                this.reader.Read(this.buffer, 0, this.buffer_size);
                return true;
            }
        }
    }
}
