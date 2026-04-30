using RestaurantOrder.Domain.Exceptions;

namespace RestaurantOrder.Domain.Entities;

public class Table
{
    private Table()
    {
    }

    public Guid Id { get; private set; }
    public int Number { get; private set; }
    public int Capacity { get; private set; }
    public bool IsOccupied { get; private set; }

    public static Table Create(int number, int capacity)
    {
        if (number <= 0)
            throw new DomainException("Table number must be greater than zero.");
        if (capacity <= 0)
            throw new DomainException("Table capacity must be greater than zero.");

        return new Table
        {
            Id = Guid.NewGuid(),
            Number = number,
            Capacity = capacity,
            IsOccupied = false,
        };
    }

    public void Occupy()
    {
        if (IsOccupied)
            throw new DomainException($"Table {Number} is already occupied.");
        IsOccupied = true;
    }

    public void Release()
    {
        IsOccupied = false;
    }
}
