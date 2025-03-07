namespace ERP.Web.Services;

using ERP.Web.Data;
using ERP.Web.Domain.Dto;
using ERP.Web.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ERP.Web.Services;



public interface IEmpleadoServices
{
    Task<List<EmpleadosDto>> Consultar(string filtro);
    Task<bool> Crear(EmpleadosDto request);
    Task<bool> Eliminar(int Id);
    Task<bool> Modificar(EmpleadosDto request);
}




public class EmpleadoService : IEmpleadosServices
{
    private readonly AppDbContext _context;
    public EmpleadoService(AppDbContext context)
    {
        _context = context;
    }
    ///Consultar los Empleados existentes

    public async Task<List<ClienteDto>> Consultar(string filtro)
    {
        var clientes = await
            _context.Empleados
            .Include(c => c.DatosPersonales)
            .Where(c => c.DatosPersonales.Nombre.Contains(filtro))
            .Select(
                c =>
                new EmpleadosDto()
                {
                    Id = c.Id,
                    PersonaId = c.PersonaId,
                    DatosPersonales = new PersonaDto()
                    {
                        Id = c.DatosPersonales.Id,
                        Nombre = c.DatosPersonales.Nombre,
                        FechaDeNacimiento = c.DatosPersonales.FechaDeNacimiento
                    }
                }
            )
            .ToListAsync();
       
    }


               