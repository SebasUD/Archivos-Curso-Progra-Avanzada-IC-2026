using System;
using System.Collections.Generic;

namespace MyLibrary.DbModel.Entities;

public partial class Bookauthor
{
    public int BookId { get; set; }

    public int AuthorId { get; set; }

    public string? Role { get; set; }

    public virtual Author Author { get; set; } = null!;

    public virtual Book Book { get; set; } = null!;
}
