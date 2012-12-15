using BusinessEntityMappers;
using BusinessEntityMappers.Emit;

namespace BusinessEntityMappers
{
	public class BusinessEntityMapper<T> : IMapper<T>
					where T : class
	{
		protected readonly ILoaderFactory<T> loaderFactory;

		public BusinessEntityMapper(ILoaderFactory<T> loaderFactory)
		{
			this.loaderFactory = loaderFactory;
		}

		/// <summary>
		/// Creates new instance of entity.
		/// </summary>
		/// <returns></returns>
		public T Create()
		{
			return new RuntimeStateFactory().CreateContext(loaderFactory.GetEntityLoader(0));
		}

		/// <summary>
		/// Loads the entity by id.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		public T Load(int id)
		{
			return CreateAndConnectEntity(loaderFactory.GetEntityLoader(id));
		}

		private static T CreateAndConnectEntity(IEntityLoader<T> entityLoader)
		{
			if (entityLoader.IsExists())
			{
				T entity = new EntitySource<T>(entityLoader).Create();
				entityLoader.Init(entity);
				return entity;
			}

			return null;
		}
	}
}