namespace ProductService.ProductViewModels
{
    public class ProductViewModels
    {
        public class ProductInputDto
        {
            public string Name { get; set; }

            public string Description { get; set; }

            public decimal Price { get; set; }
        }
    }
}