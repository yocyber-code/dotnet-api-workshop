using System;
using System.Collections.Generic;

namespace cmdev_dotnet_api.Entities;

public partial class Role
{
    public int RoleId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime Created { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
