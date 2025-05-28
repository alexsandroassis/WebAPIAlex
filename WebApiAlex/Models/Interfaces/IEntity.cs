namespace WebApiAlex.Models.Interfaces
{
    public interface IEntity
    {
        /// <summary>
        /// Identificador único (UUID) da entidade.
        /// </summary>
        public Guid Id { get; set; }
    }
}
