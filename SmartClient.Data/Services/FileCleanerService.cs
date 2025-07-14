using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

public class FileCleanerService
{
    private Timer _timer;
    private readonly string _folderPath;

    public FileCleanerService(string folderPath)
    {
        _folderPath = folderPath;
    }

    public void Start()
    {
        _timer = new Timer(async _ => await CleanUpAsync(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
    }

    public void Stop()
    {
        _timer?.Change(Timeout.Infinite, 0);
        _timer?.Dispose();
    }

    public void CleanUpNow()
    {
        _ = CleanUpAsync();
    }


    private async Task CleanUpAsync()
    {
        var filesToCheck = Directory.GetFiles(_folderPath);
        var regex = new Regex(@"^CapHotel\d+\.exe$", RegexOptions.IgnoreCase);

        foreach (var filePath in filesToCheck)
        {
            string fileName = Path.GetFileName(filePath);

            if (!regex.IsMatch(fileName))
                continue;
            try
            {
                using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    stream.Close();
                    File.Delete(filePath);
                    System.Diagnostics.Debug.WriteLine($"Deleted: {filePath}");
                }
            }
            catch (IOException)
            {
                System.Diagnostics.Debug.WriteLine($"Skipped (in use): {filePath}");
            }
            catch (UnauthorizedAccessException)
            {
                System.Diagnostics.Debug.WriteLine($"Skipped (no access): {filePath}");
            }
        }
        await Task.CompletedTask;
    }

}
