using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace EssentialRemote.Models
{
    public class UdpSenderService
    {
        private readonly UdpClient _udpClient;
        private readonly string _targetIpAddress = "172.31.47.53"; // PC ip -adresse, hvor Python-scriptet kører, lige pt. hente det påny med hostname -I kommandoen i terminalen på PC'en 
        private readonly int _targetPort = 5000; // Porten som Python scriptet skal lytte på

        public UdpSenderService()
        {
            _udpClient = new UdpClient();
        }

        public async Task SendTwistCommandAsync(TwistMessage message)
        {
            try
            {
                string jsonString = message.ToJson();
                byte[] data = Encoding.UTF8.GetBytes(jsonString);

                await _udpClient.SendAsync(data, data.Length, _targetIpAddress, _targetPort);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fejl ved afsendelse: {ex.Message}");
            }
        }
    }
}
