using Microsoft.EntityFrameworkCore;
using HotelGenericoApi.Data;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.DTOs.Response;
using HotelGenericoApi.Mappings;
using HotelGenericoApi.Services.Interfaces;

namespace HotelGenericoApi.Services.Implementations;

public class CatMetodoPagoService : ICatMetodoPagoService
{
    private readonly HotelDbContext _db;
    private readonly CatMetodoPagoMapper _mapper;

    public CatMetodoPagoService(HotelDbContext db, CatMetodoPagoMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CatMetodoPagoResponseDto>> GetAllAsync()
    {
        var entities = await _db.CatMetodoPagos.AsNoTracking().ToListAsync();
        return entities.Select(_mapper.ToResponse);
    }

    public async Task<CatMetodoPagoResponseDto?> GetByIdAsync(string codigo)
    {
        var entity = await _db.CatMetodoPagos.FindAsync(codigo);
        return entity is not null ? _mapper.ToResponse(entity) : null;
    }

    public async Task<CatMetodoPagoResponseDto> CreateAsync(CatMetodoPagoCreateDto dto)
    {
        var entity = _mapper.FromCreate(dto);
        _db.CatMetodoPagos.Add(entity);
        await _db.SaveChangesAsync();
        return _mapper.ToResponse(entity);
    }

    public async Task<bool> UpdateAsync(string codigo, CatMetodoPagoUpdateDto dto)
    {
        var entity = await _db.CatMetodoPagos.FindAsync(codigo);
        if (entity is null) return false;
        _mapper.UpdateFromDto(dto, entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(string codigo)
    {
        var entity = await _db.CatMetodoPagos.FindAsync(codigo);
        if (entity is null) return false;
        _db.CatMetodoPagos.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
}