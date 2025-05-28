using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiAlex.Exceptions;
using WebApiAlex.Models;
using WebApiAlex.Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApiAlex.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class VendasController : ControllerBase
    {
        private readonly IServiceBase<Venda> _vendaService;

        public VendasController(IServiceBase<Venda> vendaService)
        {
            _vendaService = vendaService;
        }

        /// <summary>
        /// Obtém todas as vendas cadastradas no sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Venda>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<Venda>> GetAll()
        {
            try
            {
                var vendas = _vendaService.GetAll();
                return vendas.Any() ? Ok(vendas) : NoContent();
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "Erro ao obter vendas",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Obtém uma venda específica pelo ID
        /// </summary>
        /// <param name="id">ID da venda</param>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(Venda), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public ActionResult<Venda> GetById(Guid id)
        {
            try
            {
                var venda = _vendaService.GetById(id);
                return venda != null ? Ok(venda) : NotFound(new ProblemDetails
                {
                    Title = "Venda não encontrada",
                    Detail = $"Nenhuma venda encontrada com o ID: {id}",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "Erro ao obter venda",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Adiciona uma nova venda
        /// </summary>
        /// <param name="venda">Dados da venda</param>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(Venda), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public ActionResult<Venda> Add([FromBody] Venda venda)
        {
            try
            {
                // Calcula o valor total da venda baseado nos itens
                if (venda.ItensdaVenda != null && venda.ItensdaVenda.Any())
                {
                    venda.ValorTotal = venda.ItensdaVenda.Sum(item => item.ValorTotal);
                }

                var newVenda = _vendaService.Add(venda);
                return CreatedAtAction(nameof(GetById), new { id = newVenda.Id, version = "1.0" }, newVenda);
            }
            catch (ValidationException ex)
            {
                if (ex.Errors.Count() == 0)
                {
                    ModelState.AddModelError("Erro de validação", ex.Message);
                }
                else
                {

                    foreach (var error in ex.Errors)
                    {
                        foreach (var message in error.Value)
                        {
                            ModelState.AddModelError(error.Key, message);
                        }
                    }
                }
                return ValidationProblem(ModelState);
            }
            catch (ConflictException ex)
            {
                return Problem(
                    title: "Conflito ao criar venda",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status409Conflict);
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "Erro ao criar venda",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>
        /// Remove uma venda
        /// </summary>
        /// <param name="id">ID da venda</param>
        [Authorize]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _vendaService.Delete(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return Problem(
                    title: "Venda não encontrada",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status404NotFound);
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "Erro ao excluir venda",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}