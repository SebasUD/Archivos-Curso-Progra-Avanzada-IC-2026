using System;
using System.Collections.Generic;

namespace MyLibrary.DbModel.Entities;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public int MaxConcurrentBorrows { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Borrow> Borrows { get; set; } = new List<Borrow>();
}
