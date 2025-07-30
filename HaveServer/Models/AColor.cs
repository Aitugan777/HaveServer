namespace AitukServer.Models
{
    public class AColor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<AProduct> Products { get; set; }
    }
}
