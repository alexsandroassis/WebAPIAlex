using WebApiAlex.Models;
using WebApiAlex.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using WebApiAlex.Exceptions;

namespace WebApiAlex.Services
{
    public class VendaService : ServiceBase<Venda>, IVendaService
    {

        private readonly IProdutoService _produtoService;
        private readonly IGarantiaService _garantiaService; 

        // Injeta o serviço de produtos via construtor
        public VendaService(IProdutoService produtoService, IGarantiaService garantiaService)
        {
            _produtoService = produtoService;   
            _garantiaService = garantiaService;
        }

        public Venda Add(Venda venda)
        {
            // Validação básica herdada da classe base
            if (venda == null)
            {
                throw new ValidationException("Venda não pode ser nula.");
            }

            // Validação específica para itens da venda
            if (venda.ItensdaVenda == null || !venda.ItensdaVenda.Any())
            {
                throw new ValidationException("A venda deve conter pelo menos um item.");
            }


            // Valida cada item da venda
            foreach (var item in venda.ItensdaVenda)
            {
                if (item.Produto.Id == Guid.Empty)
                {
                    throw new ValidationException("Produto Id é obrigatório para todos os itens.");
                }

                // Verifica se o produto existe usando o serviço
                var produtoExistente = _produtoService.GetById(item.Produto.Id);
                if (produtoExistente == null)
                {
                    throw new ValidationException($"Produto com ID {item.Produto.Id} não encontrado.");
                }

                // Verifica se o produto existe usando o serviço
                var garantiaExistente = _garantiaService.GetById(item.Garantia.Id);
                if (garantiaExistente == null)
                {
                    throw new ValidationException($"Garantia com ID {item.Garantia.Id} não encontrado.");
                }

                // Atualiza a referência completa do produto
                item.Produto = produtoExistente;

                if (item.Quantidade <= 0)
                {
                    throw new ValidationException($"Quantidade inválida ({item.Quantidade}) para o produto {item.Produto.Id}.");
                }

                if (item.ValorUnitario <= 0)
                {
                    throw new ValidationException($"Valor unitário inválido ({item.ValorUnitario}) para o produto {item.Produto.Id}.");
                }
            }

            // Gera ID se não existir (herdado da classe base)
            if (venda.Id == Guid.Empty)
            {
                venda.Id = Guid.NewGuid();
            }
            else if (GetById(venda.Id) != null)
            {
                throw new ConflictException($"Já existe uma venda com o ID '{venda.Id}'.");
            }

            // Calcula o valor total da venda
            venda.ValorTotal = venda.ItensdaVenda.Sum(item => item.ValorTotal);

            // Adiciona a venda (chamada ao método base)
            base.Add(venda);

            return venda;
        }

        protected override void UpdateEntity(Venda existing, Venda updated)
        {
            throw new NotImplementedException();
        }
    }
}
