using Microsoft.EntityFrameworkCore;
using HotelGenericoApi.Data;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.DTOs.Response;
using HotelGenericoApi.Mappings;
using HotelGenericoApi.Services.Interfaces;
using HotelGenericoApi.Models;

namespace HotelGenericoApi.Services.Implementations;

public class ProductoService : IProductoService
{
    private readonly HotelDbContext _db;
    private readonly ProductoMapper _mapper;

    public ProductoService(HotelDbContext db, ProductoMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductoResponseDto>> GetAllAsync()
    {
        var entities = await _db.Productos
            .Include(p => p.IdAfectacionIgvNavigation)
            .AsNoTracking()
            .ToListAsync();

        return entities.Select(MapToResponse);
    }

    public async Task<ProductoResponseDto?> GetByIdAsync(int id)
    {
        var entity = await _db.Productos
            .Include(p => p.IdAfectacionIgvNavigation)
            .FirstOrDefaultAsync(p => p.IdProducto == id);

        return entity is not null ? MapToResponse(entity) : null;
    }

    public async Task<ProductoResponseDto> CreateAsync(ProductoCreateDto dto)
    {
        var entity = _mapper.FromCreate(dto);
        entity.CreatedAt = DateTime.UtcNow;
        _db.Productos.Add(entity);
        await _db.SaveChangesAsync();

        await _db.Entry(entity).Reference(p => p.IdAfectacionIgvNavigation).LoadAsync();
        return MapToResponse(entity);
    }

    public async Task<bool> UpdateAsync(int id, ProductoUpdateDto dto)
    {
        var entity = await _db.Productos.FindAsync(id);
        if (entity is null) return false;
        _mapper.UpdateFromDto(dto, entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _db.Productos.FindAsync(id);
        if (entity is null) return false;
        _db.Productos.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    private static ProductoResponseDto MapToResponse(Producto p)
    {
        return new ProductoResponseDto(
            p.IdProducto,
            p.CodigoSunat,
            p.Nombre,
            p.Descripcion,
            p.PrecioUnitario,
            p.IdAfectacionIgv,
            p.IdAfectacionIgvNavigation?.Descripcion,
            p.Stock,
            p.StockMinimo,
            p.UnidadMedida,
            p.CreatedAt
        );
    }
}