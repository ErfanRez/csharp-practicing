namespace AspMongoDB.Common
{
    public interface IBaseService<TEntity> where TEntity : BaseEntity
    {
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(string id);
        TEntity GetById(string id);
        List<TEntity> GetAll();
    }
}
