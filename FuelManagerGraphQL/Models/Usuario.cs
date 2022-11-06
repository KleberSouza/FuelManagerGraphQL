using System.ComponentModel.DataAnnotations;

namespace FuelManagerGraphQL.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string Perfil { get; set; }
    }
}
