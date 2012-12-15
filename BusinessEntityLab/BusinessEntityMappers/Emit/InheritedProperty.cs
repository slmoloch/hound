using System;
using System.Reflection;
using System.Reflection.Emit;

namespace BusinessEntityMappers.Emit
{
	internal class InheritedProperty
	{
		internal delegate void MethodBuilderDelegate(ILGenerator ilg);

		private readonly MethodBuilderDelegate setDelegate;
		private readonly MethodBuilderDelegate getDelegate;

		public InheritedProperty(MethodBuilderDelegate setDelegate, MethodBuilderDelegate getDelegate)
		{
			this.setDelegate = setDelegate;
			this.getDelegate = getDelegate;
		}

		public void GenerateIn(TypeBuilder typeBuilder, PropertyInfo info)
		{
			string name = info.Name;
			Type type = info.PropertyType;

            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(name, PropertyAttributes.SpecialName , type, null);

			if (setDelegate != null)
			{
				MethodInfo inheritedMethod = info.GetSetMethod();

//				MethodBuilder methb = typeBuilder.DefineMethod("set_" + name, inheritedMethod.Attributes ^ MethodAttributes.Abstract,
//																											 inheritedMethod.ReturnType, new Type[] { type });
//                
                MethodBuilder methb = typeBuilder.DefineMethod(inheritedMethod.Name, inheritedMethod.Attributes ^ MethodAttributes.Abstract, inheritedMethod.ReturnType, new Type[] { type });
				ILGenerator ilg = methb.GetILGenerator();

				setDelegate(ilg);

				//propertyBuilder.SetSetMethod(methb);
				typeBuilder.DefineMethodOverride(methb, inheritedMethod);
			}

			if (getDelegate != null)
			{
				MethodInfo inheritedMethod = info.GetGetMethod();

//				MethodBuilder methb = typeBuilder.DefineMethod("get_" + name, inheritedMethod.Attributes ^ MethodAttributes.Abstract,
//																											 inheritedMethod.ReturnType, new Type[] { });
                MethodBuilder methb = typeBuilder.DefineMethod(inheritedMethod.Name, inheritedMethod.Attributes ^ MethodAttributes.Abstract, inheritedMethod.ReturnType, new Type[] { });
				ILGenerator ilg = methb.GetILGenerator();

				getDelegate(ilg);

				//propertyBuilder.SetGetMethod(methb);
				typeBuilder.DefineMethodOverride(methb, inheritedMethod);
			}
		}
	}
}