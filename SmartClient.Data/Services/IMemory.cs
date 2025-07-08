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
    Task<List<Profile>> LoadCachedProfilesAsync();
}
