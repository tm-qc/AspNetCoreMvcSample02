using System.ComponentModel.DataAnnotations.Schema;

namespace Nissy.Models.Entity;

[Table("service_type")]
public class ServiceTypeEntity
{
    public int Id { get; set; }
    public string ServiceDivision { get; set; } = string.Empty;
    public int CreatedId { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UpdatedId { get; set; }
    public DateTime UpdatedAt { get; set; }
}