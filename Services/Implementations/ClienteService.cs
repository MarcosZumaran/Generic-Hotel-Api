using Microsoft.EntityFrameworkCore;
using HotelGenericoApi.Data;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.DTOs.Response;
using HotelGenericoApi.Mappings;
using HotelGenericoApi.Services.Interfaces;
using HotelGenericoApi.Extensions;
using HotelGenericoApi.Models;
using HotelGenericoApi.Models.Exceptions;

namespace HotelGenericoApi.Services.Implementations;

public class ClienteService : IClienteService
{
    private readonly HotelDbContext _db;
    private readonly ClienteMapper _mapper;

    public ClienteService(HotelDbContext db, ClienteMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ClienteResponseDto>> GetAllAsync()
    {
        var entities = await _db.Clientes.AsNoTracking().ToListAsync();
        return entities.Select(_mapper.ToResponse);
    }

    public async Task<ClienteResponseDto?> GetByIdAsync(int id)
    {
        var entity = await _db.Clientes.FindAsync(id);
        return entity is not null ? _mapper.ToResponse(entity) : null;
    }

    public async Task<ClienteResponseDto?> GetByDocumentoAsync(string tipoDocumento, string documento)
    {
        var entity = await _db.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.TipoDocumento == tipoDocumento && c.Documento == documento);
        return entity is not null ? _mapper.ToResponse(entity) : null;
    }

    public async Task<ClienteResponseDto> CreateAsync(ClienteCreateDto dto)
    {
        // Verificar si ya existe un cliente con ese documento
        var existente = await GetByDocumentoAsync(dto.TipoDocumento, dto.Documento);
        if (existente is not null)
            throw new BusinessRuleViolationException(BusinessErrorCode.ClientDuplicate, "Ya existe un cliente con ese tipo y número de documento.");

        var entity = _mapper.FromCreate(dto);
        entity.FechaRegistro = DateTime.UtcNow;
        _db.Clientes.Add(entity);
        await _db.SaveChangesAsync();
        return _mapper.ToResponse(entity);
    }

    public async Task<bool> UpdateAsync(int id, ClienteUpdateDto dto)
    {
        var entity = await _db.Clientes.FindAsync(id);
        if (entity is null) return false;
        _mapper.UpdateFromDto(dto, entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _db.Clientes.FindAsync(id);
        if (entity is null) return false;
        _db.Clientes.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<PagedResult<ClienteResponseDto>> GetPagedAsync(int page, int pageSize)
    {
        var query = _db.Clientes.AsNoTracking();
        var paged = await query.ToPagedResultAsync(page, pageSize);
        var dtos = paged.Items.Select(_mapper.ToResponse).ToList();
        return new PagedResult<ClienteResponseDto>
        {
            Items = dtos,
            TotalItems = paged.TotalItems,
            Page = paged.Page,
            PageSize = paged.PageSize
        };
    }

    public async Task<IEnumerable<ClienteResponseDto>> BuscarAsync(string termino, int maxResults)
    {
        return await _db.Clientes
            .Where(c => c.Nombres.Contains(termino) || c.Apellidos.Contains(termino) || c.Documento.Contains(termino))
            .Take(maxResults)
            .Select(c => new ClienteResponseDto(
                c.IdCliente,
                c.TipoDocumento,
                c.Documento,
                c.Nombres,
                c.Apellidos,
                c.Nacionalidad,
                c.FechaNacimiento,
                c.Telefono,
                c.Email,
                c.Direccion,
                c.FechaRegistro ?? DateTime.UtcNow,
                c.FechaVerificacionReniec
            ))
            .ToListAsync();
    }
}