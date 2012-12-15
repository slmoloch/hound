using System;
using System.Collections.Generic;
using BLToolkit.Reflection.Emit;

namespace BusinessEntityMappers.Emit
{
	public class RuntimeStateFactory
	{
		private static AssemblyBuilderHelper assemblyBuilder;
		private static readonly IDictionary<Type, BusinessEntityTypes> typeCache = new Dictionary<Type, BusinessEntityTypes>();

		private static AssemblyBuilderHelper AssemblyBuilder
		{
			get
			{
				if (assemblyBuilder == null)
				{
					assemblyBuilder = new AssemblyBuilderHelper("O2IDynamicTypes.dll");
				}

				return assemblyBuilder;
			}
		}

		public T CreateContext<T>(IEntityLoader<T> loader)
		{
			if (!typeCache.ContainsKey(typeof(T)))
			{
				typeCache.Add(typeof(T), StateContextGenerator.GenerateContextTypes<T>(AssemblyBuilder));
			}

			return StateContextActivator.Activate(typeCache[typeof(T)], loader);
		}
	}
}