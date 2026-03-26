using EssentialRemote.ViewModels;

namespace EssentialRemote
{
    public partial class MainPage : ContentPage
    {
        public MainPage(RobotControllerViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            // Tjekker at vores ViewModel er forbundet, og sender eventet videre
            if (BindingContext is RobotControllerViewModel vm)
            {
                vm.PanUpdatedCommand.Execute(e);
            }
        }
    }
}
