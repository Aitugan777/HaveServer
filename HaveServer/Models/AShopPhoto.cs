namespace AitukServer.Models
{
    public class AShopPhoto
    {
        public long Id { get; set; }

        public long ShopId { get; set; }
        public AShop Shop { get; set; }
    }
}
