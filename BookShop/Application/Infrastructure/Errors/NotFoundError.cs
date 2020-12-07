using System.Globalization;

namespace BookShop.Application.Infrastructure.Errors
{
    public class NotFoundError : Error
    {
        private static readonly TextInfo TextInfo = CultureInfo.InvariantCulture.TextInfo;

        private NotFoundError(string message) : base(message)
        {
        }

        public static NotFoundError CreateForResource<TKey>(string resourceName, TKey key)
        {
            return new NotFoundError($"The {resourceName.ToLowerInvariant()} with id '{key}' was not found.");
        }
    }
}