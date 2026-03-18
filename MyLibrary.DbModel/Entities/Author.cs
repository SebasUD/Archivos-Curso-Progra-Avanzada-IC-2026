using System;
using System.Collections.Generic;

namespace MyLibrary.DbModel.Entities;

public partial class Author
{
    public int Id { get; set; }

    public string? First_Name { get; set; }

    public string? Last_Name { get; set; }

    public string? Bio { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Bookauthor> Bookauthors { get; set; } = new List<Bookauthor>();
}
