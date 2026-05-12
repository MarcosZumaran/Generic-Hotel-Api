namespace HotelGenericoApi.Models.Exceptions;

public enum BusinessErrorCode
{
    RoomNotAvailable,
    ClientNotFound,
    ReservationNotFound,
    ReservationConflict,
    EstanciaNotFound,
    EstanciaNotActive,
    ProductNotFound,
    InvalidTransition,
    ClientDuplicate,
    HabitacionDuplicate,
    LuaExecutionError,
    SetupAlreadyDone,
    QuantityInvalid,
    HabitacionExists,
    UserNotFound,
    InvalidCredentials,
    ProductoNotFound,
    ComprobanteNotFound,
    TarifaNotFound,
    EstadoNotFound,
    TransicionNotFound,
    ValidationError
}