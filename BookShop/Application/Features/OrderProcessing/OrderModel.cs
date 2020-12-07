using System;
using System.Collections.Generic;

namespace BookShop.Application.Features.OrderProcessing
{
    public class OrderModel
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; }
        public List<OrderLineModel> Lines { get; set; }
    }
}