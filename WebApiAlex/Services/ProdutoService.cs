using WebApiAlex.Models;
using WebApiAlex.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace WebApiAlex.Services
{
    public class ProdutoService : ServiceBase<Produto>, IProdutoService
    {
        protected override void UpdateEntity(Produto existing, Produto updated)
        {
            // Validações básicas
            if (string.IsNullOrWhiteSpace(updated.Nome))
                throw new ValidationException("Nome do produto é obrigatório");

            if (updated.Valor <= 0)
                throw new ValidationException("Valor do produto deve ser positivo");

            if (updated.EstoqueMinimo < 0)
                throw new ValidationException("Estoque mínimo não pode ser negativo");

            if (updated.EstoqueMaximo < updated.EstoqueMinimo)
                throw new ValidationException("Estoque máximo não pode ser menor que o mínimo");

            // Atualizações
            existing.Nome = updated.Nome.Trim();
            existing.Valor = updated.Valor;
            existing.EstoqueMinimo = updated.EstoqueMinimo;
            existing.EstoqueMaximo = updated.EstoqueMaximo;
            existing.SaldoEmEstoque = updated.SaldoEmEstoque;
            existing.Fornecedor = updated.Fornecedor?.Trim();
            existing.PossuiGarantia = updated.PossuiGarantia;

            // Registra data/hora da atualização (se o modelo tiver essa propriedade)
            // existing.DataAtualizacao = DateTime.UtcNow;
        }
    }
}
