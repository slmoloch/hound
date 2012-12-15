using System;

namespace BusinessEntityMappers
{
	public class BusinessEntitySaveMapper<T>: ISaveMapper<T>
		where T : class
	{
		protected readonly ISaver<T> saver;

		public BusinessEntitySaveMapper(ISaver<T> saver)
		{
			this.saver = saver;
		}

		/// <summary>
		/// Stores the specified entity.
		/// </summary>
		/// <param name="entity">The po.</param>
		public void Store(T entity)
		{
			if (saver == null)
				throw new InvalidOperationException(string.Format("Saver is not assigned in mapper '{0}'", GetType()));

			IStoreEntity storeEntity = (IStoreEntity)entity;

			switch (storeEntity.StoreStatus)
			{
				case StoreStatus.Connected:
					saver.SaveNestedEntities(entity);
					break;

				case StoreStatus.Loaded:
					saver.Update(entity);
					break;

				case StoreStatus.Created:
					saver.Insert(entity);
					((IStateContext)entity).Store();
					break;

				default:
					throw new InvalidOperationException();
			}
		}

		/// <summary>
		/// Deletes the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		public void Delete(T entity)
		{
			saver.Delete(entity);
		}
	}
}
