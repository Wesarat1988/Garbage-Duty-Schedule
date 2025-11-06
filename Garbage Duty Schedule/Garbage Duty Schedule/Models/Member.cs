namespace Garbage_Duty_Schedule.Models;

public class Member
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public int? OrderIndex { get; set; }
}
