using System;
using System.Collections.Generic;

namespace MyLibrary.DbModel.Entities;

public partial class Booktheme
{
    public int BookId { get; set; }

    public int ThemeId { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Theme Theme { get; set; } = null!;
}
