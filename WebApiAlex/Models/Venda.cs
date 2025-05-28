using System; 
using System.Collections.Generic;
using WebApiAlex.Models.Interfaces;

namespace WebApiAlex.Models
{
    /// <summary>
    /// Representa uma transação de venda completa, contendo um ou mais itens.
    /// </summary>
    public class Venda : IEntity
    {
        /// <summary>
        /// Identificador único (UUID) da venda.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Itens da venda
        /// </summary>
        public required List<ItensVenda> ItensdaVenda { get; set; }

        /// <summary>
        /// O valor total da venda, que é a soma dos Valores Totais de todos os ItemVenda.
        /// Pode ser um campo calculado ou persistido.
        /// </summary>
        public decimal ValorTotal { get; set; }

    }
}
