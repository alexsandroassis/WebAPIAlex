using System;
using WebApiAlex.Models.Interfaces;

namespace WebApiAlex.Models
{

    /// <summary>
    /// Representa um produto genérico, incluindo informações de estoque e fornecedor.
    /// </summary>
    public class Produto : IEntity
    {
        /// <summary>
        /// Identificador único global (UUID) do produto.
        /// Este ID é usado para identificar o produto de forma exclusiva em todo o sistema.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome completo do produto.
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Valor unitário de venda do produto.
        /// </summary>
        public decimal Valor { get; set; }

        /// <summary>
        /// Quantidade mínima permitida em estoque para este produto.
        /// Alertas podem ser gerados quando o saldo em estoque atinge ou fica abaixo deste valor.
        /// </summary>
        public int EstoqueMinimo { get; set; }

        /// <summary>
        /// Quantidade máxima ideal de produtos que deve ser mantida em estoque.
        /// Este valor pode ser usado para otimização de pedidos ou armazenamento.
        /// </summary>
        public int EstoqueMaximo { get; set; }

        /// <summary>
        /// Quantidade atual de unidades do produto disponíveis em estoque.
        /// </summary>
        public int SaldoEmEstoque { get; set; }

        /// <summary>
        /// Nome ou identificador do fornecedor responsável por este produto.
        /// </summary>
        public string Fornecedor { get; set; }

        /// <summary>
        /// Booleano que indica se o produto em questão possui garantia de fábrica ou do vendedor.
        /// </summary>
        public bool PossuiGarantia { get; set; }
    }
}
