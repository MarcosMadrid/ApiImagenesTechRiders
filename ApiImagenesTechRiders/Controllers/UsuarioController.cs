using ApiImagenesTechRiders.Helpers;
using ApiImagenesTechRiders.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ApiImagenesTechRiders.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private HelperFilesManager helperFilesManager;
        private HelperToken helperToken;

        public UsuarioController(HelperFilesManager helperFilesManager, HelperToken helperToken)
        {
            this.helperFilesManager = helperFilesManager;
            this.helperToken = helperToken;
        }

        // POST: UploadImgPublic
        /// <summary>
        /// Crea una imagen en el server mediante su Nombre,ID.
        /// </summary>
        /// <response code="200">Created. Imagen correctamente creado en el server.</response>        
        /// <response code="400">BadRequest. Mal formada la solicitud.</response>
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        /// <response code="500">Error interno no esperado</response>/
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UploadImgPublic(IFormFile image)
        {
            try
            {
                string urlImagen = await this.helperFilesManager.CreateImg(image.FileName, image.OpenReadStream());
                return Ok(urlImagen);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: UploadImgUsuario
        /// <summary>
        /// Modifica/Crea una imagen en el server mediante su Nombre,ID.
        /// </summary>
        /// <response code="200">Created. Imagen correctamente creado en el server.</response>        
        /// <response code="400">BadRequest. Mal formada la solicitud.</response>
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        /// <response code="500">Error interno no esperado</response>/// 
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UploadImgUsuario(IFormFile image)
        {
            try
            {
                if (image == null)
                {
                    return BadRequest();
                }
                
                //Recupera el token de la apiTechRiders y separa el Bearer
                string token = Request.Headers.Authorization!.ToString().Split(" ").Last();
                if (token == null)
                {
                    return Unauthorized();
                }

                //Decodifica y extrae el usuario del token
                string decodeToken =  await helperToken.DecodeToken(token);
                JObject jsonObject = JsonConvert.DeserializeObject<JObject>(decodeToken)!;
                string userDataString = jsonObject["UserData"]!.ToString();

                Usuario usuario = JsonConvert.DeserializeObject<Usuario>(userDataString)!;
                int IdUserImagen = this.helperFilesManager.GetIdImage(image.FileName);
                if (usuario.IdUsuario != IdUserImagen)
                {
                    return Unauthorized();
                }
                try
                {
                    string urlImagen = await this.helperFilesManager.UpdateImg(image.FileName, image.OpenReadStream());
                    return Ok(urlImagen);
                }catch (FileNotFoundException ex) 
                {
                    string urlImagen = await this.helperFilesManager.CreateImg(image.FileName, image.OpenReadStream());
                    return Ok(urlImagen);
                }                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: DeleteImg
        /// <summary>
        /// Modifica una USUARIOS en la BBDD mediante su ID, tabla USUARIOS
        /// </summary>
        /// <response code="200">Created. Imagen correctamente creado en el server.</response>        
        /// <response code="400">BadRequest. Mal formada la solicitud.</response>
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        /// <response code="500">Error interno no esperado</response>/// 
        [HttpDelete]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteImg()
        {
            try
            {                
                //Recupera el token de la apiTechRiders y separa el Bearer
                string token = Request.Headers.Authorization!.ToString().Split(" ").Last();
                if (token == null)
                {
                    return Unauthorized();
                }

                //Decodifica y extrae el usuario del token
                string decodeToken = await helperToken.DecodeToken(token);
                JObject jsonObject = JsonConvert.DeserializeObject<JObject>(decodeToken)!;
                string userDataString = jsonObject["UserData"]!.ToString();

                Usuario usuario = JsonConvert.DeserializeObject<Usuario>(userDataString)!;
                string fileName = this.helperFilesManager.GetNameImage(usuario.Nombre, usuario.IdUsuario!);
                try
                {
                    this.helperFilesManager.DeleteImg(fileName);
                    return Ok();
                }
                catch (FileNotFoundException ex)
                {                
                    return NotFound(ex.Message);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
