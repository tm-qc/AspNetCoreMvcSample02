using Nissy.Constants;
using System.ComponentModel.DataAnnotations;

namespace Nissy.ViewModels.Driver
{
    public class CreateEditViewModel
    {
        //新規登録・編集 Viewに受け渡しが必要なプロパティを定義

        //サービスからコントローラーに受け渡すためのIdプロパティ
        public int Id { get; set; }

        //public int ServiceTypeId { get; set; }
        //public int AccountManagerId { get; set; }
        [Required(ErrorMessage = Messages.Validation.NameRequired)]
        [MaxLength(30, ErrorMessage = Messages.Validation.NameMaxLength)]
        public string Name { get; set; } = string.Empty;
        public string BirthDate { get; set; } = string.Empty;
        public string RegistDate { get; set; } = string.Empty;
        public string Other { get; set; } = string.Empty;
        public string LicenseNo { get; set; } = string.Empty;
        public string ExpireDate { get; set; } = string.Empty;
        public string LicenseDate { get; set; } = string.Empty;
        public string LicenseKind { get; set; } = string.Empty;
        public string LicenseCondition { get; set; } = string.Empty;
    }
}