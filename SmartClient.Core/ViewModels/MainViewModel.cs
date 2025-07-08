using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartClient.Data.Models;
using SmartClient.Data.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClient.Core.ViewModels;

public partial class MainViewModel: ObservableObject
{
    private readonly IMemory _memory;
    public MainViewModel(IMemory memory)
    {
        this._memory = memory;
    }


    [ObservableProperty]
    private ObservableCollection<Profile> _profiles;

    [RelayCommand]
    private async void Load()
    {
        Profiles = new ObservableCollection<Profile>(await _memory.GetProfiles());
    }
}
