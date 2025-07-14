using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartClient.Data.Models;
using SmartClient.Data.Enums;
using SmartClient.Data.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;

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
    private ObservableCollection<Profile> _allProfiles;

    [NotifyCanExecuteChangedFor(nameof(StartAppCommand))]
    [ObservableProperty]
    public Profile _selectedProfile;
    private bool CanStart => SelectedProfile != null;

    private int index = -1;

    private FilterMode _currentFilterMode = FilterMode.None;

    [ObservableProperty]
    public string _searchQuery;

    partial void OnSearchQueryChanged(string value)
    {
        var noneSearch = _memory.LoadCachedProfiles();

        if (SearchQuery == null || SearchQuery == string.Empty)
        {
            FilteredProfiles = new ObservableCollection<Profile>(noneSearch);
            NameFilterMode = FilterMode.None;
            OrtFilterMode = FilterMode.None;
            PersonFilterMode = FilterMode.None;
        }
        else
        {
            FilteredProfiles = new ObservableCollection<Profile>
            (
                AllProfiles.Where
                (
                    p => p.Name.ToLower().Contains(SearchQuery.ToLower()) ||
                    p.Contact.ToLower().Contains(SearchQuery.ToLower()) ||
                    p.City.ToLower().Contains(SearchQuery.ToLower()) ||
                    p.Mail.ToLower().Contains(SearchQuery.ToLower()) ||
                    p.Tele.ToLower().Contains(SearchQuery.ToLower()) ||
                    p.Ip_Address.ToLower().Contains(SearchQuery.ToLower()) ||
                    p.Port.ToLower().Contains(SearchQuery.ToLower()) ||
                    p.Version.ToLower().Contains(SearchQuery.ToLower()) 
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
        _memory.DownloadLibs();
        var cached = _memory.LoadCachedProfiles();
        AllProfiles = new ObservableCollection<Profile>(cached);
        FilteredProfiles = AllProfiles;

        await _memory.LoadFromApiProfiles();
        var updated = _memory.LoadCachedProfiles();

        if (!cached.Select(p => p.CCID).SequenceEqual(updated.Select(p => p.CCID)))
        {
            AllProfiles = new ObservableCollection<Profile>(updated);
            System.Diagnostics.Debug.WriteLine("Update");
        }

        FilteredProfiles = AllProfiles;
    }

    [RelayCommand(CanExecute = nameof(CanStart))]
    private void StartApp()
    {
        _memory.StartCapHotel(SelectedProfile);
    }
    private readonly Dictionary<string, FilterMode> _filterModes = new()
    {
        { "Name", FilterMode.None },
        { "Ort", FilterMode.None },
        { "Person", FilterMode.None }
    };

    [ObservableProperty]
    public FilterMode _nameFilterMode;

    [ObservableProperty]
    public FilterMode _ortFilterMode;

    [ObservableProperty]
    public FilterMode _personFilterMode;

    [RelayCommand]
    private void Logout()
    {
        _memory.DeleteUserData();
        Shell.Current.GoToAsync("//LoginPage");
    }

    [RelayCommand]
    private void ToggleFilter(string filterQuery)
    {
        var noneFilter = _memory.LoadCachedProfiles();
        _filterModes[filterQuery] = _filterModes[filterQuery] switch
        {
            FilterMode.None => FilterMode.Descending,
            FilterMode.Descending => FilterMode.Ascending,
            FilterMode.Ascending => FilterMode.None ,
            _ => FilterMode.None
        };
        switch (filterQuery)
        {
            case "Name":
                AllProfiles = _filterModes["Name"] switch
                {
                    FilterMode.Ascending => new ObservableCollection<Profile>(FilteredProfiles.OrderBy(p => p.Name)),
                    FilterMode.Descending => new ObservableCollection<Profile>(FilteredProfiles.OrderByDescending(p => p.Name)),
                    FilterMode.None => new ObservableCollection<Profile>(noneFilter)
                };
                break;
            case "Ort":
                AllProfiles = _filterModes["Ort"] switch
                {
                    FilterMode.Ascending => new ObservableCollection<Profile>(FilteredProfiles.OrderBy(p => p.City)),
                    FilterMode.Descending => new ObservableCollection<Profile>(FilteredProfiles.OrderByDescending(p => p.City)),
                    FilterMode.None => new ObservableCollection<Profile>(noneFilter)
                };
                break;
            case "Person":
                AllProfiles = _filterModes["Person"] switch
                {
                    FilterMode.Ascending => new ObservableCollection<Profile>(FilteredProfiles.OrderBy(p => p.Contact)),
                    FilterMode.Descending => new ObservableCollection<Profile>(FilteredProfiles.OrderByDescending(p => p.Contact)),
                    FilterMode.None => new ObservableCollection<Profile>(noneFilter)
                };
                break;
        }

        NameFilterMode = _filterModes["Name"];
        OrtFilterMode = _filterModes["Ort"];
        PersonFilterMode = _filterModes["Person"];
        FilteredProfiles = AllProfiles;
    }
}
