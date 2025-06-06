using Domain.Enums;

namespace Domain.Entities;

public class Integration
{
    public int Id { get; set; }
    public IntegrationType IntegrationType { get; set; }
}
