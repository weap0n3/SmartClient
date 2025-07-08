using SmartClient.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;



namespace SmartClient.Data.Services;

public class MemoryService : IMemory
{
    private readonly HttpClient _client;
    private readonly string url = "https://mainframe.capcorn.net/RestService/CapEngineConnectionInfos";

    private readonly string _cachePath;
    public MemoryService(string cachePath)
    {
        this._client = new HttpClient();
        this._cachePath = cachePath;
    }
    public async Task<List<Profile>> LoadCachedProfilesAsync()
    {
        if (!File.Exists(_cachePath))
            return new List<Profile>();

        var cachedJson = await File.ReadAllTextAsync(_cachePath);
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

            if (profileResponse?.Members == null || profileResponse.Members.Count==0)
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


}
