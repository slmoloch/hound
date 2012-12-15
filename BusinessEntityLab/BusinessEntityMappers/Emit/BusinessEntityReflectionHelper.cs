using System.Reflection;

namespace BusinessEntityMappers.Emit
{
	internal class BusinessEntityReflectionHelper
	{
		public static bool IsCausedLoadProperty(ICustomAttributeProvider info)
		{
			object[] causesLoadAttributes = info.GetCustomAttributes(typeof(CausesLoadAttribute), true);
			return causesLoadAttributes.Length > 0;
		}

		public static bool IsIdentityProperty(PropertyInfo info)
		{
			object[] causesLoadAttributes = info.GetCustomAttributes(typeof(IdentityPropertyAttribute), true);
			return causesLoadAttributes.Length > 0;
		}
	}
}