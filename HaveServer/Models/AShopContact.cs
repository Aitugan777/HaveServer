namespace AitukServer.Models
{
    public class AShopContact
    {
        public long Id { get; set; }

        public string Contact { get; set; }
        public int ContactTypeId { get; set; }

        public long ShopId { get; set; }
    }
}
