using Microsoft.EntityFrameworkCore;
using HotelGenericoApi.Data;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.DTOs.Response;
using HotelGenericoApi.Mappings;
using HotelGenericoApi.Services.Interfaces;

namespace HotelGenericoApi.Services.Implementations;

public class CatRolUsuarioService : ICatRolUsuarioService
{
    private readonly HotelDbContext _db;
    private readonly RolUsuarioMapper _mapper;

    public CatRolUsuarioService(HotelDbContext db, RolUsuarioMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CatRolUsuarioResponseDto>> GetAllAsync()
    {
        var entities = await _db.RolesUsuario.AsNoTracking().ToListAsync();
        return entities.Select(_mapper.ToResponse);
    }

    public async Task<CatRolUsuarioResponseDto?> GetByIdAsync(int id)
    {
        var entity = await _db.RolesUsuario.FindAsync(id);
        return entity is not null ? _mapper.ToResponse(entity) : null;
    }

    public async Task<CatRolUsuarioResponseDto> CreateAsync(CatRolUsuarioCreateDto dto)
    {
        var entity = _mapper.FromCreate(dto);
        _db.RolesUsuario.Add(entity);
        await _db.SaveChangesAsync();
        return _mapper.ToResponse(entity);
    }

    public async Task<bool> UpdateAsync(int id, CatRolUsuarioUpdateDto dto)
    {
        var entity = await _db.RolesUsuario.FindAsync(id);
        if (entity is null) return false;
        _mapper.UpdateFromDto(dto, entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _db.RolesUsuario.FindAsync(id);
        if (entity is null) return false;
        _db.RolesUsuario.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
}