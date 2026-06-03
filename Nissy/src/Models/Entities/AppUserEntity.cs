using System.ComponentModel.DataAnnotations.Schema;

namespace Nissy.Models.Entity;

[Table("app_user")]
public class AppUserEntity
{
    public int Id { get; set; }
    public int AccountManagerId { get; set; }
    public string LoginCode { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameKana { get; set; } = string.Empty;
    public int? ServiceManagerFlag { get; set; }
    public int? IncludeListFlag { get; set; }
    public string? AdminClassification { get; set; } = string.Empty;
    public string? JoiningDate { get; set; } = string.Empty;
    public string? RetirementDate { get; set; } = string.Empty;
    public string? AlarmDate1 { get; set; } = string.Empty;
    public string? AlarmName1 { get; set; } = string.Empty;
    public string? AlarmDate2 { get; set; } = string.Empty;
    public string? AlarmName2 { get; set; } = string.Empty;
    public string? AlarmDate3 { get; set; } = string.Empty;
    public string? AlarmName3 { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public int CreatedId { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UpdatedId { get; set; }
    public DateTime UpdatedAt { get; set; }
}