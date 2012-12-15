using System;
using System.Reflection;

namespace BusinessEntityMappers.Emit
{
	internal class StateContextActivator
	{
		public static T Activate<T>(BusinessEntityTypes types, IEntityLoader<T> loadStrategy)
		{
			object casesLoadDto = ActivateDto(types.causesLoadDtoType);

			object createdState = ActivateState(types.createdStateType, casesLoadDto);
			object loadedState = ActivateState(types.loadedStateType, casesLoadDto);
			object connectedState = ActivateState(types.connectedStateType, casesLoadDto);

			return ActivateStateContext(
				types.causesLoadInterfaceType,
				loadStrategy,
				createdState,
				loadedState,
				connectedState,
				types.stateContextType);
		}

		private static T ActivateStateContext<T>(Type causesLoadingInterface, IEntityLoader<T> loadStrategy, object createdState, object loadedState, object connectedState, Type stateContextType)
		{
			Type[] constructorInput = new Type[] { typeof(IEntityLoader<T>), causesLoadingInterface, causesLoadingInterface, causesLoadingInterface };
			ConstructorInfo ctor = stateContextType.GetConstructor(constructorInput);

			return (T)ctor.Invoke(new object[] { loadStrategy, createdState, loadedState, connectedState });
		}

		private static object ActivateState(Type stateType, object casesLoadDto)
		{
			Type[] types = new Type[] { casesLoadDto.GetType() };

			return stateType.GetConstructor(types).Invoke(new object[] { casesLoadDto });
		}

		private static object ActivateDto(Type dtoType)
		{
			return dtoType.GetConstructor(Type.EmptyTypes).Invoke(Type.EmptyTypes);
		}
	}
}