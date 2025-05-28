using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using WebApiAlex.Exceptions;
using WebApiAlex.Models;
using WebApiAlex.Models.Interfaces;
using WebApiAlex.Services.Interfaces;

namespace WebApiAlex.Services
{
    public abstract class ServiceBase<T> : IServiceBase<T> where T : class, IEntity
    {
        protected readonly List<T> _items = new();

        /// <summary>
        /// Obtém todos os itens.
        /// </summary>
        /// <returns>Uma lista de todos os itens.</returns>
        public List<T> GetAll() => _items.ToList();

        /// <summary>
        /// Obtém um item pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do item.</param>
        /// <returns>O item encontrado ou null se não existir.</returns>
        public T GetById(Guid id) => _items.FirstOrDefault(x => x.Id == id);

        /// <summary>
        /// Adiciona um novo item.
        /// </summary>
        /// <param name="entity">A entidade a ser adicionada.</param>
        /// <returns>A entidade adicionada, com seu ID gerado.</returns>
        /// <exception cref="ValidationException">Lançada se a entidade for nula ou inválida.</exception>
        /// <exception cref="ConflictException">Lançada se houver um conflito de regra de negócio (ex: nome duplicado).</exception>
        public T Add(T entity)
        {
            if (entity == null)
            {
                throw new ValidationException($"{typeof(T).Name} não pode ser nulo.");
            }

            if (entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
            }
            else // Se o ID for fornecido, checar por duplicidade de ID ou lançar erro
            {
                if (GetById(entity.Id) != null)
                {
                    throw new ConflictException($"Já existe um(a) '{typeof(T).Name}' com o ID '{entity.Id}'.");
                }
            }

            _items.Add(entity);
            return entity; 
        }

        /// <summary>
        /// Atualiza um item existente.
        /// </summary>
        /// <param name="entity">A entidade com os dados atualizados.</param>
        /// <returns>A entidade atualizada.</returns>
        /// <exception cref="NotFoundException">Lançada se o item não for encontrado.</exception>
        /// <exception cref="ValidationException">Lançada se a entidade for nula ou inválida.</exception>
        public T Update(T entity)
        {
            if (entity == null)
            {
                throw new ValidationException($"{typeof(T).Name} não pode ser nulo para atualização.");
            }

            var existing = GetById(entity.Id);
            if (existing == null)
            {
                throw new NotFoundException(entity.Id, typeof(T).Name);
            }

            // Método abstrato para atualização de propriedades
            UpdateEntity(existing, entity); 
            return existing; 
        }

        /// <summary>
        /// Exclui um item pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do item a ser excluído.</param>
        /// <exception cref="NotFoundException">Lançada se o item não for encontrado.</exception>
        public void Delete(Guid id)
        {
            var item = GetById(id);
            if (item == null)
            {
                throw new NotFoundException(id, typeof(T).Name);
            }
            _items.Remove(item);
        }

        /// <summary>
        /// Método abstrato para sobrescrever a atualização de propriedades de um item existente.
        /// Assegura que apenas as propriedades permitidas sejam atualizadas.
        /// </summary>
        /// <param name="existing">A instância existente do item.</param>
        /// <param name="updated">A instância com os dados a serem atualizados.</param>
        protected abstract void UpdateEntity(T existing, T updated);

    }
}
