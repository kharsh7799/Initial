namespace EcommerceApp.Entities.APIResponses
{
    public class APICategoriesResponses
    {
        public class APICategoryResponse
        {
            public int CategoryId { get; set; }
            public string? Message { get; set; }
        }
        public class AllCategorysResponse
        {
            public int CategoryCount { get; set; }
            public string? Message { get; set; }

        }
        public class CategoryNotFoundResponse : APICategoryResponse
        {
        }
        public class CategorySuccessResponse : APICategoryResponse
        {
        }
        public class CategoryErrorResponse
        {
            public int CategoryId { get; set; }
            public string? ErrorMessage { get; set; }
        }
        public class CategoryAddFailureResponse
        {
            public string? CategoryName { get; set; }
            public string? ErrorMessage { get; set; }
        }
        public class CategoryUpdateFailResponse : CategoryErrorResponse
        {
        }
        public class CategoryDeleteFailResponse : CategoryErrorResponse
        {
        }
    }
}
