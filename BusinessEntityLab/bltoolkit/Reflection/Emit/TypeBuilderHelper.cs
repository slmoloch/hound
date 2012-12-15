using System;
using System.Reflection;
using System.Reflection.Emit;

namespace BLToolkit.Reflection.Emit
{
	/// <summary>
	/// A wrapper around the <see cref="TypeBuilder"/> class.
	/// </summary>
	/// <include file="Examples.CS.xml" path='examples/emit[@name="Emit"]/*' />
	/// <include file="Examples.VB.xml" path='examples/emit[@name="Emit"]/*' />
	/// <seealso cref="System.Reflection.Emit.TypeBuilder">TypeBuilder Class</seealso>
	public class TypeBuilderHelper
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TypeBuilderHelper"/> class
		/// with the specified parameters.
		/// </summary>
		/// <param name="assemblyBuilder">Associated <see cref="AssemblyBuilderHelper"/>.</param>
		/// <param name="typeBuilder">A <see cref="TypeBuilder"/></param>
		public TypeBuilderHelper(AssemblyBuilderHelper assemblyBuilder, System.Reflection.Emit.TypeBuilder typeBuilder)
		{
			if (assemblyBuilder == null) throw new ArgumentNullException("assemblyBuilder");
			if (typeBuilder     == null) throw new ArgumentNullException("typeBuilder");

			_assembly    = assemblyBuilder;
			_typeBuilder = typeBuilder;

			_typeBuilder.SetCustomAttribute(_assembly.BLToolkitAttribute);
		}

		private readonly AssemblyBuilderHelper _assembly;
		/// <summary>
		/// Gets associated AssemblyBuilderHelper.
		/// </summary>
		public  AssemblyBuilderHelper  Assembly
		{
			get { return _assembly; }
		}

		private readonly System.Reflection.Emit.TypeBuilder _typeBuilder;
		/// <summary>
		/// Gets TypeBuilder.
		/// </summary>
		public  System.Reflection.Emit.TypeBuilder  TypeBuilder
		{
			get { return _typeBuilder; }
		}

		/// <summary>
		/// Converts the supplied <see cref="TypeBuilderHelper"/> to a <see cref="TypeBuilder"/>.
		/// </summary>
		/// <param name="typeBuilder">The TypeBuilderHelper.</param>
		/// <returns>A TypeBuilder.</returns>
		public static implicit operator System.Reflection.Emit.TypeBuilder(TypeBuilderHelper typeBuilder)
		{
			if (typeBuilder == null) throw new ArgumentNullException("typeBuilder");

			return typeBuilder.TypeBuilder;
		}

		#region DefineMethod Overrides

		/// <summary>
		/// Adds a new method to the class, with the given name and method signature.
		/// </summary>
		/// <param name="name">The name of the method. name cannot contain embedded nulls. </param>
		/// <param name="attributes">The attributes of the method. </param>
		/// <param name="returnType">The return type of the method.</param>
		/// <param name="parameterTypes">The types of the parameters of the method.</param>
		/// <returns>The defined method.</returns>
		public MethodBuilderHelper DefineMethod(
			string name, MethodAttributes attributes, Type returnType, params Type[] parameterTypes)
		{
			return new MethodBuilderHelper(this, _typeBuilder.DefineMethod(name, attributes, returnType, parameterTypes));
		}

		public MethodBuilderHelper DefineMethod(
			string             name,
			MethodAttributes   attributes,
			CallingConventions callingConvention,
			Type               returnType,
			Type[]             parameterTypes)
		{
			return new MethodBuilderHelper(this, _typeBuilder.DefineMethod(
				name, attributes, callingConvention, returnType, parameterTypes));
		}

