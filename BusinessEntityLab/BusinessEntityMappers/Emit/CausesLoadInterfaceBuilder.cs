using System;
using System.Reflection;
using System.Reflection.Emit;
using BLToolkit.Reflection.Emit;

namespace BusinessEntityMappers.Emit
{
	public class CausesLoadInterfaceBuilder
	{
		private readonly AssemblyBuilderHelper assemblyBuilder;

		public CausesLoadInterfaceBuilder(AssemblyBuilderHelper assemblyBuilder)
		{
			this.assemblyBuilder = assemblyBuilder;
		}

		public Type Create(Type entityType)
		{
			TypeBuilderHelper typeBuilderHelper = GetTypeBuilderHelper(entityType);

			Converter<MethodInfo, MethodBuilder> defineInterfaceMethod = delegate(MethodInfo methodInfo)
																																			 {
																																				 return CreateMethod(typeBuilderHelper, methodInfo);
																																			 };

			foreach (PropertyInfo info in entityType.GetProperties())
			{
				if (BusinessEntityReflectionHelper.IsCausedLoadProperty(info))
				{
					PropertyBuilder propertyBuilder = GetPropertyBuilder(typeBuilderHelper, info);

					if (info.CanRead)
					{
						propertyBuilder.SetGetMethod(defineInterfaceMethod(info.GetGetMethod(true)));
					}

					if (info.CanWrite)
					{
						propertyBuilder.SetSetMethod(defineInterfaceMethod(info.GetSetMethod(true)));
					}
				}
			}

			return typeBuilderHelper.Create();
		}

		private static PropertyBuilder GetPropertyBuilder(TypeBuilderHelper typeBuilderHelper, PropertyInfo info)
		{
			return typeBuilderHelper.TypeBuilder.DefineProperty(
					info.Name,
					info.Attributes,
					info.PropertyType,
					GetParameterTypes(info.GetIndexParameters()));
		}

		private static MethodBuilder CreateMethod(TypeBuilderHelper typeBuilderHelper, MethodInfo methodInfo)
		{
			return typeBuilderHelper.TypeBuilder.DefineMethod(
					methodInfo.Name,
					MethodAttributes.Public |
					MethodAttributes.HideBySig |
					MethodAttributes.SpecialName |
					MethodAttributes.NewSlot |
					MethodAttributes.Abstract |
					MethodAttributes.Virtual,
					CallingConventions.HasThis,
					methodInfo.ReturnType,
					GetParameterTypes(methodInfo.GetParameters())
					);
		}

		private TypeBuilderHelper GetTypeBuilderHelper(Type businessEntityType)
		{
			return assemblyBuilder.DefineType("I" + businessEntityType.Name + "CausesLoad",
																				TypeAttributes.Public |
																				TypeAttributes.Interface |
																				TypeAttributes.Abstract,
																				null,
																				typeof(IStoreEntity));
		}
		
		public static Type[] GetParameterTypes(ParameterInfo[] paramInfos)
		{
			Type[] types = new Type[paramInfos.Length];

			for (int i = 0; i < paramInfos.Length; ++i)
			{
				types[i] = paramInfos[i].ParameterType;
			}
			return types;
		}
	}
}