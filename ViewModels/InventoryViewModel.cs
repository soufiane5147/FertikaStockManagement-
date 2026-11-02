using System.Collections.Generic;

namespace FerticaStockManagement.ViewModels
{
    public class InventoryViewModel
    {
        public List<RawMaterialInventoryViewModel> RawMaterials { get; set; }
            = new List<RawMaterialInventoryViewModel>();

        public List<ProductInventoryViewModel> Products { get; set; }
            = new List<ProductInventoryViewModel>();

        public List<StockMovementViewModel> Movements { get; set; }
            = new List<StockMovementViewModel>();

        public List<StockMovementViewModel> RecentMovements { get; set; }
            = new List<StockMovementViewModel>();
    }

    public class RawMaterialInventoryViewModel
    {
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Unit { get; set; } = string.Empty;

        public decimal CurrentStock { get; set; }

        public decimal MinimumStock { get; set; }

        public decimal UnitPrice { get; set; }

        public bool IsLowStock =>
            CurrentStock <= MinimumStock;
    }

    public class ProductInventoryViewModel
    {
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Unit { get; set; } = string.Empty;

        public decimal CurrentStock { get; set; }

        public decimal MinimumStock { get; set; }

        public decimal SellingPrice { get; set; }

        public bool IsLowStock =>
            CurrentStock <= MinimumStock;
    }
}