using System;
using System.Collections.Generic;

namespace Techno_Home.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? Name { get; set; }

    public bool IsAdmin { get; set; }

    public string? Salt { get; set; }

    public string? HashedPw { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
