using HotelGenericoApi.Services.Interfaces;

namespace HotelGenericoApi.Tests;

public static class TestTransactionFactory
{
    public static IDbTransactionManager Create() => new NoOpTransactionManager();
}