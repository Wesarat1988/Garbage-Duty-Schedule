namespace Garbage_Duty_Schedule.Models;

public class Holiday
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public string? Title { get; set; }
}
