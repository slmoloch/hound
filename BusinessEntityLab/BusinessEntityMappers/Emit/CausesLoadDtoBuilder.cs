using System;
using System.Reflection;
using BLToolkit.Reflection.Emit;

namespace BusinessEntityMappers.Emit
{
	internal class CausesLoadDtoBuilder
	{
		private readonly AssemblyBuilderHelper assemblyBuilder;

		public CausesLoadDtoBuilder(AssemblyBuilderHelper assemblyBuilder)
		{
			this.assemblyBuilder = assemblyBuilder;
		}

		public Type Create(Type entityType)
		{
			TypeBuilderHelper typeBuilder = GetDtoTypeBuilder(GetTypeName(entityType));
			PropertyInfo[] properties = entityType.GetProperties();

			foreach (PropertyInfo propertyInfo in properties)
			{
				if (BusinessEntityReflectionHelper.IsCausedLoadProperty(propertyInfo))
				{
					DtoFieldInfo fieldInfo = new DtoFieldInfo(propertyInfo);

					DefineField(
						typeBuilder,
						fieldInfo.GetFieldName(),
						fieldInfo.GetFieldType());
				}
			}

			return typeBuilder.Create();
		}
		private static void DefineField(TypeBuilderHelper typeBuilderHelper, string fieldName, Type fieldType)
		{
			typeBuilderHelper.DefineField(fieldName, fieldType, FieldAttributes.Public);
		}

		private TypeBuilderHelper GetDtoTypeBuilder(string typeName)
		{
			return assemblyBuilder.DefineType(typeName, TypeAttributes.NotPublic | TypeAttributes.Class, typeof(object), null);
		}

		private static string GetTypeName(Type businessEntityType)
		{
			return businessEntityType.Name + "CausesLoadDto";
		}
	}
}