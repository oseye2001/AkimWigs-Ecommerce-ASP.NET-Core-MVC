using System.ComponentModel.DataAnnotations;

namespace AkimWigs.Web.Models
{
    public class CheckoutViewModel
    {
        // Infos client
        [Required]
        [Display(Name = "Nom complet")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Téléphone")]
        public string Phone { get; set; } = string.Empty;

        // Adresse
        [Required]
        [Display(Name = "Adresse")]
        public string AddressLine1 { get; set; } = string.Empty;

        [Display(Name = "Complément d'adresse")]
        public string? AddressLine2 { get; set; }

        [Required]
        [Display(Name = "Ville")]
        public string City { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Province / État")]
        public string Province { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Code postal")]
        public string PostalCode { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Pays")]
        public string Country { get; set; } = "Canada";

        // Récap côté serveur (lecture seule dans la vue)
        public decimal Subtotal { get; set; }
        public decimal Delivery { get; set; }
        public decimal Taxes { get; set; }
        public decimal Total { get; set; }
    }
}