		/// <summary>
		/// Adds a new method to the class, with the given name and method signature.
		/// </summary>
		/// <param name="name">The name of the method. name cannot contain embedded nulls. </param>
		/// <param name="attributes">The attributes of the method. </param>
		/// <param name="returnType">The return type of the method.</param>
		/// <returns>The defined method.</returns>
		public MethodBuilderHelper DefineMethod(string name, MethodAttributes attributes, Type returnType)
		{
			return new MethodBuilderHelper(this, _typeBuilder.DefineMethod(name, attributes, returnType, Type.EmptyTypes));
		}

		/// <summary>
		/// Adds a new method to the class, with the given name and method signature.
		/// </summary>
		/// <param name="name">The name of the method. name cannot contain embedded nulls. </param>
		/// <param name="attributes">The attributes of the method. </param>
		/// <returns>The defined method.</returns>
		public MethodBuilderHelper DefineMethod(string name, MethodAttributes attributes)
		{
			return new MethodBuilderHelper(this, _typeBuilder.DefineMethod(name, attributes, typeof(void), Type.EmptyTypes));
		}

#if FW2
		/// <summary>
		/// Adds a new method to the class, with the given name and method signature.
		/// </summary>
		/// <param name="name">The name of the method. name cannot contain embedded nulls. </param>
		/// <param name="attributes">The attributes of the method. </param>
		/// <returns>The defined method.</returns>
		/// <param name="callingConvention">The calling convention of the method.</param>
		public MethodBuilderHelper DefineMethod(
			string             name,
			MethodAttributes   attributes,
			CallingConventions callingConvention)
		{
			return new MethodBuilderHelper(this, _typeBuilder.DefineMethod(name, attributes, callingConvention));
		}
#endif

		/// <summary>
		/// Adds a new method to the class, with the given name and method signature.
		/// </summary>
		/// <param name="name">The name of the method. name cannot contain embedded nulls. </param>
		/// <param name="methodInfoDeclaration">The method whose declaration is to be used.</param>
		/// <param name="attributes">The attributes of the method. </param>
		/// <returns>The defined method.</returns>
		public MethodBuilderHelper DefineMethod(
			string           name,
			MethodInfo       methodInfoDeclaration,
			MethodAttributes attributes)
		{
			if (methodInfoDeclaration == null) throw new ArgumentNullException("methodInfoDeclaration");

			MethodBuilderHelper method;
			ParameterInfo[]     pi         = methodInfoDeclaration.GetParameters();
			Type[]              parameters = new Type[pi.Length];

#if FW2
			// When a method contains a generic parameter we need to replace all
			// generic types from methodInfoDeclaration with local ones.
			//
			if (methodInfoDeclaration.ContainsGenericParameters)
			{
				method = new MethodBuilderHelper(this, _typeBuilder.DefineMethod(name, attributes, methodInfoDeclaration.CallingConvention), false);

				Type[]                        genArgs   = methodInfoDeclaration.GetGenericArguments();
				GenericTypeParameterBuilder[] genParams = method.MethodBuilder.DefineGenericParameters(
					Array.ConvertAll<Type, string>(genArgs, delegate (Type t) { return t.Name; }));

				// Copy parameter constraints.
				//
				System.Collections.Generic.List<Type> interfaceConstraints = null;
				for (int i = 0; i < genParams.Length; i++)
				{
					genParams[i].SetGenericParameterAttributes(genArgs[i].GenericParameterAttributes);

					foreach (Type constraint in genArgs[i].GetGenericParameterConstraints())
					{
						if (constraint.IsClass)
							genParams[i].SetBaseTypeConstraint(constraint);
						else
						{
							if (interfaceConstraints == null)
								interfaceConstraints = new System.Collections.Generic.List<Type>();
							interfaceConstraints.Add(constraint);
						}
					}

					if (interfaceConstraints != null && interfaceConstraints.Count > 0)
					{
						genParams[i].SetInterfaceConstraints(interfaceConstraints.ToArray());
						interfaceConstraints.Clear();
					}
				}

				for (int i = 0; i < pi.Length; i++)
					parameters[i] = TypeHelper.TranslateGenericParameters(pi[i].ParameterType, genParams);

				method.MethodBuilder.SetParameters(parameters);
				method.MethodBuilder.SetReturnType(TypeHelper.TranslateGenericParameters(
					methodInfoDeclaration.ReturnType, genParams));

				// Now its safe to add a custom attribute.
				//
				method.MethodBuilder.SetCustomAttribute(method.Type.Assembly.BLToolkitAttribute);
			}
			else
#endif
			{
				for (int i = 0; i < pi.Length; i++)
					parameters[i] = pi[i].ParameterType;

				method = DefineMethod(
					name,
					attributes,
					methodInfoDeclaration.CallingConvention,
					methodInfoDeclaration.ReturnType,
					parameters);
			}

			// Compiler overrides methods only for interfaces. We do the same.
			// If we wanted to override virtual methods, then methods should've had
			// MethodAttributes.VtableLayoutMask attribute
			// and the following condition should've been used below:
			// if ((methodInfoDeclaration is FakeMethodInfo) == false)
			//
			if (methodInfoDeclaration.DeclaringType.IsInterface)
				_typeBuilder.DefineMethodOverride(method.MethodBuilder, methodInfoDeclaration);

			method.OverriddenMethod = methodInfoDeclaration;

			for (int i = 0; i < pi.Length; i++)
				method.MethodBuilder.DefineParameter(i + 1, pi[i].Attributes, pi[i].Name);

			return method;
		}

