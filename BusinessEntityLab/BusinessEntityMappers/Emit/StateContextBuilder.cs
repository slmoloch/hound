using System;
using System.Reflection;
using System.Reflection.Emit;
using BLToolkit.Reflection.Emit;

namespace BusinessEntityMappers.Emit
{
    internal class StateContextBuilder<T>
    {
        private readonly Type causesLoadingInterface;
        private TypeBuilder typeBuilder;

        private FieldBuilder loaderField;
        private FieldBuilder currentStateField;
        private FieldBuilder loadedStateField;
        private FieldBuilder connectedStateField;

        public StateContextBuilder(AssemblyBuilderHelper assemblyBuilder, Type causesLoadingInterface)
        {
            this.causesLoadingInterface = causesLoadingInterface;
            DefineContextType(assemblyBuilder);
        }

        public Type Create()
        {
            BuildPrivateFields();
            BuildConstructor();

            MethodBuilder toLoadedStateMethod = BuildToLoadedStateMethod();
            MethodBuilder loadMethod = BuildLoadMethod(toLoadedStateMethod);
            MethodBuilder loadIfConnectedMethod = BuildLoadIfConnectedMethod(loadMethod);

            GenerateConnectMethod();
            GenerateStoreMethod();


            foreach (PropertyInfo info in causesLoadingInterface.GetProperties())
            {
                PropertyInfo businessEntityPropertyInfo = typeof(T).GetProperty(info.Name);

                new InheritedProperty(
                        delegate(ILGenerator ilg)
                        {
                            new EmitHelper(ilg)
                                .ldarg_0
                                .call(loadIfConnectedMethod)
                                .ldarg_0
                                .ldfld(currentStateField)
                                .ldarg_1
                                .call(info.GetSetMethod())
                                .ret();
                        },
                        delegate(ILGenerator ilg)
                        {
                            new EmitHelper(ilg)
                                .ldarg_0
                                .call(loadIfConnectedMethod)
                                .ldarg_0
                                .ldfld(currentStateField)
                                .call(info.GetGetMethod())
                                .ret();
                        })
                        .GenerateIn(typeBuilder, businessEntityPropertyInfo);
            }

            foreach (PropertyInfo info in typeof(IStoreEntity).GetProperties())
            {
                //PropertyInfo businessEntityPropertyInfo = typeof(T).GetProperty(info.Name);

                switch (info.Name)
                {
                    case "Id":
                        new InheritedProperty(
                                delegate(ILGenerator ilg)
                                {
                                    ilg.Emit(OpCodes.Ldarg_0);
                                    ilg.Emit(OpCodes.Ldfld, currentStateField);
                                    ilg.Emit(OpCodes.Ldarg_1);
                                    ilg.Emit(OpCodes.Call, info.GetSetMethod());
                                    ilg.Emit(OpCodes.Ret);
                                },
                                delegate(ILGenerator ilg)
                                {
                                    ilg.Emit(OpCodes.Ldarg_0);
                                    ilg.Emit(OpCodes.Ldfld, currentStateField);
                                    ilg.Emit(OpCodes.Call, info.GetGetMethod());
                                    ilg.Emit(OpCodes.Ret);
                                })
                                .GenerateIn(typeBuilder, info);
                        break;
                    case "StoreStatus":
                        new InheritedProperty(
                                null,
                                delegate(ILGenerator ilg)
                                {
                                    ilg.Emit(OpCodes.Ldarg_0);
                                    ilg.Emit(OpCodes.Ldfld, currentStateField);
                                    ilg.Emit(OpCodes.Call, info.GetGetMethod());
                                    ilg.Emit(OpCodes.Ret);
                                })
                                .GenerateIn(typeBuilder, info);

                        break;
                }
            }

            return typeBuilder.CreateType();

        }

        private MethodBuilder BuildLoadMethod(MethodInfo toLoadedStateMethod)
        {
            MethodBuilder loadMethod = typeBuilder.DefineMethod("Load", MethodAttributes.Private, null, Type.EmptyTypes);

            new EmitHelper(loadMethod.GetILGenerator())
                .ldarg_0
                .call(toLoadedStateMethod)
                .ldarg_0
                .ldfld(loaderField)
                .ldarg_0
                .call(typeof(IEntityLoader<T>).GetMethod("Load"))
                .ret();

            return loadMethod;
        }

        private void BuildConstructor()
        {
            Type[] constructorInput = new Type[]
		        {
		            typeof (IEntityLoader<T>),
		            causesLoadingInterface,
		            causesLoadingInterface,
		            causesLoadingInterface
		        };

            ConstructorBuilder cons =
                typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, constructorInput);

