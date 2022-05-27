using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;

namespace EB_Utility
{
    namespace Pipe
    {
        public class NamedPipeServer
        {
            private string name;
            private byte[] buffer;
            private int    buffer_size;
            private Action<byte[]> data_handler = null;
            private NamedPipeServerStream server_stream;

            public NamedPipeServer(string name, int buffer_size=256)
            {
                this.name = name;
                this.buffer_size = buffer_size;
                this.buffer = new byte[buffer_size];
            }

            public void start()
            {
                if(this.data_handler == null)
                    throw new Exception("data_handler is null.");

                this.server_stream = new NamedPipeServerStream(name, PipeDirection.InOut);
                this.server_stream.WaitForConnection();

                while(this.server_stream.IsConnected)
                {
                    int recv = this.server_stream.Read(this.buffer, 0, this.buffer_size);
                    if(recv > 0) this.data_handler(this.buffer.Take(recv).ToArray());
                }

                this.server_stream.Close();
                this.start();
            }

            public void set_data_handler(Action<byte[]> func)
            {
                this.data_handler = func;
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

            public bool write(byte[] bytes)
            {
                if(!this.client_stream.IsConnected) return false;

                this.client_stream.Write(bytes, 0, bytes.Length);
                this.client_stream.Flush();
                return true;
            }

            public bool read()
            {
                if(!this.client_stream.IsConnected) return false;

                this.client_stream.Read(this.buffer, 0, this.buffer_size);
                return true;
            }

            public byte[] get_buffer()
            {
                return this.buffer;
            }
        }
    }
}
