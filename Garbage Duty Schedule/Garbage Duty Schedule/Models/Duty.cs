namespace Garbage_Duty_Schedule.Models;

public class Duty
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public int? MemberId { get; set; }

    public Member? Member { get; set; }

    public string? Note { get; set; }

    public bool IsHoliday { get; set; }
}