            ConstructorInfo entityConstructor = typeof(T).GetConstructor(Type.EmptyTypes);

            EmitHelper helper = new EmitHelper(cons.GetILGenerator());

            helper
                .ldarg_0
                .ldarg_1
                .stfld(loaderField)
                .ldarg_0
                .ldarg_2
                .stfld(currentStateField)
                .ldarg_0
                .ldarg_3
                .stfld(loadedStateField)
                .ldarg_0
                .ldarg_s(4)
                .stfld(connectedStateField);

            if (entityConstructor != null)
            {
                helper
                    .ldarg_0
                    .call(entityConstructor);
            }

            helper
                .ret();
        }

        private MethodBuilder BuildToLoadedStateMethod()
        {
            MethodBuilder toLoadedStateMethod = typeBuilder.DefineMethod(
                "ToLoadedState",
                MethodAttributes.Private,
                null,
                new Type[] { });

            new EmitHelper(toLoadedStateMethod.GetILGenerator())
                .ldarg_0
                .ldarg_0
                .ldfld(loadedStateField)
                .stfld(currentStateField)
                .ret();

            return toLoadedStateMethod;
        }

        private MethodBuilder BuildLoadIfConnectedMethod(MethodInfo loadMethod)
        {
            MethodBuilder loadIfConnectedMethod = typeBuilder.DefineMethod("LoadIfConnected", MethodAttributes.Private, null, Type.EmptyTypes);

            EmitHelper helper = new EmitHelper(loadIfConnectedMethod.GetILGenerator());
            Label label = helper.DefineLabel();

            helper
                .ldarg_0
                .call(typeof(IStoreEntity).GetProperty("StoreStatus").GetGetMethod())
                .ldc_i4((int)StoreStatus.Connected)
                .ceq
                .brfalse_s(label)
                .ldarg_0
                .call(loadMethod)
                .MarkLabel(label)
                .ret();

            return loadIfConnectedMethod;
        }

        private void GenerateConnectMethod()
        {
            MethodInfo inheritedConnectMethod = typeof(IStateContext).GetMethod("Connect");

            MethodBuilder connectMethod = typeBuilder.DefineMethod(
                inheritedConnectMethod.Name,
                inheritedConnectMethod.Attributes ^ MethodAttributes.Abstract,
                inheritedConnectMethod.ReturnType,
                CausesLoadInterfaceBuilder.GetParameterTypes(inheritedConnectMethod.GetParameters()));

            new EmitHelper(connectMethod.GetILGenerator())
                    .ldarg_0
                    .ldarg_0
                    .ldfld(connectedStateField)
                    .stfld(currentStateField)
                    .ret();

            typeBuilder.DefineMethodOverride(connectMethod, inheritedConnectMethod);
        }

        private void GenerateStoreMethod()
        {
            MethodInfo inheritedStoreMethod = typeof(IStateContext).GetMethod("Store");

            MethodBuilder storeMethod =
                    typeBuilder.DefineMethod(inheritedStoreMethod.Name,
                                                                     inheritedStoreMethod.Attributes ^ MethodAttributes.Abstract,
                                                                     inheritedStoreMethod.ReturnType, CausesLoadInterfaceBuilder.GetParameterTypes(inheritedStoreMethod.GetParameters()));

            new EmitHelper(storeMethod.GetILGenerator())
                    .ldarg_0
                    .ldarg_0
                    .ldfld(loadedStateField)
                    .stfld(currentStateField)
                    .ret();

            typeBuilder.DefineMethodOverride(storeMethod, inheritedStoreMethod);
        }

        private void DefineContextType(AssemblyBuilderHelper assemblyBuilder)
        {
            typeBuilder = assemblyBuilder.DefineType(typeof(T).Name + "StateContext", TypeAttributes.NotPublic | TypeAttributes.Class, typeof(T), new Type[] { typeof(IStateContext), typeof(IStoreEntity) });
        }

        private void BuildPrivateFields()
        {
            loaderField = typeBuilder.DefineField("loader", typeof(IEntityLoader<T>), FieldAttributes.Private);
            currentStateField = typeBuilder.DefineField("currentState", causesLoadingInterface, FieldAttributes.Private);
            loadedStateField = typeBuilder.DefineField("loadedState", causesLoadingInterface, FieldAttributes.Private);
            connectedStateField = typeBuilder.DefineField("connectedState", causesLoadingInterface, FieldAttributes.Private);
        }
    }
}