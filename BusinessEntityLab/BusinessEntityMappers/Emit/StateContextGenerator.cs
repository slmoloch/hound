using System.Reflection;
using BLToolkit.Reflection.Emit;

namespace BusinessEntityMappers.Emit
{
	internal class StateContextGenerator
	{
		public static BusinessEntityTypes GenerateContextTypes<T>(AssemblyBuilderHelper assemblyBuilder)
		{
			BusinessEntityTypes types = new BusinessEntityTypes();

			types.causesLoadDtoType = new CausesLoadDtoBuilder(assemblyBuilder).Create(typeof(T));
			types.causesLoadInterfaceType = new CausesLoadInterfaceBuilder(assemblyBuilder).Create(typeof(T));

			FieldInfo identityField = new IdentityPropertyInvestigator().GetIdentityProperty(typeof(T), types.causesLoadDtoType);

			types.createdStateType = new CreatedStateBuilder(assemblyBuilder, types.causesLoadInterfaceType, types.causesLoadDtoType, identityField).Create();
			types.loadedStateType = new LoadedStateBuilder(assemblyBuilder, types.causesLoadInterfaceType, types.causesLoadDtoType, identityField).Create();
			types.connectedStateType = new ConnectedStateBuilder(assemblyBuilder, types.causesLoadInterfaceType, types.causesLoadDtoType, identityField).Create(); 

			types.stateContextType = new StateContextBuilder<T>(assemblyBuilder, types.causesLoadInterfaceType).Create();

			return types;
		}
	}
}