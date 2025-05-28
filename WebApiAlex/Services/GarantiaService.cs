using WebApiAlex.Models;
using WebApiAlex.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace WebApiAlex.Services
{
    public class GarantiaService : ServiceBase<Garantia>, IGarantiaService
    {
        protected override void UpdateEntity(Garantia existing, Garantia updated)
        {
            // Validações
            if (string.IsNullOrWhiteSpace(updated.Nome))
                throw new ValidationException("Nome da garantia é obrigatório");

            if (updated.Valor < 0)
                throw new ValidationException("Valor da garantia não pode ser negativo");

            if (updated.Prazo <= 0)
                throw new ValidationException("Prazo da garantia deve ser positivo");

            // Atualiza propriedades
            existing.Nome = updated.Nome.Trim();
            existing.Valor = updated.Valor;
            existing.Prazo = updated.Prazo;
        }
    }
}
