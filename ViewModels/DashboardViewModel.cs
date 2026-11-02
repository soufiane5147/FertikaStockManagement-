namespace FerticaStockManagement.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }

        public int TotalProducts { get; set; }

        public int TotalRawMaterials { get; set; }

        public int TotalSuppliers { get; set; }

        public int TotalProductionOrders { get; set; }

        public int TotalStockEntries { get; set; }

        public int TotalStockOutputs { get; set; }

        public decimal TotalProductStock { get; set; }

        public decimal TotalRawMaterialStock { get; set; }

        public int LowStockProducts { get; set; }

        public int LowStockRawMaterials { get; set; }

        public int PendingProductionOrders { get; set; }

        public int CompletedProductionOrders { get; set; }

        public List<ProductionViewModel> RecentProductionOrders { get; set; }
            = new List<ProductionViewModel>();

        public List<StockMovementViewModel> RecentStockMovements { get; set; }
            = new List<StockMovementViewModel>();
    }
}