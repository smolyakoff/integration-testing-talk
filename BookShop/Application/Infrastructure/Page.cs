using System.Collections.Generic;

namespace BookShop.Application.Infrastructure
{
    public class Page<T>
    {
        public Page(List<T> items)
        {
            Items = items;
        }

        public List<T> Items { get; }
    }
}