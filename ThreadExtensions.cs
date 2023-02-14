using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

public static class ThreadExtensions
{
    // https://stackoverflow.com/questions/58866504/is-it-possible-to-await-thread-in-c-sharp
    /*
        var thread = new Thread(() =>
        { 
            Thread.Sleep(1000); // Simulate some background work
        });
        thread.IsBackground = true;
        thread.Start();
        
        await thread; // Wait asynchronously until the thread is completed
        thread.Join(); // If you want to be extra sure that the thread has finished
    */
    public static TaskAwaiter GetAwaiter(this Thread thread)
    {
        return Task.Run(async () =>
        {
            while (thread.IsAlive)
            {
                await Task.Delay(100).ConfigureAwait(false);
            }
        }).GetAwaiter();
    }
}
