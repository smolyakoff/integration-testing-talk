using System;

namespace BookShop.Application.Infrastructure.Errors
{
    [Serializable]
    public class ServiceException : Exception
    {
        public ServiceException(Error error) : base(error.Message)
        {
            Error = error;
        }

        public Error Error { get; }
    }
}