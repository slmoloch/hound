using System;
using System.Reflection;
using System.Reflection.Emit;
using BLToolkit.Reflection.Emit;

namespace BusinessEntityMappers.Emit
{
	internal abstract class StateBuilderBase
	{
		private readonly AssemblyBuilderHelper assemblyBuilder;
		protected readonly Type causesLoadInterfaceType;
		protected readonly Type casesLoadDtoType;
		private readonly FieldInfo identityField;

		protected StateBuilderBase(AssemblyBuilderHelper assemblyBuilder, Type causesLoadInterfaceType, Type casesLoadDtoType, FieldInfo identityField)
		{
			this.assemblyBuilder = assemblyBuilder;
			this.causesLoadInterfaceType = causesLoadInterfaceType;
			this.casesLoadDtoType = casesLoadDtoType;
			this.identityField = identityField;
		}

		public TypeBuilder GetStateTypeBuilder(string typeName)
		{
			return assemblyBuilder.DefineType(
					typeName,
					TypeAttributes.NotPublic | TypeAttributes.Class,
					typeof(object),
					new Type[] { causesLoadInterfaceType, typeof(IStoreEntity) });
		}

		protected static void CreateGetProperty(TypeBuilder typeBuilder, PropertyInfo info, StoreStatus storeStatus)
		{
			new InheritedProperty(
					null,
					delegate(ILGenerator ilg)
					{
						new EmitHelper(ilg)
							.ldc_i4((int)storeStatus)
							.ret();
					})
					.GenerateIn(typeBuilder, info);
		}

		protected static void CreateCausesLoadDtoAccessor(TypeBuilder typeBuilder, PropertyInfo info, FieldInfo causesLoadDtoField)
		{
			DtoFieldInfo fieldInfo = new DtoFieldInfo(info);

			Type dtoType = causesLoadDtoField.FieldType;
			FieldInfo dtoField = dtoType.GetField(fieldInfo.GetFieldName());

			GenerateDtoAccessorField(typeBuilder, info, causesLoadDtoField, dtoField);
		}

		private static void GenerateDtoAccessorField(TypeBuilder typeBuilder, PropertyInfo info, FieldInfo causesLoadDtoField, FieldInfo dtoField)
		{
			new InheritedProperty(
				delegate(ILGenerator ilg)
				{
					new EmitHelper(ilg)
						.ldarg_0
						.ldfld(causesLoadDtoField)
						.ldarg_1
						.stfld(dtoField)
						.ret();
				},
				delegate(ILGenerator ilg)
				{
					new EmitHelper(ilg)
						.ldarg_0
						.ldfld(causesLoadDtoField)
						.ldfld(dtoField)
						.ret();
				})
				.GenerateIn(typeBuilder, info);
		}

		protected static void CreateInvalidOperationExceptionProperty(TypeBuilder typeBuilder, PropertyInfo info)
		{
			new InheritedProperty(
					delegate(ILGenerator ilg)
					{
						GenerateThrowInvalidOperation(ilg);
					},
					delegate(ILGenerator ilg)
					{
						GenerateThrowInvalidOperation(ilg);
					})
					.GenerateIn(typeBuilder, info);
		}

		private static void GenerateThrowInvalidOperation(ILGenerator ilg)
		{
			ConstructorInfo ctor = typeof(InvalidOperationException).GetConstructor(Type.EmptyTypes);

			ilg.Emit(OpCodes.Newobj, ctor);
			ilg.Emit(OpCodes.Throw);
		}

		public Type Create()
		{
			TypeBuilder typeBuilder = GetStateTypeBuilder(GetGeneratedTypeName());

			ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { casesLoadDtoType });
			FieldBuilder causesLoadObjectField = typeBuilder.DefineField("casesLoadDto", casesLoadDtoType, FieldAttributes.Private);

			EmitHelper helper = new EmitHelper(constructorBuilder.GetILGenerator());

			helper
					.ldarg_0
					.ldarg_1
					.stfld(causesLoadObjectField)
					.ret();


			foreach (PropertyInfo info in causesLoadInterfaceType.GetProperties())
			{
				GenerateCausesLoadInterfaceMember(typeBuilder, info, causesLoadObjectField);
			}

			foreach (PropertyInfo info in typeof(IStoreEntity).GetProperties())
			{
				switch (info.Name)
				{
					case "Id":
						GenerateStoreEntityId(typeBuilder, info, causesLoadObjectField, identityField);
						break;

					case "StoreStatus":
						GenerateStoreEntityStatus(typeBuilder, info);
						break;
				}
			}

			return typeBuilder.CreateType();
		}

		protected virtual void GenerateStoreEntityStatus(TypeBuilder typeBuilder, PropertyInfo info)
		{
			throw new NotImplementedException();
		}

		private static void GenerateStoreEntityId(TypeBuilder typeBuilder, PropertyInfo info, FieldInfo causesLoadDtoField, FieldInfo identityField)
		{
			GenerateDtoAccessorField(typeBuilder, info, causesLoadDtoField, identityField);
		}

		protected virtual void GenerateCausesLoadInterfaceMember(TypeBuilder typeBuilder, PropertyInfo info, FieldBuilder causesLoadObjectField)
		{
			throw new NotImplementedException();
		}

		protected virtual string GetGeneratedTypeName()
		{
			throw new NotImplementedException();
		}
	}
}