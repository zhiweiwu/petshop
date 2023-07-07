namespace petshop.Models
{
    public class Products
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string UrlImg { get; set; }
        public string Category { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }


    }
}
