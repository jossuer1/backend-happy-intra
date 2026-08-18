// Services/ServiceResult.cs
namespace Intranet.Services;

public class ServiceResult<T>
{
    public bool Exito { get; set; }
    public string? Mensaje { get; set; }
    public T? Data { get; set; }

    public static ServiceResult<T> Ok(T data) => new() { Exito = true, Data = data };
    public static ServiceResult<T> Fallo(string mensaje) => new() { Exito = false, Mensaje = mensaje };
}