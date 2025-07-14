using SmartClient.Core.ViewModels;
using SmartClient.Data.Services;
using SmartClient.Gui.Pages;

namespace SmartClient.Gui
{
    public partial class App : Application
    {
        private readonly FileCleanerService _fileCleanerService;
        private readonly IMemory _memory;
        private readonly MainViewModel vm;
        public App(MainViewModel vm, IMemory memory)
        {
            Application.Current.UserAppTheme = AppTheme.Light;
            InitializeComponent();

            string folderToWatch = @"C:\CapHotel";
            _fileCleanerService = new FileCleanerService(folderToWatch);
            _fileCleanerService.Start();
            this.vm = vm;
            _memory = memory;
        }
        protected override async void OnStart()
        {
            var savedUser = await _memory.LoadUserAsync();
            if (savedUser != null && savedUser.RememberMe)
            {
                // TODO: Validate savedUser info or token here

                // Navigate to MainPage directly
                await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                // Show login page
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}