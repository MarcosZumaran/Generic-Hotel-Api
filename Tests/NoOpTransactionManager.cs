using HotelGenericoApi.Services.Interfaces;

namespace HotelGenericoApi.Tests;

public class NoOpTransactionManager : IDbTransactionManager
{
    public Task BeginTransactionAsync() => Task.CompletedTask;
    public Task CommitAsync() => Task.CompletedTask;
    public Task RollbackAsync() => Task.CompletedTask;
    public Task DisposeAsync() => Task.CompletedTask;
}