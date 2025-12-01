using CrudMascotas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Authorization;


namespace CrudMascotas.Controllers
{
     [Authorize]
    public class MascotasController : Controller
    {
        
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _env;

        public MascotasController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
            _env = env;
        }

        private SqliteConnection GetConnection()
        {
            return new SqliteConnection(_connectionString);
        }

        // GET: Mascotas
        public IActionResult Index()
        {
            var lista = new List<Mascota>();

            using (var con = GetConnection())
            {
                con.Open();
                var cmd = con.CreateCommand();
                cmd.CommandText = "SELECT Id, Nombre, Especie, Raza, Edad, NombreDueno, TelefonoDueno, FotoRuta FROM Mascotas ORDER BY Id;";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Mascota
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Especie = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            Raza = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            Edad = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                            NombreDueno = reader.IsDBNull(5) ? "" : reader.GetString(5),
                            TelefonoDueno = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            FotoRuta = reader.IsDBNull(7) ? null : reader.GetString(7)
                        });
                    }
                }
            }

            return View(lista);
        }

        // GET: Mascotas/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            var mascota = ObtenerMascotaPorId(id.Value);
            if (mascota == null) return NotFound();

            return View(mascota);
        }

        private Mascota? ObtenerMascotaPorId(int id)
        {
            using (var con = GetConnection())
            {
                con.Open();
                var cmd = con.CreateCommand();
                cmd.CommandText = "SELECT Id, Nombre, Especie, Raza, Edad, NombreDueno, TelefonoDueno, FotoRuta FROM Mascotas WHERE Id = $id;";
                cmd.Parameters.AddWithValue("$id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Mascota
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Especie = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            Raza = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            Edad = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                            NombreDueno = reader.IsDBNull(5) ? "" : reader.GetString(5),
                            TelefonoDueno = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            FotoRuta = reader.IsDBNull(7) ? null : reader.GetString(7)
                        };
                    }
                }
            }

            return null;
        }

        // GET: Mascotas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Mascotas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Mascota mascota, IFormFile? foto)
        {
            if (!ModelState.IsValid)
                return View(mascota);

            if (foto != null && foto.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(foto.FileName);
                var uploadPath = Path.Combine(_env.WebRootPath, "images", "mascotas");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    foto.CopyTo(stream);
                }

                mascota.FotoRuta = fileName;
            }

            using (var con = GetConnection())
            {
                con.Open();
                var cmd = con.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO Mascotas
                        (Nombre, Especie, Raza, Edad, NombreDueno, TelefonoDueno, FotoRuta)
                    VALUES
                        ($Nombre, $Especie, $Raza, $Edad, $NombreDueno, $TelefonoDueno, $FotoRuta);";

                cmd.Parameters.AddWithValue("$Nombre", mascota.Nombre);
                cmd.Parameters.AddWithValue("$Especie", mascota.Especie ?? "");
                cmd.Parameters.AddWithValue("$Raza", mascota.Raza ?? "");
                cmd.Parameters.AddWithValue("$Edad", mascota.Edad);
                cmd.Parameters.AddWithValue("$NombreDueno", mascota.NombreDueno ?? "");
                cmd.Parameters.AddWithValue("$TelefonoDueno", mascota.TelefonoDueno ?? "");
                cmd.Parameters.AddWithValue("$FotoRuta", (object?)mascota.FotoRuta ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Mascotas/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var mascota = ObtenerMascotaPorId(id.Value);
            if (mascota == null) return NotFound();

            return View(mascota);
        }

        // POST: Mascotas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Mascota mascota, IFormFile? foto)
        {
            if (id != mascota.Id) return NotFound();
            if (!ModelState.IsValid) return View(mascota);

            if (foto != null && foto.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(foto.FileName);
                var uploadPath = Path.Combine(_env.WebRootPath, "images", "mascotas");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    foto.CopyTo(stream);
                }

                mascota.FotoRuta = fileName;
            }

            using (var con = GetConnection())
            {
                con.Open();
                var cmd = con.CreateCommand();
                cmd.CommandText = @"
                    UPDATE Mascotas SET
                        Nombre = $Nombre,
                        Especie = $Especie,
                        Raza = $Raza,
                        Edad = $Edad,
                        NombreDueno = $NombreDueno,
                        TelefonoDueno = $TelefonoDueno,
                        FotoRuta = $FotoRuta
                    WHERE Id = $Id;";

                cmd.Parameters.AddWithValue("$Id", mascota.Id);
                cmd.Parameters.AddWithValue("$Nombre", mascota.Nombre);
                cmd.Parameters.AddWithValue("$Especie", mascota.Especie ?? "");
                cmd.Parameters.AddWithValue("$Raza", mascota.Raza ?? "");
                cmd.Parameters.AddWithValue("$Edad", mascota.Edad);
                cmd.Parameters.AddWithValue("$NombreDueno", mascota.NombreDueno ?? "");
                cmd.Parameters.AddWithValue("$TelefonoDueno", mascota.TelefonoDueno ?? "");
                cmd.Parameters.AddWithValue("$FotoRuta", (object?)mascota.FotoRuta ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Mascotas/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var mascota = ObtenerMascotaPorId(id.Value);
            if (mascota == null) return NotFound();

            return View(mascota);
        }

        // POST: Mascotas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using (var con = GetConnection())
            {
                con.Open();
                var cmd = con.CreateCommand();
                cmd.CommandText = "DELETE FROM Mascotas WHERE Id = $Id;";
                cmd.Parameters.AddWithValue("$Id", id);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
