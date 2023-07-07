


namespace petshop.ViewModels
{
    using petshop.Models;

    public class ProductViewModel
    {    public Products Product { get; set; }
        public IEnumerable<Products> RelatedProducts { get; set; }
    }
}
