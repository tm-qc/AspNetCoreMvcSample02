using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nissy.Models.Entity;

[Table("driver")]
public class DriverEntity
{
    public int Id { get; set; }
    public int ServiceTypeId { get; set; }
    public int AccountManagerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public string RegistDate { get; set; } = string.Empty;
    public string Other { get; set; } = string.Empty;
    public string LicenseNo { get; set; } = string.Empty;
    public string ExpireDate { get; set; } = string.Empty;
    public string LicenseDate { get; set; } = string.Empty;
    public string LicenseKind { get; set; } = string.Empty;
    public string LicenseCondition { get; set; } = string.Empty;
    public int CreatedId { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UpdatedId { get; set; }
    public DateTime UpdatedAt { get; set; }
}