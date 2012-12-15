using System;
using System.Reflection;
using System.Reflection.Emit;
using BLToolkit.Reflection.Emit;

namespace BusinessEntityMappers.Emit
{
    internal class CreatedStateBuilder : StateBuilderBase
    {
			public CreatedStateBuilder(AssemblyBuilderHelper assemblyBuilder, Type causesLoadInterfaceType, Type casesLoadDtoType, FieldInfo identityField)
            : base(assemblyBuilder, causesLoadInterfaceType, casesLoadDtoType, identityField)
        {}

        protected override void GenerateStoreEntityStatus(TypeBuilder typeBuilder, PropertyInfo info)
        {
            CreateGetProperty(typeBuilder, info, StoreStatus.Created);
        }

        protected override void GenerateCausesLoadInterfaceMember(TypeBuilder typeBuilder, PropertyInfo info, FieldBuilder causesLoadObjectField)
        {
            CreateCausesLoadDtoAccessor(typeBuilder, info, causesLoadObjectField);
        }

        protected override string GetGeneratedTypeName()
        {
            return causesLoadInterfaceType.Name + "Created";
        }
    }
}