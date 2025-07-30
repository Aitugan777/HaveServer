namespace AitukServer.Models
{
    public class AGender
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<AProduct> Products { get; set; }
    }
}
