using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace EssentialRemote.Models
{
    public class UdpSenderService
    {
        private readonly UdpClient _udpClient;
        private readonly string _targetIpAddress = "172.17.45.131"; // PC ip -adresse, hvor Python-scriptet kører
        private readonly int _targetPort = 5005; // Den port dit Python-script skal lytte på

        public UdpSenderService()
        {
            _udpClient = new UdpClient();
        }

        public void SendTwistCommand(TwistMessage message)
        {
            try
            {
                string jsonString = message.ToJson();
                byte[] data = Encoding.UTF8.GetBytes(jsonString);

                // Sender dataene afsted via UDP
                _udpClient.Send(data, data.Length, _targetIpAddress, _targetPort);
            }
            catch (Exception ex)
            {
                // Håndter evt. netværksfejl her
                Console.WriteLine($"Fejl ved afsendelse: {ex.Message}");
            }
        }
    }
}
