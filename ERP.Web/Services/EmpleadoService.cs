using ERP.Web.Data;
using ERP.Web.Domain.Dto;
using ERP.Web.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Services;

public interface IEmpleadoService
{
    Task<List<EmpleadoDto>> ObtenerRegistros(string criterio);
    Task<bool> AgregarNuevo(EmpleadoDto entrada);
    Task<bool> Remover(int Id);
    Task<bool> Actualizar(EmpleadoDto entrada);
}

public class EmpleadoService : IEmpleadoService
{
    private readonly AppDbContext _db;
    public EmpleadoService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<EmpleadoDto>> ObtenerRegistros(string criterio)
    {
        var registros = await
            _db.Empleados
            .Include(e => e.DatosPersonales)
            .Where(e => e.DatosPersonales.Nombre.Contains(criterio))
            .Select(
                e =>
                new EmpleadoDto()
                {
                    Id = e.Id,
                    PersonaId = e.PersonaId,
                    Sueldo = e.Sueldo,
                    DatosPersonales = new PersonaDto()
                    {
                        Id = e.DatosPersonales.Id,
                        Nombre = e.DatosPersonales.Nombre,
                        FechaDeNacimiento = e.DatosPersonales.FechaDeNacimiento
                    }
                }
            )
            .ToListAsync();
        return registros;
    }

    public async Task<bool> AgregarNuevo(EmpleadoDto entrada)
    {
        var registro = Empleado.Create(
            entrada.DatosPersonales.Nombre,
            entrada.DatosPersonales.FechaDeNacimiento,
            entrada.Sueldo
        );
        _db.Empleados.Add(registro);
        return (await _db.SaveChangesAsync()) > 0;
    }

    public async Task<bool> Actualizar(EmpleadoDto entrada)
    {
        var registro = await _db.Empleados
            .Include(e => e.DatosPersonales)
            .FirstOrDefaultAsync(e => e.Id == entrada.Id);
            
        registro!.DatosPersonales.Nombre = entrada.DatosPersonales.Nombre;
        registro!.DatosPersonales.FechaDeNacimiento = entrada.DatosPersonales.FechaDeNacimiento;
        registro!.Sueldo = entrada.Sueldo;
        
        return (await _db.SaveChangesAsync()) > 0;
    }

    public async Task<bool> Remover(int Id)
    {
        var registro = await _db.Empleados
            .FirstOrDefaultAsync(e => e.Id == Id);

        _db.Empleados.Remove(registro!);

        return (await _db.SaveChangesAsync()) > 0;
    }
}