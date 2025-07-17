using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClient.Data.Models;

public class Profile
{
    public string CCID { get; set; }
    public string Name { get; set; }
    public string City { get; set; }
    public string Tele { get; set; }
    public string Mail { get; set; }
    public string Contact { get; set; }
    public string Ip_Address { get; set; }
    public string Port { get; set; }
    public string Version { get; set; }
}

