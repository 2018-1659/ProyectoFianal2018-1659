using ERP.Web.Components;
using ERP.Web.Data;
using ERP.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//Configuración de la base de datos
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }));

//Registro de servicios de la aplicación
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IEmpleadoService, EmpleadoService>();

var app = builder.Build();

// Inicialización de la base de datos
using (var scope = app.Services.CreateScope())
{
    try 
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        if (!db.Database.CanConnect())
        {
            logger.LogInformation("Creando base de datos...");
            db.Database.EnsureCreated();
            logger.LogInformation("Base de datos creada exitosamente.");
        }
        else
        {
            logger.LogInformation("Verificando migraciones pendientes...");
            if (db.Database.GetPendingMigrations().Any())
            {
                logger.LogInformation("Aplicando migraciones...");
                db.Database.Migrate();
                logger.LogInformation("Migraciones aplicadas exitosamente.");
            }
            else
            {
                logger.LogInformation("No hay migraciones pendientes.");
            }
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al inicializar la base de datos.");
        throw;
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
