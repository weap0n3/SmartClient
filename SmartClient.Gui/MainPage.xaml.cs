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
    } 
}
