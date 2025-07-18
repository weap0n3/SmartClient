using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartClient.Data.Models;
using SmartClient.Data.Enums;
using SmartClient.Data.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace SmartClient.Core.ViewModels;

public partial class MainViewModel: ObservableObject
{
    private readonly IMemory _memory;
    private readonly PortChecker _portChecker;
    public MainViewModel(IMemory memory)
    {
        this._memory = memory;
        this._portChecker = new PortChecker(_memory);
        //_portChecker.Start();
    }

    [ObservableProperty]
    private ObservableCollection<ProfileUI> _filteredProfiles;

    [ObservableProperty]
    private ObservableCollection<ProfileUI> _allProfiles;

    [NotifyCanExecuteChangedFor(nameof(StartAppCommand))]
    [ObservableProperty]
    public ProfileUI _selectedProfile;

    private bool CanStart => SelectedProfile != null;

    private int index = -1;

    private FilterMode _currentFilterMode = FilterMode.None;

    [ObservableProperty]
    public string _searchQuery;

    partial void OnSearchQueryChanged(string value)
    {
        var noneSearch = _memory.LoadCachedProfiles();
        SelectedProfile = null;
        if (SearchQuery == null || SearchQuery == string.Empty)
        {
            FilteredProfiles = new ObservableCollection<ProfileUI>(ProfileMapper.ListToUI(noneSearch));
            NameFilterMode = FilterMode.None;
            OrtFilterMode = FilterMode.None;
            PersonFilterMode = FilterMode.None;
            VersionFilterMode = FilterMode.None;
        }
        else
        {
            FilteredProfiles = new ObservableCollection<ProfileUI>
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

    partial void OnSelectedProfileChanged(ProfileUI value)
    {
        if (index >= 0)
        {
            FilteredProfiles[index].ColorKey = "#E5E5F0";
            FilteredProfiles[index].TextColor = "#111111";
        }
        if (SelectedProfile != null)
        {
            SelectedProfile.ColorKey = "#2c48e1";
            SelectedProfile.TextColor = "#ffffff";
        }
        index = FilteredProfiles.IndexOf(SelectedProfile);
    }

    [RelayCommand]
    private async void Load()
    {
        _memory.DownloadLibs();
        var cached = _memory.LoadCachedProfiles();
        AllProfiles = new ObservableCollection<ProfileUI>(ProfileMapper.ListToUI(cached));
        FilteredProfiles = AllProfiles;
        SelectedProfile = null;
        await _memory.LoadFromApiProfiles();
        var updated = _memory.LoadCachedProfiles();

        if (!cached.Select(p => p.CCID).SequenceEqual(updated.Select(p => p.CCID)))
        {
            AllProfiles = new ObservableCollection<ProfileUI>(ProfileMapper.ListToUI(updated));
            System.Diagnostics.Debug.WriteLine("Update");
        }

        FilteredProfiles = AllProfiles;
        
        //_portChecker.profiles = FilteredProfiles.ToList();
    }

    [RelayCommand(CanExecute = nameof(CanStart))]
    private void StartApp()
    {
        _memory.StartCapHotel(ProfileMapper.ToData(SelectedProfile));
    }
    private readonly Dictionary<string, FilterMode> _filterModes = new()
    {
        { "Name", FilterMode.None },
        { "City", FilterMode.None },
        { "Contact", FilterMode.None },
        { "Version",FilterMode.None }
    };
    
    [ObservableProperty]
    public FilterMode _nameFilterMode;

    [ObservableProperty]
    public FilterMode _ortFilterMode;

    [ObservableProperty]
    public FilterMode _personFilterMode;

    [ObservableProperty]
    public FilterMode _versionFilterMode;

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
        var property = typeof(ProfileUI).GetProperty(filterQuery);

        AllProfiles = _filterModes[filterQuery] switch
        {
            FilterMode.Ascending => new ObservableCollection<ProfileUI>(FilteredProfiles.OrderBy(p => property.GetValue(p))),
            FilterMode.Descending => new ObservableCollection<ProfileUI>(FilteredProfiles.OrderByDescending(p => property.GetValue(p))),
            FilterMode.None => new ObservableCollection<ProfileUI>(ProfileMapper.ListToUI(noneFilter))
        };

        NameFilterMode = _filterModes["Name"];
        OrtFilterMode = _filterModes["City"];
        PersonFilterMode = _filterModes["Contact"];
        VersionFilterMode = _filterModes["Version"];
        FilteredProfiles = AllProfiles;
    }
}
