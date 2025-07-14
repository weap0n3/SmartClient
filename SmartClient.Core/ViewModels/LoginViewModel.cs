using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using SmartClient.Data.Models;
using SmartClient.Data.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClient.Core.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IMemory _memory;
    public LoginViewModel(IMemory memory)
    {
        this._memory = memory;
    }

    [ObservableProperty]
    private string _userName;
    
    [ObservableProperty] 
    private string _password;

    [ObservableProperty]
    private bool _rememberMe;

    [RelayCommand]
    private async Task Login()
    {
        var u = new UserLoginInfo(UserName, Password, RememberMe);
        var result = await _memory.SaveUserAsync(u);
        if(result)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
        else
        {
            UserName = "False";
            Password = "False";
        }
    }


}
