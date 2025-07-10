using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Win32;
using System.Diagnostics;

namespace SmartClient.Data.Models;

public class ProfileBinaryWriter
{
    const int STRING_SIZE = 41;
    private string ccpFilePath = Path.Combine(@"C:\CapHotel", "CapHotel.ccp");
    string user = "CAPUSER";
    string pass = "hermis";


    public void RewriteToBinary(Profile profile)
    {
        int port = Convert.ToInt32(profile.Port);
        File.Delete(ccpFilePath);

        Registry.SetValue(@"HKEY_CURRENT_USER\Software\CapHotel\CapHotel\CONFIG", "CurrentProfile", profile.Name);

        
        using var fs = new FileStream(ccpFilePath, FileMode.Create, FileAccess.Write);
        using (var bw = new BinaryWriter(fs, Encoding.Latin1))
        {
            WriteFixedString(bw, profile.Name);             // Profil - Name
            WriteFixedString(bw, "DB_CAPHOTEL");            // ODBC - Datenquelle
            WriteFixedString(bw, user);                     // Default - User
            WriteFixedString(bw, pass);                     // Default - Passwort
            WriteFixedString(bw, profile.Ip_Address);       // Hostname oder IP
            WriteFixedString(bw, "");                       // Schlüssel
            WriteFixedString(bw, "");                       // Reserve
            bw.Write((byte)0);
            bw.Write((Int32) 60);                           // Timeout in Sekunden
            bw.Write((Int32) 1 );                           // Typ (0 ODBC, 1 CapEngine, 2 CapCloud)
            bw.Write((Int32) port);                         // Port
            bw.Write((Int32) 1);                            // Datenbank - Typ (0 MDB 1 SQL)
            bw.Write((Int32) 1);                            // Schnellanmeldung
            bw.Write((Int32) 1);                            // Backup Info
            
        }

    }

    static void WriteFixedString(BinaryWriter bw, string input)
    {
        byte[] bytes = new byte[STRING_SIZE];
        var strBytes = Encoding.Latin1.GetBytes(input);
        int copyLength = Math.Min(strBytes.Length, STRING_SIZE - 1);
        Array.Copy(strBytes, bytes, copyLength);
        bw.Write(bytes);
    }
}
