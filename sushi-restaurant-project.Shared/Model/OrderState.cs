using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sushi_restaurant_project.Shared.Models;

public sealed class OrderState
{
    public Order Order { get; } = new();
}
