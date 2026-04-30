-- hotel_tax_rules.lua
-- Calcula el IGV según el código de afectación SUNAT y el tipo de comprobante.
-- Parámetros:
--   1. afectacion_codigo (string)   ej: "10" (Gravado)
--   2. base_imponible (number)      monto sin IGV
--   3. tipo_comprobante (string)    "03" (Boleta) o "01" (Factura)
-- Retorna: tabla { tasa = number, monto = number }

function Calculate_igv_hotel(afectacion_codigo, base_imponible, tipo_comprobante)
    local tasas = {
        ["10"] = 18,   -- Gravado estándar
        ["20"] = 0,    -- Exonerado
        ["30"] = 0,    -- Inafecto
        ["40"] = 0     -- Exportación
    }

    local tasa = tasas[afectacion_codigo] or 0

    -- Si es boleta de hospedaje y está gravado, aplicar tasa reducida del 10.5%
    if tipo_comprobante == "03" and afectacion_codigo == "10" then
        tasa = 10.5
    end

    local monto = base_imponible * (tasa / 100)
    return { tasa = tasa, monto = monto }
end