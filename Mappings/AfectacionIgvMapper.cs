using HotelGenericoApi.Models;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.DTOs.Response;
using Riok.Mapperly.Abstractions;

namespace HotelGenericoApi.Mappings;

[Mapper]
public partial class AfectacionIgvMapper
{
    public partial CatAfectacionIgvResponseDto ToResponse(AfectacionIgv entity);
    public partial AfectacionIgv FromCreate(CatAfectacionIgvCreateDto dto);
    public partial void UpdateFromDto(CatAfectacionIgvUpdateDto dto, AfectacionIgv entity);
}