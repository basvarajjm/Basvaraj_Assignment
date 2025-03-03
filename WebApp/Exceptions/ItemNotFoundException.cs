namespace WebApp.Exceptions
{
    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message)
        {
        }

        public ItemNotFoundException() : base()
        {
        }
    }
}
