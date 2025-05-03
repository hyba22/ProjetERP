namespace ProjetERP.Models
{
    public class Contract
    {
        public int ContractId { get; set; }
        public int SupplierId { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Terms { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public Supplier Supplier { get; set; }
    }
}