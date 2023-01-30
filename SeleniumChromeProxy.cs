// NuGet Packet Requirements:
// [+] Titanium.Web.Proxy by Titanium.Web.Proxy

// Class Requirements:
// https://github.com/egebilecen/eb-cs/blob/main/SeleniumChrome.cs

using System;
using System.Collections.Generic;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;
using SeleniumProxyAuth;
using SeleniumProxyAuth.Models;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Threading.Tasks;

// Credits: https://github.com/Erol444/SeleniumProxyAuth
// NuGet package of this class is using an outdated dependency which causes error
// when I try to compile the code so I had to add the class into this file.
namespace SeleniumProxyAuth
{
    namespace Models
    {
        public class ProxyAuth
        {
            public ProxyAuth(string proxy, int port, string username, string password)
            {
                Proxy = proxy;
                Port = port;
                Username = username;
                Password = password;
            }

            public string Proxy { get; }
            public int Port { get; }
            public string Username { get; }
            public string Password { get; }

        }
    }

    public class SeleniumProxyServer : IDisposable
    {
        private Dictionary<int, Models.ProxyAuth> proxyAuths;
        private ProxyServer proxyServer;

        public SeleniumProxyServer()
        {
            proxyAuths = new Dictionary<int, Models.ProxyAuth>();
            proxyServer = new ProxyServer();
            proxyServer.BeforeRequest += OnRequest;
            proxyServer.Start();
        }

        /// <summary>
        /// Adds a new endpoint to the local proxy server
        /// </summary>
        /// <param name="auth">ProxyAuth</param>
        /// <returns>Port where the new proxy will be opened</returns>
        public int AddEndpoint(Models.ProxyAuth auth)
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] conArr = ipGlobalProperties.GetActiveTcpListeners();

            for (int i = 50000; i < 60000; i++)
            {
                if (conArr.Any(x => x.Port == i)) continue;

                proxyServer.AddEndPoint(new ExplicitProxyEndPoint(IPAddress.Any, i, true));

                proxyAuths.Add(i, auth);
                return i;
            }
            throw new Exception("Couldn't find any available tcp port!");
        }

        public void Close()
        {
            proxyServer.Stop();
            proxyServer.Dispose();
        }

        /// <summary>
        /// When a new request is received, set up the upstream proxy based on the port of the request
        /// </summary>
        private async Task OnRequest(object sender, SessionEventArgs e)
        {
            if (!proxyAuths.TryGetValue(e.ClientLocalEndPoint.Port, out var auth))
            {
                e.Ok("<html><h>Error with proxy</h></html>");
                return;
            }

            e.CustomUpStreamProxy = new ExternalProxy(auth.Proxy, auth.Port, auth.Username, auth.Password)
            {
                ProxyType = ExternalProxyType.Http
            };
        }

        /// <summary>
        /// Dispose the proxy server
        /// </summary>
        public void Dispose()
        {
            this.proxyServer.Dispose();
        }
    }
}

public class SeleniumChromeProxy
{
    private readonly SeleniumProxyServer proxyServer = new SeleniumProxyServer();
    public SeleniumChrome selenium = new SeleniumChrome();

    public void Init(string ip, string port, (string, string)? authPair = null, bool headless = false, double scaleFactor = 1.0, string userAgentOverride = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36")
    {
        List<string> extraArgs = new List<string>();

        if(authPair != null
        && !string.IsNullOrEmpty(authPair?.Item1)
        && !string.IsNullOrEmpty(authPair?.Item2))
        {
            int localPort = proxyServer.AddEndpoint(
                new ProxyAuth(ip, 
                              Convert.ToInt32(port), 
                              authPair?.Item1, 
                              authPair?.Item2
                )
            );

            extraArgs.Add($"--proxy-server=127.0.0.1:{localPort}");
        }
        else extraArgs.Add($"--proxy-server={ip}:{port}");

        selenium.Init(headless: headless, scaleFactor: scaleFactor, userAgentOverride: userAgentOverride, extraArgs: extraArgs);
    }

    public void Close()
    {
        proxyServer.Close();
        selenium.Close();
    }
}
