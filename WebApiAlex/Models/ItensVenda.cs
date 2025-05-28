using System.ComponentModel.DataAnnotations;
using WebApiAlex.Models.Interfaces;

namespace WebApiAlex.Models
{
    /// <summary>
    /// Representa um item de venda associado a um produto e sua garantia
    /// </summary>
    public class ItensVenda 
    {

        /// <summary>
        /// Identificador único (UUID) do Produto.
        /// </summary>
        //[Required]
        //public Guid ProdutoId { get; set; }

        /// <summary>
        /// Referência ao produto (opcional - para navegação)
        /// </summary>
        public virtual Produto? Produto { get; set; }

        /// <summary>
        /// Quantidade de Produtos do Item.
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser pelo menos 1")]
        public int Quantidade { get; set; }

        /// <summary>
        /// Valor unitário do Item.
        /// </summary>
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor unitário deve ser positivo")]
        public decimal ValorUnitario { get; set; }

        /// <summary>
        /// Valor Total dos Itens x Quantidade (calculado).
        /// </summary>
        public decimal ValorTotal => Quantidade * ValorUnitario;

        /// <summary>
        /// Garantia do Item (opcional - para navegação)
        /// </summary>
        public virtual Garantia? Garantia { get; set; }

    }
}