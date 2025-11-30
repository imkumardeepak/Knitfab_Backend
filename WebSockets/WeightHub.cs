using Microsoft.AspNetCore.SignalR;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AvyyanBackend.WebSockets
{
    public class WeightHub : Hub
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private bool _isMonitoring = false;

        public async Task StartWeightMonitoring(string ipAddress, int port = 23)
        {
            if (_isMonitoring)
            {
                await Clients.Caller.SendAsync("WeightError", "Weight monitoring is already active.");
                return;
            }

            _isMonitoring = true;

            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(ipAddress, port);
                var stream = client.GetStream();

                await Clients.Caller.SendAsync("WeightStatus", "Connected to weight machine.");

                byte[] buffer = new byte[256];
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string weightData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        var parsedData = ParseWeightData(weightData);
                        await Clients.Caller.SendAsync("WeightUpdate", parsedData);
                    }

                    await Task.Delay(2000); 
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("WeightError", $"Error connecting to weight machine: {ex.Message}");
                _isMonitoring = false;
            }
        }

        public async Task StopWeightMonitoring()
        {
            if (_isMonitoring)
            {
                _cancellationTokenSource.Cancel();
                _isMonitoring = false;
                await Clients.Caller.SendAsync("WeightStatus", "Weight monitoring stopped.");
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _cancellationTokenSource.Cancel();
            _isMonitoring = false;
            return base.OnDisconnectedAsync(exception);
        }

        private object ParseWeightData(string data)
        {
            // Parse the weight data string (format: "GROSS:123.45 TARE:2.00")
            var parts = data.Split(' ');
            float grossWeight = Convert.ToInt32(parts[0]);
            float tareWeight = 0f;

           

            return new
            {
                grossWeight = grossWeight.ToString("F2"),
                tareWeight = tareWeight.ToString("F2"),
                netWeight = (grossWeight - tareWeight).ToString("F2")
            };
        }
    }
}