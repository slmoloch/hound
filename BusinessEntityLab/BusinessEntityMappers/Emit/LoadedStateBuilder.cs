using System;
using System.Reflection;
using System.Reflection.Emit;
using BLToolkit.Reflection.Emit;

namespace BusinessEntityMappers.Emit
{
    internal class LoadedStateBuilder : StateBuilderBase
    {
			public LoadedStateBuilder(AssemblyBuilderHelper assemblyBuilder, Type causesLoadDtoType, Type casesLoadDtoType, FieldInfo identityField)
            : base(assemblyBuilder, causesLoadDtoType, casesLoadDtoType, identityField)
        { }

        protected override void GenerateStoreEntityStatus(TypeBuilder typeBuilder, PropertyInfo info)
        {
            CreateGetProperty(typeBuilder, info, StoreStatus.Loaded);
        }

        protected override void GenerateCausesLoadInterfaceMember(TypeBuilder typeBuilder, PropertyInfo info, FieldBuilder causesLoadObjectField)
        {
            CreateCausesLoadDtoAccessor(typeBuilder, info, causesLoadObjectField);
        }

        protected override string GetGeneratedTypeName()
        {
            return causesLoadInterfaceType.Name + "Loaded";
        }
    }
}