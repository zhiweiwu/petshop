namespace petshop.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public virtual ICollection<CartItem> CartItems { get; set; }

        //calculate total price
        public decimal GetTotal()
        {
            return CartItems.Sum(item => item.GetSubtotal());
        }
        public decimal GetGst()
        {
            return GetTotal()/11;
        }
    }
}
