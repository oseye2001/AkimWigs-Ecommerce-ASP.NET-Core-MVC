using System;

namespace AkimWigs.Web.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        // Propriété pratique pour le calcul
        public decimal Total => UnitPrice * Quantity;
    }
}
