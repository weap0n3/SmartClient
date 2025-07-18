﻿using SmartClient.Data.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartClient.Data.Services;
public class MemoryService : IMemory
{
    private readonly HttpClient _client;
    private readonly string url = "https://mainframe.capcorn.net/RestService/CapEngineConnectionInfos";

    private readonly string _appDataPath;
    private  string _cachePath;
    private string _versionsFolderPath;
    private readonly string _loginInfoPath;
    public MemoryService(string appDataPath)
    {
        this._client = new HttpClient();
        this._appDataPath = appDataPath;
        this._cachePath = Path.Combine(_appDataPath, "profiles.json");
        this._versionsFolderPath = Path.Combine(_appDataPath, "Versions");
        this._loginInfoPath = Path.Combine(_appDataPath, "loginInfo.json");
    }
    public async Task DownloadLibs()
    {
        var capHotelPath = @"C:\CapHotel";
        var downloadUrl = "https://www.capcorn.at/update/libraries/caphotel/";
        string[] libs = { "WebView2Loader.dll", "libcrypto-3.dll", "libssl-3.dll", "mfc140.dll", "vcruntime140.dll", "wkhtmltox.dll" };

        foreach (string lib in libs)
        {
            var fileBytes = await _client.GetByteArrayAsync(Path.Combine(downloadUrl, lib));
            await File.WriteAllBytesAsync(Path.Combine(capHotelPath, lib), fileBytes);
        }
    }
    public List<Profile> LoadCachedProfiles()
    {
        if (!File.Exists(_cachePath))
            return new List<Profile>();

        var cachedJson = File.ReadAllText(_cachePath);
        return JsonSerializer.Deserialize<List<Profile>>(cachedJson) ?? new List<Profile>();
    }
    public async Task LoadFromApiProfiles()
    {
        DotNetEnv.Env.Load(@"D:\school\APR\SmartClient\.env");

        var body = new
        {
            user = Environment.GetEnvironmentVariable("USER"),
            password = Environment.GetEnvironmentVariable("PASSWORD")
        };

        string jsonBody = System.Text.Json.JsonSerializer.Serialize(body);
        var content = new StringContent(jsonBody);

        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("X-Application-Secret", Environment.GetEnvironmentVariable("X-APPLICATION-SECRET"));

        try
        {
            var response = await _client.PostAsync(url, content);
            System.Diagnostics.Debug.WriteLine(response);

            string responseBody = await response.Content.ReadAsStringAsync();

            var profileResponse = JsonSerializer.Deserialize<ProfileResponse>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (profileResponse?.Members == null || profileResponse.Members.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("API returned null");
                return;
            }

            string jsonData = JsonSerializer.Serialize(profileResponse.Members, new JsonSerializerOptions { WriteIndented = true });

            await File.WriteAllTextAsync(_cachePath, jsonData);

        }
        catch(Exception ex) 
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }
    public async Task StartCapHotel(Profile selectedProfile)
    {
        
        string defaultFileName = "CapHotel.exe";
        string rawNeededVersionFile = defaultFileName + "." + selectedProfile.Version;
        string newVersionPath = Path.Combine(@"C:\CapHotel", "CapHotel" + selectedProfile.Version + ".exe");
        
        string filePath = Path.Combine(_versionsFolderPath, rawNeededVersionFile);
        string downloadUrlPath = "https://www.capcorn.at/download";
        List<Profile> profiles = LoadCachedProfiles();
        
        if (!Directory.Exists(_versionsFolderPath))
        {
            Directory.CreateDirectory(_versionsFolderPath);
        }

        if (!File.Exists(filePath))
        {
            var fileBytes = await _client.GetByteArrayAsync(Path.Combine(downloadUrlPath,rawNeededVersionFile));
            await File.WriteAllBytesAsync(filePath, fileBytes);
        }
        if (!File.Exists(newVersionPath))
        {
            File.Copy(filePath, newVersionPath, true);
        }
        System.Diagnostics.Debug.WriteLine(newVersionPath);
        ProfileBinaryWriter pbw = new ProfileBinaryWriter();

        pbw.RewriteToBinary(selectedProfile);

        Process.Start(new ProcessStartInfo
        {
            FileName = newVersionPath,
            UseShellExecute = true
        });
    }
    public async Task<bool>SaveUserAsync(UserLoginInfo user)
    {
        DotNetEnv.Env.Load(@"D:\school\APR\SmartClient\.env");
        var body = new
        {
            user = user.Username,
            password = user.Password
        };

        string jsonBody = System.Text.Json.JsonSerializer.Serialize(body);
        var content = new StringContent(jsonBody);

        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("X-Application-Secret", Environment.GetEnvironmentVariable("X-APPLICATION-SECRET"));

        var response = await _client.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            if (user.RememberMe)
            {
                var json = JsonSerializer.Serialize(user);
                await File.WriteAllTextAsync(_loginInfoPath, json);
            }
            else
            {
                DeleteUserData();
            }
            return true;
        }
        else
        {
            return false;
        }
    }
    public async Task<UserLoginInfo> LoadUserAsync()
    {
        if (!File.Exists(_loginInfoPath))
        {
            return null;
        }

        var json = await File.ReadAllTextAsync(_loginInfoPath);
        return JsonSerializer.Deserialize<UserLoginInfo>(json);
    }
    public void DeleteUserData()
    {
        if (File.Exists(_loginInfoPath))
            File.Delete(_loginInfoPath);
    }
}
