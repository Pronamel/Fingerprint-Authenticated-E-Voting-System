using System.Collections.Concurrent;
using Server.Data;

namespace Server.Services;

public class VoterService
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, string>>> _countyVoterCodes;
    private readonly ConcurrentDictionary<string, DateTime> _activeVotingSessions;
    private readonly ApplicationDbContext _dbContext;

    public VoterService(
        ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, string>>> countyVoterCodes,
        ConcurrentDictionary<string, DateTime> activeVotingSessions,
        ApplicationDbContext dbContext)
    {
        _countyVoterCodes = countyVoterCodes;
        _activeVotingSessions = activeVotingSessions;
        _dbContext = dbContext;
    }

    public async Task<bool> RequestAccess(string voterId, string county, string constituency, string deviceName = "Unknown")
    {
        try
        {
            if (string.IsNullOrEmpty(voterId))
                return false;

            if (string.IsNullOrEmpty(county))
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Cannot process request: Invalid county for voter {voterId}");
                return false;
            }

            if (_activeVotingSessions.ContainsKey(voterId))
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Voter {voterId} already has active session");
                return false;
            }

            var requestMessage = $"Voter {voterId} requesting access from {deviceName}";

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {requestMessage} in {county}/{constituency}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Error processing voter request: {ex.Message}");
            return false;
        }
    }
    public bool RevokeVoterSession(string voterId)
    {
        var removed = _activeVotingSessions.TryRemove(voterId, out _);
        if (removed)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Voter {voterId} session revoked");
        }
        return removed;
    }
}