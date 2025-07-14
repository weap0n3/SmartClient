using SmartClient.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClient.Data.Services;

public interface IMemory
{
    Task LoadFromApiProfiles();
    List<Profile> LoadCachedProfiles();
    Task StartCapHotel(Profile selectedProfile);
    Task DownloadLibs();
    Task<bool> SaveUserAsync(UserLoginInfo user);
    Task<UserLoginInfo> LoadUserAsync();
    void DeleteUserData();
}
