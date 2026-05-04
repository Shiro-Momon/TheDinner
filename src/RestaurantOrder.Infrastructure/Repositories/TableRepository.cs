using RestaurantOrder.Application.Interfaces.Repositories;
using RestaurantOrder.Domain.Entities;
using RestaurantOrder.Infrastructure.Persistence;

namespace RestaurantOrder.Infrastructure.Repositories;

public class TableRepository : Repository<Table>, ITableRepository
{
    public TableRepository(RestaurantDbContext context) : base(context)
    {
    }
}
