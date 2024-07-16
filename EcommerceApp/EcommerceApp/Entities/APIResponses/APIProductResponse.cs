namespace EcommerceApp.Entities.APIResponses
{
    public class APIProductResponse
    {
        public string? Message { get; set; }
    }
    public class AllProductsResponse:APIProductResponse
    {
        public int ProductCount { get; set; }
    }
    public class ProductNotFoundResponse: APIProductResponse
    {
        public int ProductId { get; set; }
    }
    public class ProductSuccessResponse: APIProductResponse
    {
        public int ProductId { get; set; }
    }
    public class ProductNotFoundByCategory: APIProductResponse
    {
        public int CategoryId { get; set; }
    }
    public class ProductErrorResponse 
    {
        public string? ErrorMessage { get; set; }
    }
    public class ProductIdInvalidResponse : ProductErrorResponse
    {
        public int ProductId { get; set; }
    }
    public class ProductAlreadyPresentResponse: ProductErrorResponse
    {
        public string? ProductName { get; set; }
    }
    public class ProductAddFailureResponse : ProductErrorResponse
    {
        public int CategoryId { get; set; }
    }
    public class ProductUpdateFailResponse: ProductErrorResponse
    {
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
    }
    public class ProductDeleteFailResponse : ProductErrorResponse
    {
        public int ProductId { get; set; }
    }

}
