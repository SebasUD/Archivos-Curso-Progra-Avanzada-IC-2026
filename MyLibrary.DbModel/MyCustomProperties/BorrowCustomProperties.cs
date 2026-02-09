using System;

namespace MyLibrary.DbModel.Entities;

public partial class Borrow
{
    public int DelayedDays
    {
        get
        {
            int result = 0;

            if (DueDate != null)
            {
                if (ReturnDate != null)
                {
                    // Book has been returned
                    var returnDateOnly = DateOnly.FromDateTime(ReturnDate.Value);
                    if (DueDate.Value < returnDateOnly)
                    {
                        // Returned late
                        result = returnDateOnly.DayNumber - DueDate.Value.DayNumber;
                    }
                }
                else
                {
                    // Book not yet returned
                    var today = DateOnly.FromDateTime(DateTime.Today);
                    if (DueDate.Value < today)
                    {
                        // Currently overdue
                        result = today.DayNumber - DueDate.Value.DayNumber;
                    }
                }
            }

            return result;
        }
    }

    public string StatusName => Status switch
    {
        1 => "Active",
        2 => "Returned",
        3 => "Overdue",
        4 => "Lost",
        _ => "Unknown"
    };
}
