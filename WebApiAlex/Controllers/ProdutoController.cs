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
    public class ProdutosController : ControllerBase
    {
        private readonly IServiceBase<Produto> _produtoService;

        public ProdutosController(IServiceBase<Produto> produtoService)
        {
            _produtoService = produtoService;
        }

        /// <summary>
        /// Obtém todos os produtos cadastrados no sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Produto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<Produto>> GetAll()
        {
            try
            {
                var produtos = _produtoService.GetAll();
                return produtos.Any() ? Ok(produtos) : NoContent();
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "Erro ao obter produtos",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Obtém um produto específico pelo ID
        /// </summary>
        /// <param name="id">ID do produto</param>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(Produto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public ActionResult<Produto> GetById(Guid id)
        {
            try
            {
                var produto = _produtoService.GetById(id);
                return produto != null ? Ok(produto) : NotFound(new ProblemDetails
                {
                    Title = "Produto não encontrado",
                    Detail = $"Nenhum produto encontrado com o ID: {id}",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "Erro ao obter produto",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Adiciona um novo produto
        /// </summary>
        /// <param name="produto">Dados do produto</param>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(Produto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public ActionResult<Produto> Add([FromBody] Produto produto)
        {
            try
            {
                var newProduto = _produtoService.Add(produto);
                return CreatedAtAction(nameof(GetById), new { id = newProduto.Id, version = "1.0" }, newProduto);
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
                    title: "Conflito ao criar produto",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status409Conflict);
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "Erro ao criar produto",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Atualiza um produto existente
        /// </summary>
        /// <param name="id">ID do produto</param>
        /// <param name="produto">Dados atualizados do produto</param>
        [Authorize]
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(Produto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public ActionResult<Produto> Update(Guid id, [FromBody] Produto produto)
        {
            if (id != produto.Id)
            {
                ModelState.AddModelError("id", "ID da rota não corresponde ao ID do produto");
                return ValidationProblem(ModelState);
            }

            try
            {
                var updatedProduto = _produtoService.Update(produto);
                return Ok(updatedProduto);
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
                    title: "Produto não encontrado",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status404NotFound);
            }
            catch (ConflictException ex)
            {
                return Problem(
                    title: "Conflito ao atualizar produto",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status409Conflict);
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "Erro ao atualizar produto",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Remove um produto
        /// </summary>
        /// <param name="id">ID do produto</param>
        [Authorize]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _produtoService.Delete(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return Problem(
                    title: "Produto não encontrado",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status404NotFound);
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "Erro ao excluir produto",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}