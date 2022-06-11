using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static ApiPeliculas.Models.Pelicula;

namespace ApiPeliculas.Models.Dtos
{
    public class PeliculaDto
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string RutaImagen { get; set; }
        [Required]
        public string Descripcion { get; set; }
        [Required]
        public string Duracion { get; set; }             
        public TipoClasificacion Clasificacion { get; set; }

        public int categoriaId { get; set; }
        
        public Categoria Categoria { get; set; }
    }
}
