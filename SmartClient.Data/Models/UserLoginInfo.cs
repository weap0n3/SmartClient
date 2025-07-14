using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClient.Data.Models;

public class UserLoginInfo
{
    public string Username { get; set; }
    public string Password { get; set; }
    public bool RememberMe { get; set; }
    public UserLoginInfo(string username,string password, bool rememberme)
    {
        this.Username = username;
        this.Password = password;
        this.RememberMe = rememberme;
    }
}