		/// <summary>
		/// Adds a new method to the class, with the given name and method signature.
		/// </summary>
		/// <param name="name">The name of the method. name cannot contain embedded nulls. </param>
		/// <param name="methodInfoDeclaration">The method whose declaration is to be used.</param>
		/// <returns>The defined method.</returns>
		public MethodBuilderHelper DefineMethod(string name, MethodInfo methodInfoDeclaration)
		{
			return DefineMethod(name, methodInfoDeclaration, MethodAttributes.Virtual);
		}

		/// <summary>
		/// Adds a new private method to the class.
		/// </summary>
		/// <param name="methodInfoDeclaration">The method whose declaration is to be used.</param>
		/// <returns>The defined method.</returns>
		public MethodBuilderHelper DefineMethod(MethodInfo methodInfoDeclaration)
		{
			if (methodInfoDeclaration == null) throw new ArgumentNullException("methodInfoDeclaration");

			bool isInterface = methodInfoDeclaration.DeclaringType.IsInterface;

			string name = isInterface?
				methodInfoDeclaration.DeclaringType.FullName + "." + methodInfoDeclaration.Name:
				methodInfoDeclaration.Name;

			MethodAttributes attrs = 
				MethodAttributes.Virtual |
				MethodAttributes.HideBySig |
				MethodAttributes.PrivateScope |
				methodInfoDeclaration.Attributes & MethodAttributes.SpecialName;

			if (isInterface)
				attrs |= MethodAttributes.Private;
			else if ((attrs & MethodAttributes.SpecialName) != 0)
				attrs |= MethodAttributes.Public;
			else
				attrs |= methodInfoDeclaration.Attributes & 
					(MethodAttributes.Public | MethodAttributes.Private);

			return DefineMethod(name, methodInfoDeclaration, attrs);
		}

		#endregion

		/// <summary>
		/// Creates a Type object for the class.
		/// </summary>
		/// <returns>Returns the new Type object for this class.</returns>
		public Type Create()
		{
			return TypeBuilder.CreateType();
		}

		/// <summary>
		/// Sets a custom attribute.
		/// </summary>
		/// <param name="attributeType">Attribute type</param>
		public void SetCustomAttribute(Type attributeType)
		{
			if (attributeType == null) throw new ArgumentNullException("attributeType");

			ConstructorInfo        ci        = attributeType.GetConstructor(Type.EmptyTypes);
			CustomAttributeBuilder caBuilder = new CustomAttributeBuilder(ci, new object[0]);

			_typeBuilder.SetCustomAttribute(caBuilder);
		}

