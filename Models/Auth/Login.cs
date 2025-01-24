using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Higertech.Models.Auth;
public class Login
{
  public string Username { get; set; } = null!;
  public string Password { get; set; } = null!;
  public bool Rememberme { get; set; } = false;
}
