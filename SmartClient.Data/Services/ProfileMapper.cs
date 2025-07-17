using SmartClient.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClient.Data.Services;

public static class ProfileMapper
{
    private static ProfileUI ToUI(Profile profile)
    {
        return new ProfileUI(profile);
    }

    public static Profile ToData(ProfileUI profileUI)
    {
        return profileUI.profile;
    }

    public static List<ProfileUI> ListToUI(List<Profile> profiles)
    {
        List<ProfileUI> uiProfiles = new List<ProfileUI>();
        foreach(Profile p in profiles)
        {
            uiProfiles.Add(ToUI(p));
        }
        return uiProfiles;
    }
}

