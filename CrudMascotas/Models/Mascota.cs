namespace CrudMascotas.Models
{
    public class Mascota
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Especie { get; set; } = string.Empty;
        public string Raza { get; set; } = string.Empty;
        public int Edad { get; set; }
        public string NombreDueno { get; set; } = string.Empty;
        public string TelefonoDueno { get; set; } = string.Empty;
        public string? FotoRuta { get; set; }
    }
}
