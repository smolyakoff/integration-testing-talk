namespace BookShop.Application.Infrastructure.Errors
{
    public abstract class Error
    {
        protected Error(string message)
        {
            Message = message;
        }

        public string Message { get; }

        public ServiceException ToException()
        {
            return new ServiceException(this);
        }
    }
}