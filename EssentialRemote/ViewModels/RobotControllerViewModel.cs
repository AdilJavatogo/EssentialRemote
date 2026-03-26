// ViewModels/RobotJoystickViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EssentialRemote.Models;
using System.Text.Json;

namespace EssentialRemote.ViewModels
{
    public partial class RobotControllerViewModel : ObservableObject
    {
        private readonly UdpSenderService _udpSender;
        private readonly IDispatcherTimer _timer;

        // Joystick konstanter for matematik
        private const double BaseRadius = 150.0; // Radius af den store cirkel
        private const double Deadzone = 20.0;     // Hvor meget man skal flytte knoppen før vi sender data (forhindrer flimmer)

        public RobotControllerViewModel()
        {
            // Vi initialiserer vores UDP-service (vi bruger Dependency Injection senere)
            _udpSender = new UdpSenderService();

            // Vi bruger en timer til kun at sende data periodisk (f.eks. 10 gange i sekundet), 
            // i stedet for at sende en pakke for hver pixel brugeren flytter fingeren.
            _timer = Application.Current.Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += (s, e) => SendCommands();
        }

        [ObservableProperty]
        private double joystickX = BaseRadius; // Startposition for knoppen (midten af cirklen)

        [ObservableProperty]
        private double joystickY = BaseRadius; // Startposition for knoppen (midten af cirklen)

        [ObservableProperty]
        private bool isSending = false; // Angiver om vi i øjeblikket sender data (fingeren er på skærmen)

        // Hastigheder beregnet ud fra joystick-positionen
        [ObservableProperty]
        private float linearXSpeed;

        [ObservableProperty]
        private float angularZSpeed;

        [RelayCommand]
        private void PanUpdated(PanUpdatedEventArgs e)
        {
            // Vores store cirkel har radius 150. e.TotalX/Y er hvor meget vi har trukket fra startpunktet.
            double currentX = 150 + e.TotalX;
            double currentY = 150 + e.TotalY;

            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    IsSending = true;
                    // _timer.Start(); // Hvis du bruger timeren fra tidligere
                    UpdateJoystick(new Point(currentX, currentY));
                    break;

                case GestureStatus.Running:
                    UpdateJoystick(new Point(currentX, currentY));
                    break;

                case GestureStatus.Completed:
                case GestureStatus.Canceled:
                    ResetJoystick();
                    break;
            }
        }

        [RelayCommand]
        private void OnTouchStarted(Point location)
        {
            // Når brugeren trykker ned, flytter vi knoppen og starter timeren
            UpdateJoystick(location);
            isSending = true;
            _timer.Start();
        }

        [RelayCommand]
        private void OnTouchMoved(Point location)
        {
            // Når brugeren trækker fingeren, opdaterer vi knoppens position
            UpdateJoystick(location);
        }

        [RelayCommand]
        private void OnTouchEnded()
        {
            // Når brugeren slipper skærmen, nulstiller vi joysticket til midten og stopper timeren
            ResetJoystick();
        }

        private void UpdateJoystick(Point touchPoint)
        {
            // Beregn afstanden fra midten af den store cirkel
            double deltaX = touchPoint.X - BaseRadius;
            double deltaY = touchPoint.Y - BaseRadius;
            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            // Begræns knoppen til at forblive inden for den store cirkel
            if (distance > BaseRadius)
            {
                // Brug trigonometri til at finde den begrænsede x, y position
                double angle = Math.Atan2(deltaY, deltaX);
                JoystickX = BaseRadius + Math.Cos(angle) * BaseRadius;
                JoystickY = BaseRadius + Math.Sin(angle) * BaseRadius;
                distance = BaseRadius; // Sæt afstand til max
            }
            else
            {
                // Flyt blot knoppen til touch-punktet, hvis det er indenfor cirklen
                JoystickX = touchPoint.X;
                JoystickY = touchPoint.Y;
            }

            // --- BEREGN HASTIGHEDER FOR TWIST (Standard matematik for joystick styring) ---

            // 1. Lineær hastighed (X): Afhænger af afstanden fra midten og Y-aksen (frem/tilbage)
            // Vi dividerer med BaseRadius for at få en værdi mellem -1.0 og 1.0.
            // (Y-aksen er inverteret i UI, så vi bruger negativ deltaY for 'frem')
            LinearXSpeed = distance < Deadzone ? 0f : (float)(-deltaY / BaseRadius);

            // 2. Angulær hastighed (Z): Afhænger af X-aksen (venstre/højre)
            // For at gøre det nemmere at køre lige frem, dæmper vi drejningen når joysticket er næsten lige frem.
            // En simpel tilgang er at bruge deltaX divideret med BaseRadius, men du kan forbedre dette.
            // Dette muliggør skrå kørsel, da både LinearX og AngularZ kan være aktive samtidigt.
            AngularZSpeed = distance < Deadzone ? 0f : (float)(deltaX / BaseRadius);

            // Vi kan tilføje mere avanceret dæmpning her, men dette er en solid start.
            // For eksempel, kan vi gøre det sværere at dreje når man kører meget hurtigt (høj LinearX).
        }

        private void ResetJoystick()
        {
            // Sæt alt til midten/stop
            _timer.Stop();
            IsSending = false;
            JoystickX = BaseRadius;
            JoystickY = BaseRadius;
            LinearXSpeed = 0f;
            AngularZSpeed = 0f;

            // Send en stop-besked med det samme
            SendCommands();
        }

        private void SendCommands()
        {
            // Hvis vi ikke trykker, men timeren alligevel tikker (skal ikke ske), så stop
            if (!IsSending && LinearXSpeed == 0f && AngularZSpeed == 0f) return;

            // Opret og send Twist beskeden
            var msg = new TwistMessage
            {
                LinearX = LinearXSpeed,
                AngularZ = AngularZSpeed
            };

            // Publicer via UDP
            _udpSender.SendTwistCommand(msg);
        }
    }
}