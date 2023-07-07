namespace petshop.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }

        public int ProductsId { get; set; }
        public virtual Products Products { get; set; }

        public int CartId { get; set; }
        public virtual Cart Cart { get; set; }

        //Get subtotal for single product
        public decimal GetSubtotal()
        {
            if (Products.DiscountPrice != null && Products.DiscountPrice < Products.Price)
            {
                return Quantity * (decimal)Products.DiscountPrice;
            }
            else
            {
                return Quantity * Products.Price;
            }
            
        }
    }
}
