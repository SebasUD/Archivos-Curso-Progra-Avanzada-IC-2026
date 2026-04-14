using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using ParkingSystemApp.Configuration;
using ParkingSystemApp.Data;

namespace ParkingSystemApp.Examples;

/// <summary>
/// Comprehensive examples of how to use the Parking System DbContext
/// and leverage the computed properties in CarEntry.
/// </summary>
public class UsageExamples
{
    /// <summary>
    /// Example 1: Query completed parking sessions with fees calculation
    /// </summary>
    public static async Task Example1_QueryCompletedSessionsAsync(
        ParkingSystemDbContext context)
    {
        Console.WriteLine("\n=== Example 1: Completed Parking Sessions with Fees ===\n");

        // Get all completed sessions with related data
        var completedSessions = await context.CarEntries
            .Where(ce => ce.ExitDateTime != null)
            .Include(ce => ce.Parking)
            .Include(ce => ce.Automobile)
            .OrderBy(ce => ce.EntryDateTime)
            .ToListAsync();

        Console.WriteLine($"Total completed sessions: {completedSessions.Count}\n");

        foreach (var session in completedSessions.Take(5))
        {
            Console.WriteLine($"Session ID: {session.Consecutive}");
            Console.WriteLine($"  Vehicle: {session.Automobile.Manufacturer} ({session.Automobile.Color})");
            Console.WriteLine($"  Type: {session.Automobile.Type}");
            Console.WriteLine($"  Parking: {session.Parking.ParkingName} - ${session.Parking.PricePerHour}/hr");
            Console.WriteLine($"  Entry: {session.EntryDateTime:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"  Exit: {session.ExitDateTime:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"  Duration: {session.StayDurationMinutes} minutes ({session.StayDurationHours:F2} hours)");
            Console.WriteLine($"  Amount to Pay: ${session.TotalAmountToPay:F2}");
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Example 2: Find currently parked vehicles
    /// </summary>
    public static async Task Example2_CurrentlyParkedVehiclesAsync(
        ParkingSystemDbContext context)
    {
        Console.WriteLine("\n=== Example 2: Currently Parked Vehicles ===\n");

        var parkedVehicles = await context.GetCurrentlyParkedVehicles()
            .Include(ce => ce.Parking)
            .Include(ce => ce.Automobile)
            .ToListAsync();

        Console.WriteLine($"Total currently parked: {parkedVehicles.Count}\n");

        if (parkedVehicles.Count == 0)
        {
            Console.WriteLine("No vehicles currently parked.");
            return;
        }

        foreach (var parked in parkedVehicles)
        {
            // Note: Computed properties return NULL for currently parked vehicles
            Console.WriteLine($"Vehicle: {parked.Automobile.Manufacturer} {parked.Automobile.Color}");
            Console.WriteLine($"Parking: {parked.Parking.ParkingName}");
            Console.WriteLine($"Entry Time: {parked.EntryDateTime:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Status: {parked.SessionStatus}");
            Console.WriteLine($"Exit Time: {(parked.ExitDateTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "NOT YET")}");
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Example 3: Calculate parking revenue for a specific parking lot
    /// </summary>
    public static async Task Example3_CalculateRevenueAsync(
        ParkingSystemDbContext context,
        long parkingId)
    {
        Console.WriteLine($"\n=== Example 3: Revenue Calculation for Parking ID: {parkingId} ===\n");

        var parking = await context.ParkingLots.FindAsync(parkingId);
        if (parking == null)
        {
            Console.WriteLine($"Parking lot with ID {parkingId} not found.");
            return;
        }

        Console.WriteLine($"Parking: {parking.ParkingName}");
        Console.WriteLine($"Location: {parking.ProvinceName}");
        Console.WriteLine($"Rate: ${parking.PricePerHour}/hour\n");

        // Get all completed sessions for this parking
        var sessions = await context.CarEntries
            .Where(ce => ce.ParkingId == parkingId && ce.ExitDateTime != null)
            .Include(ce => ce.Automobile)
            .OrderBy(ce => ce.EntryDateTime)
            .ToListAsync();

        Console.WriteLine($"Total sessions completed: {sessions.Count}\n");

        decimal totalRevenue = 0;
        foreach (var session in sessions)
        {
            decimal sessionFee = session.TotalAmountToPay ?? 0;
            totalRevenue += sessionFee;

            Console.WriteLine($"  {session.Automobile.Manufacturer} | " +
                            $"{session.StayDurationHours:F2}h | ${sessionFee:F2}");
        }

        Console.WriteLine($"\nTotal Revenue: ${totalRevenue:F2}");
        
        // Alternative: Use DbContext helper method
        var helperRevenue = context.GetParkingRevenue(parkingId);
        Console.WriteLine($"(Verified via helper method: ${helperRevenue:F2})");
    }

    /// <summary>
    /// Example 4: Query sessions by parking lot and date
    /// </summary>
    public static async Task Example4_SessionsByDateAsync(
        ParkingSystemDbContext context,
        long parkingId,
        DateTime date)
    {
        Console.WriteLine($"\n=== Example 4: Sessions by Date ===\n");
        Console.WriteLine($"Parking ID: {parkingId}");
        Console.WriteLine($"Date: {date:yyyy-MM-dd}\n");

        var sessions = context.GetSessionsByParkingAndDate(parkingId, date);
        var count = await sessions.CountAsync();

        Console.WriteLine($"Sessions on {date:yyyy-MM-dd}: {count}\n");

        foreach (var session in await sessions.ToListAsync())
        {
            Console.WriteLine($"Session {session.Consecutive}:");
            Console.WriteLine($"  Vehicle: {session.Automobile.Manufacturer}");
            Console.WriteLine($"  Entry: {session.EntryDateTime:HH:mm:ss}");
            Console.WriteLine($"  Exit: {session.ExitDateTime?.ToString("HH:mm:ss") ?? "STILL PARKED"}");
            
            if (session.IsSessionCompleted)
            {
                Console.WriteLine($"  Duration: {session.StayDurationMinutes} minutes");
                Console.WriteLine($"  Fee: ${session.TotalAmountToPay:F2}");
            }
            
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Example 5: Advanced filtering with LINQ
    /// </summary>
    public static async Task Example5_AdvancedFilteringAsync(
        ParkingSystemDbContext context)
    {
        Console.WriteLine("\n=== Example 5: Advanced Filtering ===\n");

        // Get all vehicles that have spent more than 3 hours in parking
        Console.WriteLine("Vehicles that spent more than 3 hours:\n");
        
        var longStayVehicles = await context.CarEntries
            .Where(ce => ce.ExitDateTime != null)
            .Include(ce => ce.Automobile)
            .Include(ce => ce.Parking)
            .ToListAsync()
            .ContinueWith(task => task.Result
                .Where(ce => ce.StayDurationHours > 3)
                .OrderByDescending(ce => ce.StayDurationHours)
                .ToList())
            .ConfigureAwait(false);

        foreach (var session in longStayVehicles)
        {
            Console.WriteLine($"Vehicle: {session.Automobile.Manufacturer}");
            Console.WriteLine($"Duration: {session.StayDurationHours:F2} hours ({session.StayDurationMinutes} minutes)");
            Console.WriteLine($"Fee: ${session.TotalAmountToPay:F2}");
            Console.WriteLine();
        }

        // Get sessions that cost more than $5
        Console.WriteLine("\n\nSessions with fees exceeding $5:\n");
        
        var expensiveSessions = await context.CarEntries
            .Where(ce => ce.ExitDateTime != null)
            .Include(ce => ce.Parking)
            .Include(ce => ce.Automobile)
            .ToListAsync()
            .ContinueWith(task => task.Result
                .Where(ce => ce.TotalAmountToPay > 5)
                .OrderByDescending(ce => ce.TotalAmountToPay)
                .ToList())
            .ConfigureAwait(false);

        foreach (var session in expensiveSessions)
        {
            Console.WriteLine($"Amount: ${session.TotalAmountToPay:F2} | " +
                            $"{session.StayDurationHours:F2}h | " +
                            $"{session.Automobile.Manufacturer}");
        }
    }

    /// <summary>
    /// Example 6: Automobile statistics
    /// </summary>
    public static async Task Example6_AutomobileStatisticsAsync(
        ParkingSystemDbContext context)
    {
        Console.WriteLine("\n=== Example 6: Automobile Statistics ===\n");

        var automobiles = await context.Automobiles
            .Include(a => a.CarEntries)
            .ToListAsync();

        foreach (var auto in automobiles.Take(5))
        {
            Console.WriteLine($"Vehicle: {auto.Manufacturer} {auto.Color} ({auto.Year})");
            Console.WriteLine($"Type: {auto.Type}");
            Console.WriteLine($"Total parking sessions: {auto.CarEntries.Count}");

            var completedSessions = auto.CarEntries
                .Where(ce => ce.ExitDateTime != null)
                .ToList();

            if (completedSessions.Any())
            {
                var totalDuration = completedSessions
                    .Sum(ce => ce.StayDurationMinutes ?? 0);
                
                var totalFees = completedSessions
                    .Sum(ce => ce.TotalAmountToPay ?? 0);

                Console.WriteLine($"Completed sessions: {completedSessions.Count}");
                Console.WriteLine($"Total parking time: {totalDuration} minutes");
                Console.WriteLine($"Total fees paid: ${totalFees:F2}");
            }

            Console.WriteLine();
        }
    }

    /// <summary>
    /// Example 7: Monthly revenue report
    /// </summary>
    public static async Task Example7_MonthlyRevenueReportAsync(
        ParkingSystemDbContext context)
    {
        Console.WriteLine("\n=== Example 7: Monthly Revenue Report ===\n");

        var allSessions = await context.CarEntries
            .Where(ce => ce.ExitDateTime != null)
            .Include(ce => ce.Parking)
            .ToListAsync();

        // Group by year-month
        var byMonth = allSessions
            .GroupBy(ce => new { 
                ce.EntryDateTime.Year, 
                ce.EntryDateTime.Month 
            })
            .OrderBy(g => g.Key.Year)
            .ThenBy(g => g.Key.Month);

        foreach (var monthGroup in byMonth)
        {
            var monthStr = new DateTime(monthGroup.Key.Year, monthGroup.Key.Month, 1)
                .ToString("MMMM yyyy");

            var monthRevenue = monthGroup
                .Sum(ce => ce.TotalAmountToPay ?? 0);

            Console.WriteLine($"{monthStr}: ${monthRevenue:F2}");
        }

        Console.WriteLine($"\nTotal Revenue: ${allSessions.Sum(ce => ce.TotalAmountToPay ?? 0):F2}");
    }
}
