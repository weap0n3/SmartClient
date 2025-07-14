namespace SmartClient.Gui
{
    public partial class App : Application
    {
        private readonly FileCleanerService _fileCleanerService;
        {
            Application.Current.UserAppTheme = AppTheme.Light;
            InitializeComponent();
            _fileCleanerService = new FileCleanerService(folderToWatch);
            _fileCleanerService.Start();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}