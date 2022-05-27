using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;

namespace EB_Utility
{
    namespace Pipe
    {
        public class NamedPipeServer
        {
            private string name;
            private byte[] buffer;
            private int    buffer_size;
            private NamedPipeServerStream server_stream;

            public NamedPipeServer(string name, int buffer_size=256)
            {
                this.name = name;
                this.buffer_size = buffer_size;
                this.buffer = new byte[buffer_size];
            }

            public void start()
            {
                this.server_stream = new NamedPipeServerStream(name, PipeDirection.InOut);
                this.server_stream.WaitForConnection();
            }

            public void read(Action<byte[]> callback)
            {
                if(!this.server_stream.IsConnected) return;

                int recv = this.server_stream.Read(this.buffer, 0, this.buffer_size);
                if(recv > 0) callback(this.buffer.Take(recv).ToArray());
            }

            public void write(byte[] data)
            {
                if(!this.server_stream.IsConnected) return;

                this.server_stream.Write(data, 0, data.Length);
            }

            public bool is_connected()
            { 
                return this.server_stream.IsConnected; 
            }
        }

        public class NamedPipeClient
        {
            private string                name;
            private int                   connection_timeout;
            private byte[]                buffer;
            private int                   buffer_size;
            private NamedPipeClientStream client_stream;

            public NamedPipeClient(string name, int recieve_buffer_size=256, int timeout_ms=10000)
            {
                if(recieve_buffer_size < 0) throw new System.Exception("Buffer size cannot be negative");

                this.name               = name;
                this.buffer_size        = recieve_buffer_size;
                this.buffer             = new byte[recieve_buffer_size];
                this.connection_timeout = timeout_ms;

                this.client_stream = new NamedPipeClientStream(".", name, PipeDirection.InOut, PipeOptions.None);
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

            // Can't write while read in progress
            public bool read()
            {
                if(!this.client_stream.IsConnected) return false;

                this.client_stream.Read(this.buffer, 0, this.buffer_size);
                return true;
            }

            public bool write(byte[] bytes)
            {
                if(!this.client_stream.IsConnected) return false;

                this.client_stream.Write(bytes, 0, bytes.Length);
                this.client_stream.Flush();
                return true;
            }

            public bool is_connected()
            { 
                return this.client_stream.IsConnected; 
            }
        }
    }
}
