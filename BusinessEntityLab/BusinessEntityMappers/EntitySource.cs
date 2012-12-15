using BusinessEntityMappers.Emit;

namespace BusinessEntityMappers
{
    public class EntitySource<T> : IEntitySource<T>
        where T : class
    {
        private readonly IEntityLoader<T> loader;

        public EntitySource(IEntityLoader<T> loader)
        {
            this.loader = loader;
        }

        public T Create()
        {
            T context = new RuntimeStateFactory().CreateContext(loader);

            ((IStateContext)context).Connect();
            ((IStoreEntity)context).Id = loader.Id;

            return context;
        }
    }
}