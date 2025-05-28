using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiAlex.Exceptions;
using WebApiAlex.Models;
using WebApiAlex.Services.Interfaces;

namespace WebApiAlex.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class GarantiasController : ControllerBase
    {
        private readonly IServiceBase<Garantia> _garantiaService;

        public GarantiasController(IServiceBase<Garantia> garantiaService)
        {
            _garantiaService = garantiaService;
        }

        /// <summary>
        /// Obtém todos os planos de garantia cadastrados no sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Garantia>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<Garantia>> GetAll()
        {
            try
            {
                var garantias = _garantiaService.GetAll();
                return garantias.Any() ? Ok(garantias) : NoContent();
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "Erro ao obter garantias",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Obtém um plano de garantia específico pelo ID
        /// </summary>
        /// <param name="id">ID da garantia</param>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(Garantia), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public ActionResult<Garantia> GetById(Guid id)
        {
            try
            {
                var garantia = _garantiaService.GetById(id);
                return garantia != null ? Ok(garantia) : NotFound(new ProblemDetails
                {
                    Title = "Garantia não encontrada",
                    Detail = $"Nenhum plano de garantia encontrado com o ID: {id}",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "Erro ao obter garantia",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Adiciona um novo plano de garantia
        /// </summary>
        /// <param name="garantia">Dados do plano de garantia</param>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(Garantia), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public ActionResult<Garantia> Add([FromBody] Garantia garantia)
        {
            try
            {
                var newGarantia = _garantiaService.Add(garantia);
                return CreatedAtAction(nameof(GetById), new { id = newGarantia.Id, version = "1.0" }, newGarantia);
            }
            catch (ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    foreach (var message in error.Value)
                    {
                        ModelState.AddModelError(error.Key, message);
                    }
                }
                return ValidationProblem(ModelState);
            }
            catch (ConflictException ex)
            {
                return Problem(
                    title: "Conflito ao criar garantia",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status409Conflict);
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "Erro ao criar garantia",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Atualiza um plano de garantia existente
        /// </summary>
        /// <param name="id">ID da garantia</param>
        /// <param name="garantia">Dados atualizados da garantia</param>
        [Authorize]
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(Garantia), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public ActionResult<Garantia> Update(Guid id, [FromBody] Garantia garantia)
        {
            if (id != garantia.Id)
            {
                ModelState.AddModelError("id", "ID da rota não corresponde ao ID da garantia");
                return ValidationProblem(ModelState);
            }

            try
            {
                var updatedGarantia = _garantiaService.Update(garantia);
                return Ok(updatedGarantia);
            }
            catch (ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    foreach (var message in error.Value)
                    {
                        ModelState.AddModelError(error.Key, message);
                    }
                }
                return ValidationProblem(ModelState);
            }
            catch (NotFoundException ex)
            {
                return Problem(
                    title: "Garantia não encontrada",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status404NotFound);
            }
            catch (ConflictException ex)
            {
                return Problem(
                    title: "Conflito ao atualizar garantia",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status409Conflict);
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "Erro ao atualizar garantia",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Remove um plano de garantia
        /// </summary>
        /// <param name="id">ID da garantia</param>
        [Authorize]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _garantiaService.Delete(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return Problem(
                    title: "Garantia não encontrada",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status404NotFound);
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "Erro ao excluir garantia",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}