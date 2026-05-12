using NLua;
using System.Collections.Concurrent;
using HotelGenericoApi.Models.Exceptions;

namespace HotelGenericoApi.Services.Implementations;

public class LuaService : ILuaService
{
    private readonly Lua _lua;
    private readonly string _scriptPath;
    private readonly ConcurrentDictionary<string, bool> _loadedScripts = new();

    public LuaService(IConfiguration configuration)
    {
        _lua = new Lua();
        _lua.LoadCLRPackage();
        _scriptPath = configuration["LuaScriptsPath"] ?? "Scripts";
    }

    private void EnsureScriptLoaded(string scriptName)
    {
        if (!_loadedScripts.ContainsKey(scriptName))
        {
            string fullPath = Path.Combine(_scriptPath, scriptName);
            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"Script Lua no encontrado: {fullPath}");

            _lua.DoFile(fullPath);
            _loadedScripts[scriptName] = true;
        }
    }

    public object[] CallFunction(string scriptName, string functionName, params object[] args)
    {
        EnsureScriptLoaded(scriptName);
        var func = _lua.GetFunction(functionName);
        if (func == null)
            throw new InvalidOperationException(
                $"Función '{functionName}' no encontrada en el script '{scriptName}'"
            );
        return func.Call(args) ?? Array.Empty<object>();
    }

    public void SetGlobal(string name, object value)
    {
        _lua[name] = value;
    }

    public object GetGlobal(string name)
    {
        return _lua[name];
    }
}