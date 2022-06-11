using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Models.Dtos
{
    public class UsuarioAuthDto
    {
        public int Id { get; set; }
        [Required]
        public string Usuario { get; set; }
        [Required]
        [StringLength(10,MinimumLength =4)]
        public string Password { get; set; }


    }
}
