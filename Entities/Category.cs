using System;
using System.Collections.Generic;

namespace cmdev_dotnet_api.Entities;

public partial class Category
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime Created { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
