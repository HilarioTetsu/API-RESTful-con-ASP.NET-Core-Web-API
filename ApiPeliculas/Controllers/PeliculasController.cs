using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Controllers
{

    [Authorize]
    [Route("api/Peliculas")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "ApiPeliculas")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class PeliculasController : Controller
    {
        private readonly IPeliculaRepository _pelRepo;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public PeliculasController(IPeliculaRepository pelRepo, IMapper mapper, IWebHostEnvironment hostingEnvironment)
        {
            _pelRepo = pelRepo;
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// Obtener todoas las peliculas
        /// </summary>
        /// <returns></returns>

        [AllowAnonymous]
        [HttpGet]        
        [ProducesResponseType(200, Type = typeof(List<PeliculaDto>))]
        [ProducesResponseType(400)]


        public IActionResult GetPeliculas() {
            var listaPeliculas = _pelRepo.GetPeliculas();

            var listaPeliculasDto = new List<PeliculaDto>();

            foreach (var lista in listaPeliculas)
            {
                listaPeliculasDto.Add(_mapper.Map<PeliculaDto>(lista));
            }

            return Ok(listaPeliculasDto);
        }

        [AllowAnonymous]
        [HttpGet("{peliculaId:int}",Name = "GetPelicula")]
        public IActionResult GetPelicula(int peliculaId) {

            var itemPelicula = _pelRepo.GetPelicula(peliculaId);

            if (itemPelicula==null)
            {
                return NotFound();
            }

            var itemPeliculaDto = _mapper.Map<PeliculaDto>(itemPelicula);

            return Ok(itemPeliculaDto);
        }
        [AllowAnonymous]
        [HttpGet("GetPeliculasEnCategoria/{categoriaId:int}")]
        public IActionResult GetPeliculasEnCategoria(int categoriaId) 
        {

            var listaPelicula = _pelRepo.GetPeliculasEnCategoria(categoriaId);

            if (listaPelicula ==null)
            {
                return NotFound();
            }

            var itemPelicula = new List<PeliculaDto>();

            foreach (var item in listaPelicula)
            {
                itemPelicula.Add(_mapper.Map<PeliculaDto>(item));
            }

            return Ok(itemPelicula);

        }
        [AllowAnonymous]
        [HttpGet("Buscar")]
        public IActionResult Buscar(string nombre) {

            try
            {
                var resultado = _pelRepo.BuscarPelicula(nombre);
                if (resultado.Any())
                {
                    return Ok(resultado);
                }

                return NotFound();
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,"Error recuperando datos de la aplicacion");
            }
        }


        [HttpPost]
        public IActionResult CrearPelicula([FromForm] PeliculaCreateDto PeliculaDto) 
        {

            if (PeliculaDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_pelRepo.ExistePelicula(PeliculaDto.Nombre))
            {
                ModelState.AddModelError("","La pelicula ya existe");
                return StatusCode(404, ModelState);
            }

            //subida de archivos
            var archivo = PeliculaDto.Foto;

            string rutaPrincipal = _hostingEnvironment.WebRootPath;
            var archivos = HttpContext.Request.Form.Files;

            if (archivo.Length>0)
            {
                //Nueva imagen
                var nombreFoto = Guid.NewGuid().ToString();
                var subidas = Path.Combine(rutaPrincipal, @"fotos");
                var extension = Path.GetExtension(archivos[0].FileName);
                using (var fileStreams = new FileStream(Path.Combine(subidas, nombreFoto + extension), FileMode.Create))
                {
                    archivos[0].CopyTo(fileStreams);
                }

                PeliculaDto.RutaImagen = @"\fotos\" + nombreFoto + extension;
            }
           
            var pelicula = _mapper.Map<Pelicula>(PeliculaDto);

            if (!_pelRepo.CrearPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal guardando el registro{pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetPelicula", new { peliculaId = pelicula.Id },pelicula);
        }



        [HttpPatch("{peliculaId:int}", Name = "ActualizarPelicula")]

        public IActionResult ActualizarPelicula(int peliculaId, [FromBody]PeliculaUpdateDto peliculaDto) 
        {
            if (peliculaDto==null || peliculaId!= peliculaDto.Id)
            {
                return BadRequest(ModelState);
            }

            var pelicula = _mapper.Map<Pelicula>(peliculaDto);

            if (!_pelRepo.ActualizarPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal al actualizar el registro{pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }


            return NoContent();
        }


        [HttpDelete("{peliculaId:int}", Name = "BorrarPelicula")]

        public IActionResult BorrarPelicula(int peliculaId)
        {            
            if (!_pelRepo.ExistePelicula(peliculaId))
            {
                return NotFound();
            }

            var pelicula = _pelRepo.GetPelicula(peliculaId);

            if (!_pelRepo.BorrarPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal al borrar el registro{pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }


            return NoContent();
        }


    }
}
