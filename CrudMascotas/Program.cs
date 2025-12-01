using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? "Data Source=mascotas.db";

using (var connection = new SqliteConnection(connectionString))
{
    connection.Open();
    var cmd = connection.CreateCommand();
    cmd.CommandText = @"
        CREATE TABLE IF NOT EXISTS Mascotas (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Nombre TEXT NOT NULL,
            Especie TEXT,
            Raza TEXT,
            Edad INTEGER,
            NombreDueno TEXT,
            TelefonoDueno TEXT,
            FotoRuta TEXT
        );

        CREATE TABLE IF NOT EXISTS Usuarios (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            UserName TEXT NOT NULL UNIQUE,
            Password TEXT NOT NULL
        );

        INSERT OR IGNORE INTO Usuarios (Id, UserName, Password)
        VALUES (1, 'admin', '1234');
    ";
    cmd.ExecuteNonQuery();
}

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Cuenta/Login";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();   
app.UseAuthorization();   

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Mascotas}/{action=Index}/{id?}");

app.Run();