		public void SetCustomAttribute(Type attributeType, PropertyInfo[] properties, object[] propertyValues)
		{
			if (attributeType == null) throw new ArgumentNullException("attributeType");

			ConstructorInfo        ci        = attributeType.GetConstructor(Type.EmptyTypes);
			CustomAttributeBuilder caBuilder = new CustomAttributeBuilder(
				ci, new object[0], properties, propertyValues );

			_typeBuilder.SetCustomAttribute(caBuilder);
		}

		public void SetCustomAttribute(Type attributeType, string propertyName, object propertyValue)
		{
			SetCustomAttribute(
				attributeType,
				new PropertyInfo[] { attributeType.GetProperty(propertyName) },
				new object[]       { propertyValue } );
		}

		private ConstructorBuilderHelper _typeInitializer;
		/// <summary>
		/// Gets the initializer for this type.
		/// </summary>
		public ConstructorBuilderHelper TypeInitializer
		{
			get 
			{
				if (_typeInitializer == null)
					_typeInitializer = new ConstructorBuilderHelper(this, _typeBuilder.DefineTypeInitializer());

				return _typeInitializer;
			}
		}

		public bool IsTypeInitializerDefined
		{
			get { return _typeInitializer != null; }
		}

		private ConstructorBuilderHelper _defaultConstructor;
		/// <summary>
		/// Gets the default constructor for this type.
		/// </summary>
		public  ConstructorBuilderHelper  DefaultConstructor
		{
			get 
			{
				if (_defaultConstructor == null)
				{
					ConstructorBuilder builder = _typeBuilder.DefineConstructor(
						MethodAttributes.Public,
						CallingConventions.Standard,
						Type.EmptyTypes);

					_defaultConstructor = new ConstructorBuilderHelper(this, builder);
				}

				return _defaultConstructor;
			}
		}

		public bool IsDefaultConstructorDefined
		{
			get { return _defaultConstructor != null; }
		}

		private ConstructorBuilderHelper _initConstructor;
		/// <summary>
		/// Gets the init context constructor for this type.
		/// </summary>
		public  ConstructorBuilderHelper  InitConstructor
		{
			get 
			{
				if (_initConstructor == null)
				{
					ConstructorBuilder builder = _typeBuilder.DefineConstructor(
						MethodAttributes.Public, 
						CallingConventions.Standard,
						new Type[] { typeof(InitContext) });

					_initConstructor = new ConstructorBuilderHelper(this, builder);
				}

				return _initConstructor;
			}
		}

		public bool IsInitConstructorDefined
		{
			get { return _initConstructor != null; }
		}

		/// <summary>
		/// Adds a new field to the class, with the given name, attributes and field type.
		/// </summary>
		/// <param name="fieldName">The name of the field. fieldName cannot contain embedded nulls.</param>
		/// <param name="type">The type of the field.</param>
		/// <param name="attributes">The attributes of the field.</param>
		/// <returns>The defined field.</returns>
		public FieldBuilder DefineField(string fieldName, Type type, FieldAttributes attributes)
		{
			return _typeBuilder.DefineField(fieldName, type, attributes);
		}

		#region DefineConstructor Overrides

		public ConstructorBuilderHelper DefinePublicConstructor(params Type[] parameterTypes)
		{
			return new ConstructorBuilderHelper(
				this,
				_typeBuilder.DefineConstructor(
					MethodAttributes.Public, CallingConventions.Standard, parameterTypes));
		}

		public ConstructorBuilderHelper DefineConstructor(
			MethodAttributes   attributes,
			CallingConventions callingConventions,
			params Type[]      parameterTypes)
		{
			return new ConstructorBuilderHelper(
				this,
				_typeBuilder.DefineConstructor(attributes, callingConventions, parameterTypes));
		}

		#endregion

		#region DefineNestedType Overrides

