using System.ComponentModel.DataAnnotations.Schema;

namespace Nissy.Models.Entity;

[Table("office")]
public class OfficeEntity
{
    public int Id { get; set; }
    public int AccountManagerId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? OfficeName { get; set; } = string.Empty;
    public string? DelegateName { get; set; } = string.Empty;
    public string? OfficialPosition { get; set; } = string.Empty;
    public string? PostCode { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
    public string? Tel { get; set; } = string.Empty;
    public string? Fax { get; set; } = string.Empty;
    public string? MailAddress { get; set; } = string.Empty;
    public int CreatedId { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UpdatedId { get; set; }
    public DateTime UpdatedAt { get; set; }
}