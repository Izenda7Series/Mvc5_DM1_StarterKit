using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mvc5StarterKit.Models
{
    #region ViewModels
    public class LoginViewModel
    {
        [Display(Name = "Tenant")]
        public string Tenant { get; set; }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class CreateUserViewModel
    {
        [Display(Name = "Tenant*")]
        public IEnumerable<string> Tenants { get; set; }

        [Display(Name = "Selected Tenant")]
        public string SelectedTenant { get; set; }

        [Display(Name = "Seleted Role")]
        public string SelectedRole { get; set; }

        [Display(Name = "Is Admin")]
        public bool IsAdmin { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "User ID")]
        public string UserID { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
    }

    public class CreateTenantViewModel
    {
        [Required]
        [Display(Name = "Tenant ID")]
        public string TenantID { get; set; }

        [Required]
        [Display(Name = "Tenant Name")]
        public string TenantName { get; set; }
    }
    #endregion
}
