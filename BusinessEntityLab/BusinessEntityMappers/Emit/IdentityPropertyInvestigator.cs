using System;
using System.Reflection;

namespace BusinessEntityMappers.Emit
{
	internal class IdentityPropertyInvestigator
	{
		public FieldInfo GetIdentityProperty(Type entityType, Type dtoType)
		{
			PropertyInfo[] properties = entityType.GetProperties();

			foreach (PropertyInfo propertyInfo in properties)
			{
				if (BusinessEntityReflectionHelper.IsIdentityProperty(propertyInfo))
				{
					if (!BusinessEntityReflectionHelper.IsCausedLoadProperty(propertyInfo))
					{
						throw new InvalidOperationException(string.Format("Identity property in type '{0}' should have CauseseLoad attribute.", entityType));
					}

					return dtoType.GetField(new DtoFieldInfo(propertyInfo).GetFieldName());
				}
			}

			throw new InvalidOperationException(string.Format("Entity of type '{0}' should have Identity property defined.", entityType));
		}
	}
}