using System;
using System.Collections.Generic;

namespace MyLibrary.DbModel.Entities;

public partial class Book
{
    public int Id { get; set; }

    public string? Isbn { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Publisher { get; set; }

    public DateOnly? PublishedDate { get; set; }

    public int? Pages { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Bookauthor> Bookauthors { get; set; } = new List<Bookauthor>();

    public virtual ICollection<Bookcopy> Bookcopies { get; set; } = new List<Bookcopy>();

    public virtual ICollection<Booktheme> Bookthemes { get; set; } = new List<Booktheme>();
}
