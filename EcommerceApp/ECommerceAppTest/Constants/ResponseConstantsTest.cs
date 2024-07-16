namespace ECommerceAppTest.Constants
{
    public static class ResponseConstantsTest
    {
        public const string BadRequest = "Request can not be completed.Please try again";
        public const string ServerError = "Some internal error occurred.Please try again";
        public const string NoRecord = "No records are found. Empty result";
        public const string NullOrInvalidProductId = "Product id value can not be null, Zero and less than zero.";
        public const string NullOrInvalidCategoryId = "Category id value can not be null, Zero and less than zero.";


        //products constants
        public const string NoRecordWithProductId = "No records are found with entered product id";
        public const string CategoryIdNotPresent = "Entered category id is not found.Please enter correct vategory id";

        public const string ProductAlreadyPresent = "Product with provided name is alreday present. add another product";
        public const string ProductNotAdded = "Product is not added. Insertion failed, Please Check CategoryId provided.";
        public const string ProductUpdateSuccess = "Product details are updated successfully.";
        public const string ProductUpdateFailed = "Product details are not updated. Product update failed, Please check entered product or category id";
        public const string ProductDeleteSuccess = "Product details are deleted successfully.";
        public const string ProductDeleteFailed = "Product details are not deleted. Product deletion failed, Please check entered product id";

        //category constants
        public const string NoRecordWithCategoryId = "No record is found with entered Category id";
        public const string CategoryAlreadyPresent = "Category with provided name is alreday present. add another Category";
        public const string CategoryNotAdded = "Category is not added. Insertion failed, Please try again";
        public const string CategoryUpdateSuccess = "Category details are updated successfully.";
        public const string CategoryUpdateFailed = "Category details are not updated. Category update failed";
        public const string CategoryDeleteSuccess = "Category details are deleted successfully.";
        public const string CategoryDeleteFailed = "Category details are not deleted. Product deletion failed";


    }
}
