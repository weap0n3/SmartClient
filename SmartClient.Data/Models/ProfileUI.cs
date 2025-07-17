using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClient.Data.Models;

public partial class ProfileUI : ObservableObject
{
    public Profile profile;
    public ProfileUI(Profile profile)
    {
        this.profile = profile;
        ColorKey = "#E5E5F0";
        TextColor = "#000000";
    }
    public string CCID => profile.CCID;
    public string Name => profile.Name;
    public string Contact => profile.Contact;
    public string City => profile.City;
    public string Mail => profile.Mail;
    public string Tele => profile.Tele;
    public string Ip_Address => profile.Ip_Address;
    public string Port => profile.Port;
    public string Version => profile.Version;


    [ObservableProperty]
    private bool isPortOpen;

    [ObservableProperty]
    public string colorKey;

    [ObservableProperty]
    public string textColor;
}
