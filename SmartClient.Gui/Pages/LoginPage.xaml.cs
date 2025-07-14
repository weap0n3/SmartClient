using SmartClient.Core.ViewModels;

namespace SmartClient.Gui.Pages;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel vm)
	{
		InitializeComponent();
		this.BindingContext = vm;
	}
}