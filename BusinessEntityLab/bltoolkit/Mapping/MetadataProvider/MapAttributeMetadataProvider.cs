using System;
using System.Collections;
using System.Reflection;

using BLToolkit.Reflection;
using BLToolkit.Reflection.Extension;

namespace BLToolkit.Mapping.MetadataProvider
{
	public class MapAttributeMetadataProvider : MapMetadataProvider
	{
		#region Helpers

		private object[] _mapFieldAttributes;

		private object[] GetMapFieldAttributes(ObjectMapper mapper)
		{
			if (_mapFieldAttributes == null)
				_mapFieldAttributes = TypeHelper.GetAttributes(mapper.TypeAccessor.Type, typeof(MapFieldAttribute));

			return _mapFieldAttributes;
		}

		#endregion

		#region GetFieldName

		public override string GetFieldName(ObjectMapper mapper, MemberAccessor member, out bool isSet)
		{
			MapFieldAttribute a = (MapFieldAttribute)member.GetAttribute(typeof(MapFieldAttribute));

			if (a != null)
			{
				isSet = true;
				return a.MapName;
			}

			string name = member.Name.ToLower();

			foreach (MapFieldAttribute attr in GetMapFieldAttributes(mapper))
			{
				if (attr.OrigName.ToLower() == name)
				{
					isSet = true;
					return attr.MapName;
				}
			}

			return base.GetFieldName(mapper, member, out isSet);
		}

		#endregion

		#region EnsureMapper

		public override void EnsureMapper(ObjectMapper mapper, EnsureMapperHandler handler)
		{
			foreach (MapFieldAttribute attr in GetMapFieldAttributes(mapper))
				handler(attr.MapName, attr.OrigName);
		}

		#endregion

		#region GetIgnore

		public override bool GetIgnore(ObjectMapper mapper, MemberAccessor member, out bool isSet)
		{
			MapIgnoreAttribute attr = 
				(MapIgnoreAttribute)member.GetAttribute(typeof(MapIgnoreAttribute));
			if (attr == null)
				attr = (MapIgnoreAttribute)TypeHelper.GetFirstAttribute(member.Type, typeof(MapIgnoreAttribute));

			if (attr != null)
			{
				isSet = true;
				return attr.Ignore;
			}

			if (member.GetAttribute(typeof(MapImplicitAttribute)) != null ||
				TypeHelper.GetFirstAttribute(member.Type, typeof(MapImplicitAttribute)) != null)
			{
				isSet = true;
				return false;
			}

			return base.GetIgnore(mapper, member, out isSet);
		}

		#endregion

		#region GetTrimmable

		public override bool GetTrimmable(ObjectMapper mapper, MemberAccessor member, out bool isSet)
		{
			if (member.Type == typeof(string))
			{
				TrimmableAttribute attr =
					(TrimmableAttribute)member.GetAttribute(typeof(TrimmableAttribute));

				if (attr != null)
				{
					isSet = true;
					return attr.IsTrimmable;
				}

				attr = (TrimmableAttribute)TypeHelper.GetFirstAttribute(
					member.MemberInfo.DeclaringType, typeof(TrimmableAttribute));

				if (attr != null)
				{
					isSet = true;
					return attr.IsTrimmable;
				}
			}

			return base.GetTrimmable(mapper, member, out isSet);
		}

		#endregion

		#region GetMapValues

		public override MapValue[] GetMapValues(ObjectMapper mapper, MemberAccessor member, out bool isSet)
		{
			ArrayList list = null;

			object[] attrs = member.GetAttributes(typeof(MapValueAttribute));

			if (attrs != null)
			{
				list = new ArrayList(attrs.Length);

				foreach (MapValueAttribute a in attrs)
					list.Add(new MapValue(a.OrigValue, a.Values));
			}

			attrs = member.GetTypeAttributes(typeof(MapValueAttribute));

			if (attrs != null && attrs.Length > 0)
			{
				if (list == null)
					list = new ArrayList(attrs.Length);

				foreach (MapValueAttribute a in attrs)
					if (a.Type == null && a.OrigValue != null && a.OrigValue.GetType() == member.Type ||
						a.Type != null && a.Type == member.Type)
						list.Add(new MapValue(a.OrigValue, a.Values));
			}

			MapValue[] typeMapValues = GetMapValues(mapper.Extension, member.Type, out isSet);

			if (list == null)
				return typeMapValues;

			if (typeMapValues != null)
				list.AddRange(typeMapValues);

			isSet = true;

			return (MapValue[])list.ToArray(typeof(MapValue));
		}

		const FieldAttributes EnumField = FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.Literal;

		static ArrayList GetEnumMapValues(Type type)
		{
			ArrayList   list   = null;
			FieldInfo[] fields = type.GetFields();

			foreach (FieldInfo fi in fields)
			{
				if ((fi.Attributes & EnumField) == EnumField)
				{
					Attribute[] enumAttributes = Attribute.GetCustomAttributes(fi, typeof(MapValueAttribute));

					foreach (MapValueAttribute attr in enumAttributes)
					{
						if (list == null)
							list = new ArrayList(fields.Length);

						object origValue = Enum.Parse(type, fi.Name);

						list.Add(new MapValue(origValue, attr.Values));
					}
				}
			}

			return list;
		}

