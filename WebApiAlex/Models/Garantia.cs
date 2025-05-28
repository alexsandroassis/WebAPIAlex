using System;
using WebApiAlex.Models.Interfaces;

namespace WebApiAlex.Models
{
    /// <summary>
    /// Representa um tipo ou plano de garantia que pode ser oferecido.
    /// Esta entidade define as características de uma garantia, como nome, valor e duração.
    /// </summary>
    public class Garantia : IEntity
    {
        /// <summary>
        /// Identificador único global (UUID) do plano de garantia.
        /// Este ID é usado para identificar o tipo de garantia de forma exclusiva.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome descritivo do plano de garantia (ex: "Garantia Estendida", "Garantia Padrão 1 Ano").
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Valor associado a este plano de garantia, se aplicável (ex: custo adicional).
        /// </summary>
        public decimal Valor { get; set; }

        /// <summary>
        /// Duração do plano de garantia em anos.
        /// </summary>
        public int Prazo { get; set; }
    }
}