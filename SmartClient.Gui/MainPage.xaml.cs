using CommunityToolkit.Maui.Behaviors;
using SmartClient.Core.ViewModels;

namespace SmartClient.Gui
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel mainViewModel)
        {
            InitializeComponent();
            this.BindingContext = mainViewModel;
            var behavior = new EventToCommandBehavior
            {
                EventName = nameof(ContentPage.Appearing),
                Command = mainViewModel.LoadCommand
            };
            Behaviors.Add(behavior);
        }
        private async void OnEmailTapped(object sender, EventArgs e)
        {
            if (sender is Label emailLabel)
            {
                string email = emailLabel.Text?.Trim();

                if (!string.IsNullOrEmpty(email))
                {
                    var mailtoUri = new Uri($"mailto:{email}");

                    if (await Launcher.CanOpenAsync(mailtoUri))
                    {
                        await Launcher.OpenAsync(mailtoUri);
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Unable to open email client.", "OK");
                    }
                }
            }
        }

    }
}
