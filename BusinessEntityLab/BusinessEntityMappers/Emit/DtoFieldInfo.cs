using System;
using System.Reflection;

namespace BusinessEntityMappers.Emit
{
	internal class DtoFieldInfo
	{
		private readonly PropertyInfo propertyInfo;

		public DtoFieldInfo(PropertyInfo propertyInfo)
		{
			this.propertyInfo = propertyInfo;
		}

		public Type GetFieldType()
		{
			return propertyInfo.PropertyType;
		}

		public string GetFieldName()
		{
			return propertyInfo.Name.ToLower();
		}

	}
}