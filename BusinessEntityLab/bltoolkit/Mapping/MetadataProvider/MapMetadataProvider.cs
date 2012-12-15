using System;

using BLToolkit.Reflection;
using BLToolkit.Reflection.Extension;

namespace BLToolkit.Mapping.MetadataProvider
{
	public delegate void                OnCreateProvider(MapMetadataProvider parentProvider);
	public delegate MapMetadataProvider CreateProvider();
	public delegate MemberMapper        EnsureMapperHandler(string mapName, string origName);

	public abstract class MapMetadataProvider
	{
		#region Provider Support

		public virtual void AddProvider(MapMetadataProvider provider)
		{
		}

		public virtual void InsertProvider(int index, MapMetadataProvider provider)
		{
		}

		public virtual MapMetadataProvider[] GetProviders()
		{
			return new MapMetadataProvider[0];
		}

		#endregion

		#region GetFieldName

		public virtual string GetFieldName(ObjectMapper mapper, MemberAccessor member, out bool isSet)
		{
			isSet = false;
			return member.Name;
		}

		#endregion

		#region EnsureMapper

		public virtual void EnsureMapper(ObjectMapper mapper, EnsureMapperHandler handler)
		{
		}

		#endregion

		#region GetIgnore

		public virtual bool GetIgnore(ObjectMapper mapper, MemberAccessor member, out bool isSet)
		{
			isSet = false;

			return
				TypeHelper.IsScalar(member.Type) == false;// ||
				//(member.MemberInfo is FieldInfo && ((FieldInfo)member.MemberInfo).IsLiteral);
		}

		#endregion

		#region GetTrimmable

		public virtual bool GetTrimmable(ObjectMapper mapper, MemberAccessor member, out bool isSet)
		{
			isSet = member.Type != typeof(string);
			return isSet? false: TrimmableAttribute.Default.IsTrimmable;
		}

		#endregion

		#region GetMapValues

		public virtual MapValue[] GetMapValues(ObjectMapper mapper, MemberAccessor member, out bool isSet)
		{
			isSet = false;
			return null;
		}

		public virtual MapValue[] GetMapValues(TypeExtension typeExt, Type type, out bool isSet)
		{
			isSet = false;
			return null;
		}

		#endregion

		#region GetDefaultValue

		public virtual object GetDefaultValue(ObjectMapper mapper, MemberAccessor member, out bool isSet)
		{
			isSet = false;
			return null;
		}

		public virtual object GetDefaultValue(TypeExtension typeExt, Type type, out bool isSet)
		{
			isSet = false;
			return null;
		}

		#endregion

		#region GetNullable

		public virtual bool GetNullable(ObjectMapper mapper, MemberAccessor member, out bool isSet)
		{
			isSet = false;
			return false;
		}

		#endregion

		#region GetNullValue

		public virtual object GetNullValue(ObjectMapper mapper, MemberAccessor member, out bool isSet)
		{
			isSet = false;

			if (member.Type.IsEnum)
				return null;

			object value = mapper.MappingSchema.GetNullValue(member.Type);

			if (value is Type && (Type)value == typeof(DBNull))
			{
				value = DBNull.Value;

				if (member.Type == typeof(string))
					value = null;
			}

			return value;
		}

		#endregion

		#region Static Members

		public static event OnCreateProvider OnCreateProvider;

		private static CreateProvider _createProvider = new CreateProvider(CreateInternal);
		public  static CreateProvider  CreateProvider
		{
			get { return _createProvider; }
			set { _createProvider = value != null? value: new CreateProvider(CreateInternal); }
		}

		private static MapMetadataProvider CreateInternal()
		{
			MapMetadataProviderList list = new MapMetadataProviderList();

			if (OnCreateProvider != null)
				OnCreateProvider(list);

			return list;
		}

		#endregion
	}
}
