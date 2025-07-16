using SmartClient.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartClient.Data.Services;

public class PortChecker
{
    public List<Profile> profiles;
    private readonly CancellationTokenSource _cts = new();
    private readonly IMemory memory;
    public PortChecker(IMemory memory)
    {
        this.memory = memory;
    }

    public void Start()
    {
        _ = Task.Run(() => RunAsync(_cts.Token));
    }

    public void Stop()
    {
        _cts.Cancel();  
    }

    private async Task RunAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (profiles != null)
            {
                foreach (var profile in profiles)
                {
                    profile.IsPortOpen = await IsPortOpenAsync(profile.Ip_Address, Convert.ToInt32(profile.Port));
                    System.Diagnostics.Debug.WriteLine(profile.IsPortOpen);
                }
            }
            try
            {
                await Task.Delay(1000, token); // Wait before next check
            }
            catch (TaskCanceledException)
            {
                // App is shutting down
                break;
            }
        }
    }

    public async Task<bool> IsPortOpenAsync(string host, int port)
    {
        try
        {
            using var client = new TcpClient();
            var connectTask = client.ConnectAsync(host, port);
            var delayTask = Task.Delay(100);

            var completedTask = await Task.WhenAny(connectTask, delayTask);
            return completedTask == connectTask && client.Connected;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            return false;
        }
    }
}
