using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BookShop.Application.Infrastructure.Errors
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ValidationError : Error
    {
        [JsonConstructor]
        public ValidationError(string title, Dictionary<string, string[]> errors) : base(title)
        {
            Title = title;
            Errors = errors;
        }
        
        public string Title { get; }

        public Dictionary<string, string[]> Errors
        {
            get;
        }
}
}