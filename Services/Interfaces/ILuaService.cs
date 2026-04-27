public interface ILuaService
{
    /// Ejecuta una función dentro de un script Lua.
    object[] CallFunction(string scriptName, string functionName, params object[] args);

    /// Establece una variable global en el entorno Lua (opcional, para scripts sin función)
    void SetGlobal(string name, object value);

    /// Obtiene el valor de una variable global Lua
    object GetGlobal(string name);
}