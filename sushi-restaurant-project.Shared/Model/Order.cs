using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sushi_restaurant_project.Shared.Models;

public sealed class Order
{
    public int? TableNumber { get; set; }

    public int? PeopleCount { get; set; }

    public bool IsAllYouCanEat { get; set; }

    public List<OrderItem> Items { get; set; } = [];

    public decimal ItemsSubtotal { get; set; }

    public decimal CoverAmount { get; set; }

    public decimal AllYouCanEatAmount { get; set; }

    public decimal TotalAmount { get; set; }
}
