using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Microsoft.Maui.Devices; // VIGTIGT: Denne skal tilføjes i toppen!

namespace EssentialRemote.Models
{
    public class UdpSenderService
    {
        private readonly UdpClient _udpClient;
        //private readonly string _targetIpAddress = "10.0.2.2"; // mirroed ip adresse fra windows til WSL, emulator: 10.0.2.2 , windows: "127.0.0.1"
        private readonly int _targetPort = 5000; // Porten som Python scriptet skal lytte på

        public UdpSenderService()
        {
            _udpClient = new UdpClient();
        }

        // VORES NYE SMARTE AUTO-IP FUNKTION
        private string GetTargetIpAddress()
        {
            // 1. Kører vi på Windows Desktop?
            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                return "127.0.0.1"; // Brug altid localhost på Windows
            }

            // 2. Kører vi på en Android Emulator (virtuel maskine)?
            if (DeviceInfo.DeviceType == DeviceType.Virtual)
            {
                return "10.0.2.2"; // Android Emulatorens magiske loopback-IP
            }

            // 3. Hvis ingen af delene er sande, MÅ vi være på en fysisk telefon.
            // HER SKAL DU INDTASTE DIN COMPUTERS WI-FI IP-ADRESSE!
            return "10.254.120.132";
        }

        public async Task SendTwistCommandAsync(TwistMessage message)
        {
            try
            {
                string jsonString = message.ToJson();
                byte[] data = Encoding.UTF8.GetBytes(jsonString);

                // Hent automatisk den rigtige IP lige inden vi sender
                string targetIp = GetTargetIpAddress();

                await _udpClient.SendAsync(data, data.Length, targetIp, _targetPort);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fejl ved afsendelse: {ex.Message}");
            }
        }
    }
}
