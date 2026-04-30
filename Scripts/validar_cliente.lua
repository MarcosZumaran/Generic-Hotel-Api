function Validar(documento, tipo)
    if tipo == "DNI" then
        return #documento == 8
    elseif tipo == "RUC" then
        return #documento == 11
    else
        return false
    end
end