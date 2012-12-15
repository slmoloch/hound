using System;
using System.Reflection;
using System.Reflection.Emit;
using BLToolkit.Reflection.Emit;

namespace BusinessEntityMappers.Emit
{
	internal class ConnectedStateBuilder : StateBuilderBase
	{
		public ConnectedStateBuilder(AssemblyBuilderHelper assemblyBuilder, Type causesLoadDtoType, Type casesLoadDtoType, FieldInfo identityField)
			: base(assemblyBuilder, causesLoadDtoType, casesLoadDtoType, identityField)
		{ }

		protected override void GenerateStoreEntityStatus(TypeBuilder typeBuilder, PropertyInfo info)
		{
			CreateGetProperty(typeBuilder, info, StoreStatus.Connected);
		}
		

		protected override void GenerateCausesLoadInterfaceMember(TypeBuilder typeBuilder, PropertyInfo info, FieldBuilder causesLoadObjectField)
		{
			CreateInvalidOperationExceptionProperty(typeBuilder, info);
		}

		protected override string GetGeneratedTypeName()
		{
			return causesLoadInterfaceType.Name + "Connected";
		}
	}
}