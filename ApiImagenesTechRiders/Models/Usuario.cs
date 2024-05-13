using Newtonsoft.Json;

namespace ApiImagenesTechRiders.Models
{
    public class Usuario
    {
        [JsonProperty("IdUsuario")]
        public int IdUsuario { get; set; }

        [JsonProperty("Nombre")]
        public string? Nombre { get; set; }

        [JsonProperty("Apellidos")]
        public string? Apellidos { get; set; }

        [JsonProperty("Email")]
        public string? Email { get; set; }

        [JsonProperty("Telefono")]
        public string? Telefono { get; set; }

        [JsonProperty("LinkedIn")]
        public string? LinkedIn { get; set; } 

        [JsonProperty("Password")]
        public string? Password { get; set; }

        [JsonProperty("IdRole")]
        public int IdRole { get; set; }

        [JsonProperty("IdProvincia")]
        public int IdProvincia { get; set; }

        [JsonProperty("IdEmpresaCentro")]
        public int? IdEmpresaCentro { get; set; }

        [JsonProperty("Estado")]
        public int Estado { get; set; }

        [JsonProperty("LinkedInVisible")]
        public int LinkedInVisible { get; set; }

        [JsonProperty("Imagen")]
        public string? Imagen { get; set; }
    }
}

