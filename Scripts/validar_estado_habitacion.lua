-- Recibira el nombre del estado actual y del estado nuevo
-- Retornara true si la trancicion es valido y false si no lo es 

local transiciones = {
    ["Disponible"] = {"Ocupada", "Mantenimiento"},
    ["Ocupada"] = {"Limpieza"},
    ["Limpieza"] = {"Disponible", "Mantenimiento"},
    ["Mantenimiento"] = {"Disponible", "Limpieza"}
};

function Validar_trancision(estado_actual, estado_nuevo)
    if transiciones[estado_actual] == nil then
        return false, "Estado actual desconocido: " .. estado_actual;
    end
    for _, destino in ipairs(transiciones[estado_actual]) do
        if destino == estado_nuevo then
            return true, "Transición Exitosa de " .. estado_actual .. " a " .. estado_nuevo .. ".";
        end
    end
    return false, "Transición no válida de " .. estado_actual .. " a " .. estado_nuevo .. " , accion no permitida.";
end