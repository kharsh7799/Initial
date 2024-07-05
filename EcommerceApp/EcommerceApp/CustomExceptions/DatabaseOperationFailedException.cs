namespace EcommerceApp.CustomExceptions
{
    public class DatabaseOperationFailedException:Exception
    {
        public DatabaseOperationFailedException()
        {
                
        }
        public DatabaseOperationFailedException(string message):base(message) 
        {
                
        }
    }
}
