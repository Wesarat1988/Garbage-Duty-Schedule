using Garbage_Duty_Schedule.Data;
using Garbage_Duty_Schedule.Models;
using Microsoft.EntityFrameworkCore;

namespace Garbage_Duty_Schedule.Services;

public class RotationService
{
    private readonly AppDbContext _db;

    public RotationService(AppDbContext db)
    {
        _db = db;
    }

    public async Task GenerateMonthAsync(int year, int month, IReadOnlyCollection<DayOfWeek> skipDays)
    {
        if (month is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(month));
        }

        var firstDay = new DateOnly(year, month, 1);
        var daysInMonth = DateTime.DaysInMonth(year, month);

        var members = await _db.Members
            .Where(m => m.IsActive)
            .OrderBy(m => m.OrderIndex)
            .ThenBy(m => m.Id)
            .ToListAsync();

        if (members.Count == 0)
        {
            return;
        }

        var lastDuty = await _db.Duties
            .Where(d => d.Date < firstDay && d.MemberId != null)
            .OrderByDescending(d => d.Date)
            .FirstOrDefaultAsync();

        var startIndex = 0;
        if (lastDuty?.MemberId is int lastMemberId)
        {
            var idx = members.FindIndex(m => m.Id == lastMemberId);
            if (idx >= 0)
            {
                startIndex = (idx + 1) % members.Count;
            }
        }

        using var enumerator = Cycle(members, startIndex).GetEnumerator();
        var skipSet = skipDays is { Count: > 0 }
            ? new HashSet<DayOfWeek>(skipDays)
            : new HashSet<DayOfWeek>();

        for (var day = 1; day <= daysInMonth; day++)
        {
            var date = new DateOnly(year, month, day);

            if (skipSet.Contains(date.DayOfWeek))
            {
                await SetHolidayAsync(date, "Skip day");
                continue;
            }

            enumerator.MoveNext();
            var member = enumerator.Current;
            await SetDutyAsync(date, member.Id, isHoliday: false, note: null);
        }

        await _db.SaveChangesAsync();
    }

    public async Task SetDutyAsync(DateOnly date, int? memberId, bool isHoliday, string? note)
    {
        var duty = await GetOrCreateDutyAsync(date);
        duty.MemberId = memberId;
        duty.IsHoliday = isHoliday;
        duty.Note = note;
    }

    public async Task SetMemberAsync(DateOnly date, int? memberId)
    {
        var duty = await GetOrCreateDutyAsync(date);
        duty.MemberId = memberId;
        duty.IsHoliday = memberId is null && duty.IsHoliday;
        if (memberId is not null)
        {
            duty.IsHoliday = false;
        }
    }

    public async Task SetHolidayAsync(DateOnly date, string? note)
    {
        var duty = await GetOrCreateDutyAsync(date);
        duty.MemberId = null;
        duty.IsHoliday = true;
        duty.Note = note;
    }

    public async Task SetNoteAsync(DateOnly date, string? note)
    {
        var duty = await GetOrCreateDutyAsync(date);
        duty.Note = note;
    }

    private async Task<Duty> GetOrCreateDutyAsync(DateOnly date)
    {
        var duty = await _db.Duties.FirstOrDefaultAsync(d => d.Date == date);
        if (duty is null)
        {
            duty = new Duty { Date = date };
            _db.Duties.Add(duty);
        }

        return duty;
    }

    private static IEnumerable<Member> Cycle(IReadOnlyList<Member> members, int startIndex)
    {
        var index = startIndex;
        while (true)
        {
            yield return members[index];
            index = (index + 1) % members.Count;
        }
    }
}
