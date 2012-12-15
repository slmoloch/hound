using System;
using System.Reflection;
using System.Reflection.Emit;

using BLToolkit.TypeBuilder.Builders;
using BLToolkit.Reflection.Emit;
using BLToolkit.TypeBuilder;

namespace BLToolkit.Aspects
{
	public class MixinAspectBuilder : AbstractTypeBuilderBase
	{
		public MixinAspectBuilder(
			Type targetInterface, string memberName, bool throwExceptionIfNull, string exceptionMessage)
		{
			_targetInterface      = targetInterface;
			_memberName           = memberName;
			_throwExceptionIfNull = throwExceptionIfNull;
			_exceptionMessage     = exceptionMessage;
		}

		private readonly Type   _targetInterface;
		private readonly string _memberName;
		private readonly bool   _throwExceptionIfNull;
		private readonly string _exceptionMessage;

		public override bool IsApplied(BuildContext context, AbstractTypeBuilderList builders)
		{
			return context.BuildElement == BuildElement.InterfaceMethod;
		}

		public override Type[] GetInterfaces()
		{
			return new Type[] { _targetInterface };
		}

		public override void Build(BuildContext context)
		{
			Context = context;

			if (CheckOverrideAttribute())
				return;

			EmitHelper      emit   = Context.MethodBuilder.Emitter;
			MethodInfo      method = Context.MethodBuilder.OverriddenMethod;
			ParameterInfo[] ps     = method.GetParameters();

			FieldInfo field = Context.Type.GetField(_memberName);

			if (field != null)
			{
				if (field.IsPrivate)
					throw new TypeBuilderException(string.Format(
						"Field '{0}.{1}' must be protected or public.",
						Context.Type.Name, _memberName));

				emit
					.ldarg_0
					.ldfld   (field)
					;

				CheckNull(emit);

				emit
					.ldarg_0
					.ldfld   (field)
					;
			}
			else
			{
				PropertyInfo prop = Context.Type.GetProperty(_memberName);

				if (prop != null)
				{
					MethodInfo mi = prop.GetGetMethod(true);

					if (mi == null)
						throw new TypeBuilderException(string.Format(
							"Property '{0}.{1}' getter not found.",
							Context.Type.Name, _memberName));

					if (mi.IsPrivate)
						throw new TypeBuilderException(string.Format(
							"Property '{0}.{1}' getter must be protected or public.",
							Context.Type.Name, _memberName));

					emit
						.ldarg_0
						.callvirt (mi)
						;

					CheckNull(emit);

					emit
						.ldarg_0
						.callvirt (mi)
						;
				}
				else
				{
					throw new TypeBuilderException(string.Format(
						"Member '{0}.{1}' not found.",
						Context.Type.Name, _memberName));
				}
			}

			for (int i = 0; i < ps.Length; i++)
				emit.ldarg(i + 1);

			emit.callvirt(method);

			if (Context.ReturnValue != null)
				emit.stloc(Context.ReturnValue);
		}

		private void CheckNull(EmitHelper emit)
		{
			if (_throwExceptionIfNull == false &&
				(_exceptionMessage == null || _exceptionMessage.Length == 0))
			{
				emit
					.brfalse (Context.ReturnLabel)
					;
			}
			else
			{
				string message = string.Format(
					_exceptionMessage == null || _exceptionMessage.Length == 0?
						"'{0}.{1}' is not initialized." : _exceptionMessage,
					_targetInterface.Name, _memberName, _targetInterface.FullName);

				Label label = emit.DefineLabel();

				emit
					.brtrue    (label)
					.ldstr     (message)
					.newobj    (typeof(InvalidOperationException), typeof(string))
					.@throw
					.MarkLabel (label)
					;
			}
		}

		private bool CheckOverrideAttribute()
		{
			MethodInfo      method = Context.MethodBuilder.OverriddenMethod;
			ParameterInfo[] ps     = method.GetParameters();

			MethodInfo[] methods = Context.Type.GetMethods(
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			foreach (MethodInfo mi in methods)
			{
				if (mi.IsPrivate)
					continue;

				object[] attrs = mi.GetCustomAttributes(typeof(MixinOverrideAttribute), true);

				if (attrs == null || attrs.Length == 0)
					continue;

				foreach (MixinOverrideAttribute attr in attrs)
				{
					if (attr.TargetInterface != null &&
						attr.TargetInterface != Context.CurrentInterface.Type)
						continue;

					string name = attr.MethodName == null || attr.MethodName.Length == 0?
						mi.Name: attr.MethodName;

					if (name != method.Name || mi.ReturnType != method.ReturnType)
						continue;

					ParameterInfo[] mips = mi.GetParameters();

					if (mips.Length != ps.Length)
						continue;

					bool equal = true;

					for (int i = 0; equal && i < ps.Length; i++)
						equal = ps[i].ParameterType == mips[i].ParameterType;

					if (equal)
					{
						EmitHelper emit = Context.MethodBuilder.Emitter;

						for (int i = -1; i < ps.Length; i++)
							emit.ldarg(i + 1);

						emit.callvirt(mi);

						if (Context.ReturnValue != null)
							emit.stloc(Context.ReturnValue);

						return true;
					}
				}
			}

			return false;
		}
	}
}
