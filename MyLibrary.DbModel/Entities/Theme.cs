using System;
using System.Collections.Generic;

namespace MyLibrary.DbModel.Entities;

public partial class Theme
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Booktheme> Bookthemes { get; set; } = new List<Booktheme>();
}
