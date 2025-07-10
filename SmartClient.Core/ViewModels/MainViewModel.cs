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
    private ObservableCollection<Profile> _filteredProfiles;
    [ObservableProperty]
    public Profile _selectedProfile;
    partial void OnSelectedProfileChanged(Profile value)
    {
        if (index >= 0)
        {
            FilteredProfiles[index].ColorKey = "#EEE1B3";
        }
        if (SelectedProfile != null)
        {
            SelectedProfile.ColorKey = "#38182F";
        }
        index = FilteredProfiles.IndexOf(SelectedProfile);
    }

    [RelayCommand]
    private async void Load()
    {
        var cached = await _memory.LoadCachedProfilesAsync();
        FilteredProfiles = new ObservableCollection<Profile>(cached);

        await _memory.LoadFromApiProfiles();
        var updated = await _memory.LoadCachedProfilesAsync();

        if (!cached.Select(p => p.CCID).SequenceEqual(updated.Select(p => p.CCID)))
        {
            FilteredProfiles = new ObservableCollection<Profile>(updated);
            System.Diagnostics.Debug.WriteLine("Update");
        }


    }
}