		public override MapValue[] GetMapValues(TypeExtension typeExt, Type type, out bool isSet)
		{
			ArrayList list = null;

			if (type.IsEnum)
				list = GetEnumMapValues(type);

			object[] attrs = TypeHelper.GetAttributes(type, typeof(MapValueAttribute));

			if (attrs != null && attrs.Length != 0)
			{
				if (list == null)
					list = new ArrayList(attrs.Length);

				for (int i = 0; i < attrs.Length; i++)
				{
					MapValueAttribute a = (MapValueAttribute)attrs[i];
					list.Add(new MapValue(a.OrigValue, a.Values));
				}
			}

			isSet = list != null;

			return isSet? (MapValue[])list.ToArray(typeof(MapValue)): null;
		}

		#endregion

		#region GetDefaultValue

		public override object GetDefaultValue(ObjectMapper mapper, MemberAccessor member, out bool isSet)
		{
			// Check member [DefaultValue(0)]
			//
			DefaultValueAttribute attr =
				(DefaultValueAttribute)member.GetAttribute(typeof(DefaultValueAttribute));

			if (attr != null)
			{
				isSet = true;
				return attr.Value;
			}

			// Check type [DefaultValues(typeof(int), 0)]
			//
			object[] attrs = member.GetTypeAttributes(typeof(DefaultValueAttribute));

			foreach (DefaultValueAttribute a in attrs)
				if (a.Type == null && a.Value != null && a.Value.GetType() == member.Type ||
					a.Type != null && a.Type == member.Type)
				{
					isSet = true;
					return a.Value;
				}

			return GetDefaultValue(mapper.Extension, member.Type, out isSet);
		}

		public override object GetDefaultValue(TypeExtension typeExt, Type type, out bool isSet)
		{
			object value = null;

			if (type.IsEnum)
				value = GetEnumDefaultValueFromType(type);

			if (value == null)
			{
				object[] attrs = TypeHelper.GetAttributes(type, typeof(DefaultValueAttribute));

				if (attrs != null && attrs.Length != 0)
					value = ((DefaultValueAttribute)attrs[0]).Value;
			}

			isSet = value != null;

			return TypeExtension.ChangeType(value, type);
		}

		private static object GetEnumDefaultValueFromType(Type type)
		{
			FieldInfo[] fields = type.GetFields();

			foreach (FieldInfo fi in fields)
			{
				if ((fi.Attributes & EnumField) == EnumField)
				{
					Attribute[] attrs = Attribute.GetCustomAttributes(fi, typeof(DefaultValueAttribute));

					if (attrs.Length > 0)
						return Enum.Parse(type, fi.Name);
				}
			}

			return null;
		}

		#endregion

		#region GetNullable

		public override bool GetNullable(ObjectMapper mapper, MemberAccessor member, out bool isSet)
		{
			// Check member [Nullable(true | false)]
			//
			NullableAttribute attr1 =
				(NullableAttribute)member.GetAttribute(typeof(NullableAttribute));

			if (attr1 != null)
			{
				isSet = true;
				return attr1.IsNullable;
			}

			// Check member [NullValue(0)]
			//
			NullValueAttribute attr2 =
				(NullValueAttribute)member.GetAttribute(typeof(NullValueAttribute));

			if (attr2 != null)
				return isSet = true;

			// Check type [Nullable(true || false)]
			//
			attr1 = (NullableAttribute)TypeHelper.GetFirstAttribute(
				member.MemberInfo.DeclaringType, typeof(NullableAttribute));

			if (attr1 != null)
			{
				isSet = true;
				return attr1.IsNullable;
			}

			// Check type [NullValues(typeof(int), 0)]
			//
			object[] attrs = member.GetTypeAttributes(typeof(NullValueAttribute));

			foreach (NullValueAttribute a in attrs)
				if (a.Type == null && a.Value != null && a.Value.GetType() == member.Type ||
					a.Type != null && a.Type == member.Type)
					return isSet = true;

			if (member.Type.IsEnum)
				return isSet = mapper.MappingSchema.GetNullValue(member.Type) != null;

			isSet = false;
			return false;
		}

		#endregion

		#region GetNullable

		private static object CheckNullValue(object value, MemberAccessor member)
		{
			if (value is Type && (Type)value == typeof(DBNull))
			{
				value = DBNull.Value;

				if (member.Type == typeof(string))
					value = null;
			}

			return value;
		}

		public override object GetNullValue(ObjectMapper mapper, MemberAccessor member, out bool isSet)
		{
			// Check member [NullValue(0)]
			//
			NullValueAttribute attr =
				(NullValueAttribute)member.GetAttribute(typeof(NullValueAttribute));

			if (attr != null)
			{
				isSet = true;
				return CheckNullValue(attr.Value, member);
			}

			// Check type [NullValues(typeof(int), 0)]
			//
			object[] attrs = member.GetTypeAttributes(typeof(NullValueAttribute));

			foreach (NullValueAttribute a in attrs)
			{
				if (a.Type == null && a.Value != null && a.Value.GetType() == member.Type ||
					a.Type != null && a.Type == member.Type)
				{
					isSet = true;
					return CheckNullValue(a.Value, member);
				}
			}

			if (member.Type.IsEnum)
			{
				object value = CheckNullValue(mapper.MappingSchema.GetNullValue(member.Type), member);

				if (value != null)
				{
					isSet = true;
					return value;
				}
			}

			isSet = false;
			return null;
		}

		#endregion
	}
}
