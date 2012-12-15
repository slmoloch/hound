using System;
using System.Collections;
using System.Collections.Generic;
using BusinessEntityMappers;

namespace BusinessEntityMappers
{
	internal class BusinessEntityCollection<T> : IBusinessEntityCollection<T>, IMappedCollection<T>
			where T : class
	{
		private IList<T> loadedEntities = new List<T>();
		private readonly IList<T> innerList = new List<T>();
		private bool isLoaded;

		private readonly IBusinessCollectionItemsSource<T> loader;

		internal BusinessEntityCollection(IBusinessCollectionItemsSource<T> loader)
		{
			this.loader = loader;
		}

		private void LoadIfNotLoaded()
		{
			if (!isLoaded)
			{
				loadedEntities = loader.LoadAllEntities();

				foreach (T entity in loadedEntities)
				{
					innerList.Add(entity);
				}

				isLoaded = true;
			}
		}

		public IEnumerator GetEnumerator()
		{
			return ((IEnumerable<T>)this).GetEnumerator();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			LoadIfNotLoaded();
			return innerList.GetEnumerator();
		}

		public IList<T> SelectWithPaging(int startRow, int pageSize, string sortExpression)
		{
			LoadIfNotLoaded();

			if (ToDelete.Count != 0 || ToInsert.Count != 0)
			{
				throw new InvalidOperationException("BusinessEntityCollection can't perform paging if there are unsaved changes in it.");
			}

			IList<int> ids = loader.SelectPageIdentificators(startRow, pageSize, sortExpression);

			IList<T> pagedEntities = new List<T>();
			foreach (int id in ids)
			{
				foreach (T entity in innerList)
				{
					if (((IStoreEntity)entity).Id == id)
					{
						pagedEntities.Add(entity);
					}
				}
			}

			return pagedEntities;
		}

		/// <summary>
		/// Selects the item by id.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		public T SelectById(int id)
		{
			LoadIfNotLoaded();

			foreach (T item in innerList)
			{
				if (id == ((IStoreEntity)item).Id)
				{
					return item;
				}
			}

			return default(T);
		}

		public void Add(T item)
		{
			LoadIfNotLoaded();

			innerList.Add(item);
		}

		public void Remove(T item)
		{
			LoadIfNotLoaded();

			innerList.Remove(item);
		}

		public int Count
		{
			get
			{
				LoadIfNotLoaded();
				return innerList.Count;
			}
		}

		public IList<T> ToUpdate
		{
			get
			{
				List<T> items = new List<T>();

				foreach (T item in innerList)
				{
					if (ListContains(loadedEntities, ((IStoreEntity)item).Id))
					{
						items.Add(item);
					}
				}

				return items;
			}
		}

		public IList<T> ToDelete
		{
			get
			{
				List<T> items = new List<T>();

				foreach (T item in loadedEntities)
				{
					if (!ListContains(innerList, ((IStoreEntity)item).Id))
					{
						items.Add(item);
					}
				}

				return items;
			}
		}

		public IList<T> ToInsert
		{
			get
			{
				List<T> items = new List<T>();

				foreach (T item in innerList)
				{
					if (!ListContains(loadedEntities, ((IStoreEntity)item).Id))
					{
						items.Add(item);
					}
				}

				return items;
			}
		}

		private static bool ListContains(IEnumerable<T> list, int id)
		{
			foreach (T entity in list)
			{
				if (((IStoreEntity)entity).Id == id)
				{
					return true;
				}
			}

			return false;
		}

		public void Store(StoreHandler<T> removeHandler, StoreHandler<T> insertHandler, StoreHandler<T> updateHandler)
		{
			foreach (T line in ToDelete)
			{
				removeHandler(line);
			}

			if (updateHandler != null)
			{
				foreach (T line in ToUpdate)
				{
					updateHandler(line);
				}
			}

			if (insertHandler != null)
			{
				foreach (T line in ToInsert)
				{
					insertHandler(line);
				}
			}

			loadedEntities.Clear();

			foreach (T entity in innerList)
			{
				loadedEntities.Add(entity);
			}
		}
	}
}