using System;

using BLToolkit.Reflection.Emit;

namespace BLToolkit.TypeBuilder.Builders
{
	public interface ITypeBuilder
	{
		string AssemblyNameSuffix { get; }
		Type   Build (Type sourceType, AssemblyBuilderHelper assemblyBuilder);
	}
}
