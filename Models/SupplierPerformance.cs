namespace ProjetERP.Models
{
    public class SupplierPerformance
    {
        public int PerformanceId { get; set; }
        public int SupplierId { get; set; }
        public DateTime EvaluationDate { get; set; }
        public int Score { get; set; }
        public string Comments { get; set; }
        public Supplier Supplier { get; set; }
    }
}