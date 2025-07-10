using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartClient.Data.Models;
using SmartClient.Data.Services;
using System.Collections.ObjectModel;

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

    [NotifyCanExecuteChangedFor(nameof(StartAppCommand))]
    [ObservableProperty]
    public Profile _selectedProfile;
    private bool CanStart => SelectedProfile != null;

    private int index = -1;

    [ObservableProperty]
    public string _searchQuery;

    partial void OnSearchQueryChanged(string value)
    {
        var allprofiles = _memory.LoadCachedProfiles();

        if (SearchQuery == null || SearchQuery == string.Empty)
        {
            FilteredProfiles = new ObservableCollection<Profile>(allprofiles); 
        }
        else
        {
            FilteredProfiles = new ObservableCollection<Profile>
            (
                allprofiles.Where
                (
                    p => p.Name.ToLower().Contains(SearchQuery.ToLower()) ||
                    p.Contact.ToLower().Contains(SearchQuery.ToLower()) ||
                    p.City.ToLower().Contains(SearchQuery.ToLower()) ||
                    p.Mail.ToLower().Contains(SearchQuery.ToLower())
                )
            );
        }
    }

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
        var cached = _memory.LoadCachedProfiles();
        FilteredProfiles = new ObservableCollection<Profile>(cached);

        await _memory.LoadFromApiProfiles();
        var updated = _memory.LoadCachedProfiles();

        if (!cached.Select(p => p.CCID).SequenceEqual(updated.Select(p => p.CCID)))
        {
            FilteredProfiles = new ObservableCollection<Profile>(updated);
            System.Diagnostics.Debug.WriteLine("Update");
        }
    }

    [RelayCommand(CanExecute = nameof(CanStart))]
    private void StartApp()
    {
        _memory.StartCapHotel(SelectedProfile);
    }
}
