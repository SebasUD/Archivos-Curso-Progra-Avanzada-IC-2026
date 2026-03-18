using System;
using System.Collections.Generic;

namespace MyLibrary.DbModel.Entities;

public partial class Bookcopy
{
    public int Id { get; set; }

    public int BookId { get; set; }

    public string? Barcode { get; set; }

    public sbyte Status { get; set; }

    public sbyte PenaltyWeight { get; set; }

    public DateTime? AddedAt { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual ICollection<Borrow> Borrows { get; set; } = new List<Borrow>();
}