		/// <summary>
		/// Defines a nested type given its name..
		/// </summary>
		/// <param name="name">The short name of the type.</param>
		/// <returns>Returns the created <b>TypeBuilderHelper</b>.</returns>
		/// <seealso cref="System.Reflection.Emit.TypeBuilder.DefineNestedType(string)">
		/// TypeBuilder.DefineNestedType Method</seealso>
		public TypeBuilderHelper DefineNestedType(string name)
		{
			return new TypeBuilderHelper(_assembly, _typeBuilder.DefineNestedType(name));
		}

		/// <summary>
		/// Defines a public nested type given its name and the type that it extends.
		/// </summary>
		/// <param name="name">The short name of the type.</param>
		/// <param name="parent">The type that the nested type extends.</param>
		/// <returns>Returns the created <b>TypeBuilderHelper</b>.</returns>
		/// <seealso cref="System.Reflection.Emit.TypeBuilder.DefineNestedType(string,TypeAttributes,Type)">
		/// TypeBuilder.DefineNestedType Method</seealso>
		public TypeBuilderHelper DefineNestedType(string name, Type parent)
		{
			return new TypeBuilderHelper(
				_assembly, _typeBuilder.DefineNestedType(name, TypeAttributes.NestedPublic, parent));
		}

		/// <summary>
		/// Defines a nested type given its name, attributes, and the type that it extends.
		/// </summary>
		/// <param name="name">The short name of the type.</param>
		/// <param name="attrs">The attributes of the type.</param>
		/// <param name="parent">The type that the nested type extends.</param>
		/// <returns>Returns the created <b>TypeBuilderHelper</b>.</returns>
		/// <seealso cref="System.Reflection.Emit.TypeBuilder.DefineNestedType(string,TypeAttributes,Type)">
		/// TypeBuilder.DefineNestedType Method</seealso>
		public TypeBuilderHelper DefineNestedType(string name, TypeAttributes attrs, Type parent)
		{
			return new TypeBuilderHelper(
				_assembly, _typeBuilder.DefineNestedType(name, attrs, parent));
		}

		/// <summary>
		/// Defines a public nested type given its name, the type that it extends, and the interfaces that it implements.
		/// </summary>
		/// <param name="name">The short name of the type.</param>
		/// <param name="parent">The type that the nested type extends.</param>
		/// <param name="interfaces">The interfaces that the nested type implements.</param>
		/// <returns>Returns the created <b>TypeBuilderHelper</b>.</returns>
		/// <seealso cref="System.Reflection.Emit.TypeBuilder.DefineNestedType(string,TypeAttributes,Type,Type[])">
		/// TypeBuilder.DefineNestedType Method</seealso>
		public TypeBuilderHelper DefineNestedType(string name, Type parent, params Type[] interfaces)
		{
			return new TypeBuilderHelper(
				_assembly, _typeBuilder.DefineNestedType(name, TypeAttributes.NestedPublic, parent, interfaces));
		}

		/// <summary>
		/// Defines a nested type given its name, attributes, the type that it extends, and the interfaces that it implements.
		/// </summary>
		/// <param name="name">The short name of the type.</param>
		/// <param name="attrs">The attributes of the type.</param>
		/// <param name="parent">The type that the nested type extends.</param>
		/// <param name="interfaces">The interfaces that the nested type implements.</param>
		/// <returns>Returns the created <b>TypeBuilderHelper</b>.</returns>
		/// <seealso cref="System.Reflection.Emit.ModuleBuilder.DefineType(string,TypeAttributes,Type,Type[])">ModuleBuilder.DefineType Method</seealso>
		public TypeBuilderHelper DefineNestedType(string name, TypeAttributes attrs, Type parent, params Type[] interfaces)
		{
			return new TypeBuilderHelper(
				_assembly, _typeBuilder.DefineNestedType(name, attrs, parent, interfaces));
		}

		#endregion

	}
}
