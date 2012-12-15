namespace BusinessEntityMappers
{
	public static class BusinessEntityCollectionFactory
	{
		public static IBusinessEntityCollection<T> Create<T>(IBusinessCollectionItemsSource<T> businessItemsSource)
				where T : class
		{
			return new BusinessEntityCollection<T>(businessItemsSource);
		}

		public static IBusinessEntityCollection<T> CreateEmptyCollection<T>()
			where T : class
		{
			return Create(new EmptyLoadCollectionStrategy<T>());
		}
	}
}
