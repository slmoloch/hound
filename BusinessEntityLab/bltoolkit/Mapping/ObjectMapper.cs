using System;
using System.Collections;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using BLToolkit.Mapping.MetadataProvider;
using BLToolkit.Reflection;
using BLToolkit.Reflection.Extension;

namespace BLToolkit.Mapping
{
#if FW2
	[DebuggerDisplay("Type = {TypeAccessor.Type}, OriginalType = {TypeAccessor.OriginalType}")]
#endif
	public class ObjectMapper : MapDataSourceDestinationBase, IEnumerable
	{
		#region Constructor

		public ObjectMapper()
		{
			_members      = new ArrayList();
			_nameToMember = new Hashtable();
		}

		#endregion

		#region Protected Members

		protected virtual MemberMapper CreateMemberMapper(MapMemberInfo mapMemberInfo)
		{
			if (mapMemberInfo == null) throw new ArgumentNullException("mapMemberInfo");

			MemberMapper mm = null;

			Attribute attr = mapMemberInfo.MemberAccessor.GetAttribute(typeof(MemberMapperAttribute));
			if (attr == null)
			{
				object[] attrs = TypeHelper.GetAttributes(mapMemberInfo.Type, typeof(MemberMapperAttribute));

				foreach (MemberMapperAttribute a in attrs)
				{
					if (a.MemberType == null)
					{
						mm = a.MemberMapper;
						break;
					}
				}
			}
			else
				mm = ((MemberMapperAttribute)attr).MemberMapper;

			if (mm == null)
			{
				object[] attrs = TypeHelper.GetAttributes(
					mapMemberInfo.MemberAccessor.MemberInfo.DeclaringType, typeof(MemberMapperAttribute));

				foreach (MemberMapperAttribute a in attrs)
				{
					if (a.MemberType == mapMemberInfo.Type)
					{
						mm = a.MemberMapper;
						break;
					}
				}
			}

			if (mm == null)
				mm = MemberMapper.CreateMemberMapper(mapMemberInfo);

			mm.Init(mapMemberInfo);

			return mm;
		}

		[SuppressMessage("Microsoft.Performance", "CA1807:AvoidUnnecessaryStringCreation", MessageId = "stack1")]
		protected virtual void Add(MemberMapper memberMapper)
		{
			if (memberMapper == null) throw new ArgumentNullException("memberMapper");

			memberMapper.SetOrdinal(_members.Count);

			_members.     Add(memberMapper);
			_nameToMember.Add(memberMapper.Name.ToLower(), memberMapper);
		}

		protected virtual MapMetadataProvider CreateMetadataProvider()
		{
			return MapMetadataProvider.CreateProvider();
		}

		#endregion

		#region Public Members

		private readonly ArrayList _members;
		public  MemberMapper this[int index]
		{
			get { return (MemberMapper)_members[index]; }
		}

		private TypeExtension _extension;
		public  TypeExtension  Extension
		{
			get { return _extension;  }
			set { _extension = value; }
		}

		private MapMetadataProvider _metadataProvider;
		public  MapMetadataProvider  MetadataProvider
		{
			get
			{
				if (_metadataProvider == null)
					_metadataProvider = CreateMetadataProvider();
				return _metadataProvider;
			}
			set { _metadataProvider = value; }
		}

		private string[] _fieldNames;
		public  string[]  FieldNames
		{
			get
			{
				if (_fieldNames == null)
				{
					_fieldNames = new string[_members.Count];

					for (int i = 0; i < _fieldNames.Length; i++)
					{
						_fieldNames[i] = ((MemberMapper)_members[i]).Name;
					}
				}

				return _fieldNames;
			}
		}

		private readonly Hashtable _nameToMember;
		public  MemberMapper this[string name]
		{
			get
			{
				if (name == null) throw new ArgumentNullException("name");

				MemberMapper mm = (MemberMapper)_nameToMember[name];

				if (mm == null)
				{
					lock (_nameToMember.SyncRoot)
					{
						mm = (MemberMapper)_nameToMember[name];

						if (mm == null)
						{
							mm = (MemberMapper)_nameToMember[name.ToLower(CultureInfo.CurrentCulture)];

							if (mm == null)
							{
								mm = GetComplexMapper(name, name);

								if (mm != null)
								{
									if (_members.Contains(mm))
										throw new MappingException(string.Format(
											"Wrong mapping field name: '{0}', type: '{1}'. Use field name '{2}' instead.",
											name, _typeAccessor.OriginalType.Name, mm.Name));

									Add(mm);
								}
							}
							else
								_nameToMember[name] = mm;
						}
					}
				}

				return mm;
			}
		}

		public MemberMapper this[string name, bool byPropertyName]
		{
			get
			{
				if (byPropertyName)
				{
					foreach (MemberMapper ma in _members)
						if (ma.MemberName == name)
							return ma;

					return null;
				}

				return this[name];
			}
		}

		public int GetOrdinal(string name, bool byPropertyName)
		{
			if (byPropertyName)
			{
				for (int i = 0; i < _members.Count; ++i)
					if (((MemberMapper)_members[i]).MemberName == name)
						return i;

				return -1;
			}

			return GetOrdinal(name);
		}

		private TypeAccessor _typeAccessor;
		public  TypeAccessor  TypeAccessor
		{
			get { return _typeAccessor; }
		}

		private MappingSchema _mappingSchema;
		public  MappingSchema  MappingSchema
		{
			get { return _mappingSchema; }
		}

		#endregion

		#region Init Mapper

		public virtual void Init(MappingSchema mappingSchema, Type type)
		{
			if (type == null) throw new ArgumentNullException("type");

			_typeAccessor  = TypeAccessor.GetAccessor(type);
			_mappingSchema = mappingSchema;
			_extension     = TypeExtension.GetTypeExtension(
				_typeAccessor.OriginalType, mappingSchema.Extensions);

			foreach (MemberAccessor ma in _typeAccessor)
			{
				if (GetIgnore(ma))
					continue;

				MapFieldAttribute mapFieldAttr = (MapFieldAttribute)ma.GetAttribute(typeof(MapFieldAttribute));

				if (mapFieldAttr == null || (mapFieldAttr.OrigName == null && mapFieldAttr.Format == null))
				{
					MapMemberInfo mi = new MapMemberInfo();

					mi.MemberAccessor  = ma;
					mi.Type            = ma.Type;
					mi.MappingSchema   = mappingSchema;
					mi.MemberExtension = _extension[ma.Name];
					mi.Name            = GetFieldName   (ma);
					mi.MemberName      = ma.Name;
					mi.Trimmable       = GetTrimmable   (ma);
					mi.MapValues       = GetMapValues   (ma);
					mi.DefaultValue    = GetDefaultValue(ma);
					mi.Nullable        = GetNullable    (ma);
					mi.NullValue       = GetNullValue   (ma, mi.Nullable);

					Add(CreateMemberMapper(mi));
				}
				else if (mapFieldAttr.OrigName != null)
				{
					EnsureMapper(mapFieldAttr.MapName, ma.Name + "." + mapFieldAttr.OrigName);
				}
				else if (mapFieldAttr.Format != null)
				{
					foreach (MemberMapper inner in _mappingSchema.GetObjectMapper(ma.Type))
						EnsureMapper(string.Format(mapFieldAttr.Format, inner.Name), ma.Name + "." + inner.MemberName);
				}
			}

			foreach (AttributeExtension ae in _extension.Attributes["MapField"])
			{
				string mapName  = (string)ae["MapName"];
				string origName = (string)ae["OrigName"];

				if (mapName == null || origName == null)
					throw new MappingException(string.Format(
						"Type '{0}' has invalid  extension. MapField MapName='{1}' OrigName='{2}'.",
							type.FullName, mapName, origName));

				EnsureMapper(mapName, origName);
			}

			MetadataProvider.EnsureMapper(this, new EnsureMapperHandler(EnsureMapper));
		}

		private MemberMapper EnsureMapper(string mapName, string origName)
		{
			MemberMapper mm = this[mapName];

			if (mm == null)
			{
				string name = mapName.ToLower();

				foreach (MemberMapper m in _members)
				{
					if (m.MemberAccessor.Name.ToLower() == name)
					{
						_nameToMember.Add(name, m);
						return m;
					}
				}

				mm = GetComplexMapper(mapName, origName);

				if (mm != null)
					Add(mm);
			}

			return mm;
		}

		private readonly Hashtable _nameToComplexMapper          = new Hashtable();
		private readonly Hashtable _nameToNotFoundComplexMappers = new Hashtable();

		[SuppressMessage("Microsoft.Performance", "CA1807:AvoidUnnecessaryStringCreation", MessageId = "stack0")]
		[SuppressMessage("Microsoft.Performance", "CA1807:AvoidUnnecessaryStringCreation", MessageId = "origName")]
		protected MemberMapper GetComplexMapper(string mapName, string origName)
		{
			if (origName == null) throw new ArgumentNullException("origName");

			string name = origName.ToLower();
			int    idx  = origName.IndexOf('.');

			if (idx > 0)
			{
				name = name.Substring(0, idx);

				foreach (MemberAccessor ma in TypeAccessor)
				{
					if (ma.Name.Length == name.Length && ma.Name.ToLower() == name)
					{
						ObjectMapper om = MappingSchema.GetObjectMapper(ma.Type);

						if (om != null)
						{
							MemberMapper mm = om.GetComplexMapper(mapName, origName.Substring(idx + 1));

							if (mm != null)
							{
								MapMemberInfo mi = new MapMemberInfo();

								mi.MemberAccessor        = ma;
								mi.ComplexMemberAccessor = mm.ComplexMemberAccessor;
								mi.Type                  = mm.Type;
								mi.MappingSchema         = MappingSchema;
								mi.Name                  = mapName;
								mi.MemberName            = origName;

								MemberMapper mapper = new MemberMapper.ComplexMapper(mm);

								mapper.Init(mi);

								return mapper;
							}
						}

						break;
					}
				}
			}
			else
			{
				MemberMapper mm = (MemberMapper)_nameToComplexMapper[name];

				if (mm != null)
					return mm;

				if (_nameToNotFoundComplexMappers.ContainsKey(name))
					return null;

				foreach (MemberMapper m in _members)
					if (m.MemberAccessor.Name.Length == name.Length && m.MemberAccessor.Name.ToLower() == name)
					{
						_nameToComplexMapper[name] = m;
						return m;
					}

				// Under some conditions, this way lead to memory leaks.
				// In other hand, shaking mappers up every time lead to performance loss.
				// So we cache failed requests.
				// If this optimization is a memory leak for you, just comment out next line.
				//
				_nameToNotFoundComplexMappers.Add(name, null);
			}

			return null;
		}

		private MapValue[] GetMapValues(MemberAccessor member)
		{
			bool isSet;

			MapValue[] values = MetadataProvider.GetMapValues(this, member, out isSet);

			return isSet? values: _mappingSchema.GetMapValues(member.Type);
		}

		protected virtual object GetDefaultValue(MemberAccessor memberAccessor)
		{
			bool isSet;

			object value = MetadataProvider.GetDefaultValue(this, memberAccessor, out isSet);

			return isSet? value: _mappingSchema.GetDefaultValue(memberAccessor.Type);
		}

		protected virtual bool GetNullable(MemberAccessor memberAccessor)
		{
			bool isSet;
			return MetadataProvider.GetNullable(this, memberAccessor, out isSet);
		}

		protected virtual bool GetIgnore(MemberAccessor memberAccessor)
		{
			bool isSet;
			return MetadataProvider.GetIgnore(this, memberAccessor, out isSet);
		}

		protected virtual string GetFieldName(MemberAccessor memberAccessor)
		{
			bool isSet;
			return MetadataProvider.GetFieldName(this, memberAccessor, out isSet);
		}

		protected virtual bool GetTrimmable(MemberAccessor memberAccessor)
		{
			bool isSet;
			return MetadataProvider.GetTrimmable(this, memberAccessor, out isSet);
		}

		protected virtual object GetNullValue(MemberAccessor memberAccessor, bool isNullable)
		{
			if (isNullable)
			{
				bool isSet;
				return MetadataProvider.GetNullValue(this, memberAccessor, out isSet);
			}

			return MappingSchema.GetNullValue(memberAccessor.Type);
		}

		#endregion

		#region IObjectMappper Members

		public virtual object CreateInstance()
		{
			return _typeAccessor.CreateInstanceEx();
		}

		public virtual object CreateInstance(InitContext context)
		{
			return _typeAccessor.CreateInstanceEx(context);
		}

		#endregion

		#region IMapDataSource Members

		public override int Count
		{
			get { return _members.Count; }
		}

		public override Type GetFieldType(int index)
		{
			return ((MemberMapper)_members[index]).Type;
		}

		public override string GetName(int index)
		{
			return ((MemberMapper)_members[index]).Name;
		}

		public override object GetValue(object o, int index)
		{
			return ((MemberMapper)_members[index]).GetValue(o);
		}

		public override object GetValue(object o, string name)
		{
			MemberMapper mm = (MemberMapper)_nameToMember[name];

			if (mm == null)
				mm = this[name];

			return mm == null? null: mm.GetValue(o);
		}

		public override bool     IsNull     (object o, int index) { return this[index].IsNull(o);      }

		public override bool     SupportsTypedValues(int index)   { return this[index].SupportsValue;  }

		// Simple type getters.
		//
		[CLSCompliant(false)]
		public override SByte    GetSByte   (object o, int index) { return this[index].GetSByte   (o); }
		public override Int16    GetInt16   (object o, int index) { return this[index].GetInt16   (o); }
		public override Int32    GetInt32   (object o, int index) { return this[index].GetInt32   (o); }
		public override Int64    GetInt64   (object o, int index) { return this[index].GetInt64   (o); }

		public override Byte     GetByte    (object o, int index) { return this[index].GetByte    (o); }
		[CLSCompliant(false)]
		public override UInt16   GetUInt16  (object o, int index) { return this[index].GetUInt16  (o); }
		[CLSCompliant(false)]
		public override UInt32   GetUInt32  (object o, int index) { return this[index].GetUInt32  (o); }
		[CLSCompliant(false)]
		public override UInt64   GetUInt64  (object o, int index) { return this[index].GetUInt64  (o); }

		public override Boolean  GetBoolean (object o, int index) { return this[index].GetBoolean (o); }
		public override Char     GetChar    (object o, int index) { return this[index].GetChar    (o); }
		public override Single   GetSingle  (object o, int index) { return this[index].GetSingle  (o); }
		public override Double   GetDouble  (object o, int index) { return this[index].GetDouble  (o); }
		public override Decimal  GetDecimal (object o, int index) { return this[index].GetDecimal (o); }
		public override Guid     GetGuid    (object o, int index) { return this[index].GetGuid    (o); }
		public override DateTime GetDateTime(object o, int index) { return this[index].GetDateTime(o); }

#if FW2
		// Nullable type getters.
		//
		[CLSCompliant(false)]
		public override SByte?    GetNullableSByte   (object o, int index) { return this[index].GetNullableSByte   (o); }
		public override Int16?    GetNullableInt16   (object o, int index) { return this[index].GetNullableInt16   (o); }
		public override Int32?    GetNullableInt32   (object o, int index) { return this[index].GetNullableInt32   (o); }
		public override Int64?    GetNullableInt64   (object o, int index) { return this[index].GetNullableInt64   (o); }

		public override Byte?     GetNullableByte    (object o, int index) { return this[index].GetNullableByte    (o); }
		[CLSCompliant(false)]
		public override UInt16?   GetNullableUInt16  (object o, int index) { return this[index].GetNullableUInt16  (o); }
		[CLSCompliant(false)]
		public override UInt32?   GetNullableUInt32  (object o, int index) { return this[index].GetNullableUInt32  (o); }
		[CLSCompliant(false)]
		public override UInt64?   GetNullableUInt64  (object o, int index) { return this[index].GetNullableUInt64  (o); }

		public override Boolean?  GetNullableBoolean (object o, int index) { return this[index].GetNullableBoolean (o); }
		public override Char?     GetNullableChar    (object o, int index) { return this[index].GetNullableChar    (o); }
		public override Single?   GetNullableSingle  (object o, int index) { return this[index].GetNullableSingle  (o); }
		public override Double?   GetNullableDouble  (object o, int index) { return this[index].GetNullableDouble  (o); }
		public override Decimal?  GetNullableDecimal (object o, int index) { return this[index].GetNullableDecimal (o); }
		public override Guid?     GetNullableGuid    (object o, int index) { return this[index].GetNullableGuid    (o); }
		public override DateTime? GetNullableDateTime(object o, int index) { return this[index].GetNullableDateTime(o); }
#endif
		// SQL type getters.
		//
		public override SqlByte     GetSqlByte    (object o, int index) { return this[index].GetSqlByte    (o); }
		public override SqlInt16    GetSqlInt16   (object o, int index) { return this[index].GetSqlInt16   (o); }
		public override SqlInt32    GetSqlInt32   (object o, int index) { return this[index].GetSqlInt32   (o); }
		public override SqlInt64    GetSqlInt64   (object o, int index) { return this[index].GetSqlInt64   (o); }
		public override SqlSingle   GetSqlSingle  (object o, int index) { return this[index].GetSqlSingle  (o); }
		public override SqlBoolean  GetSqlBoolean (object o, int index) { return this[index].GetSqlBoolean (o); }
		public override SqlDouble   GetSqlDouble  (object o, int index) { return this[index].GetSqlDouble  (o); }
		public override SqlDateTime GetSqlDateTime(object o, int index) { return this[index].GetSqlDateTime(o); }
		public override SqlDecimal  GetSqlDecimal (object o, int index) { return this[index].GetSqlDecimal (o); }
		public override SqlMoney    GetSqlMoney   (object o, int index) { return this[index].GetSqlMoney   (o); }
		public override SqlGuid     GetSqlGuid    (object o, int index) { return this[index].GetSqlGuid    (o); }
		public override SqlString   GetSqlString  (object o, int index) { return this[index].GetSqlString  (o); }

		#endregion

		#region IMapDataDestination Members

		public override int GetOrdinal(string name)
		{
			MemberMapper mm = (MemberMapper)_nameToMember[name];

			if (mm == null)
				mm = this[name];

			return mm == null? -1: mm.Ordinal;
		}

		public override void SetValue(object o, int index, object value)
		{
			((MemberMapper)_members[index]).SetValue(o, value);
		}

		public override void SetValue(object o, string name, object value)
		{
			SetValue(o, GetOrdinal(name), value);
		}

		public override void SetNull    (object o, int index)                 { this[index].SetNull    (o); }

		// Simple types setters.
		//
		[CLSCompliant(false)]
		public override void SetSByte   (object o, int index, SByte    value) { this[index].SetSByte   (o, value); }
		public override void SetInt16   (object o, int index, Int16    value) { this[index].SetInt16   (o, value); }
		public override void SetInt32   (object o, int index, Int32    value) { this[index].SetInt32   (o, value); }
		public override void SetInt64   (object o, int index, Int64    value) { this[index].SetInt64   (o, value); }

		public override void SetByte    (object o, int index, Byte     value) { this[index].SetByte    (o, value); }
		[CLSCompliant(false)]
		public override void SetUInt16  (object o, int index, UInt16   value) { this[index].SetUInt16  (o, value); }
		[CLSCompliant(false)]
		public override void SetUInt32  (object o, int index, UInt32   value) { this[index].SetUInt32  (o, value); }
		[CLSCompliant(false)]
		public override void SetUInt64  (object o, int index, UInt64   value) { this[index].SetUInt64  (o, value); }

		public override void SetBoolean (object o, int index, Boolean  value) { this[index].SetBoolean (o, value); }
		public override void SetChar    (object o, int index, Char     value) { this[index].SetChar    (o, value); }
		public override void SetSingle  (object o, int index, Single   value) { this[index].SetSingle  (o, value); }
		public override void SetDouble  (object o, int index, Double   value) { this[index].SetDouble  (o, value); }
		public override void SetDecimal (object o, int index, Decimal  value) { this[index].SetDecimal (o, value); }
		public override void SetGuid    (object o, int index, Guid     value) { this[index].SetGuid    (o, value); }
		public override void SetDateTime(object o, int index, DateTime value) { this[index].SetDateTime(o, value); }

#if FW2
		// Simple types setters.
		//
		[CLSCompliant(false)]
		public override void SetNullableSByte   (object o, int index, SByte?    value) { this[index].SetNullableSByte   (o, value); }
		public override void SetNullableInt16   (object o, int index, Int16?    value) { this[index].SetNullableInt16   (o, value); }
		public override void SetNullableInt32   (object o, int index, Int32?    value) { this[index].SetNullableInt32   (o, value); }
		public override void SetNullableInt64   (object o, int index, Int64?    value) { this[index].SetNullableInt64   (o, value); }

		public override void SetNullableByte    (object o, int index, Byte?     value) { this[index].SetNullableByte    (o, value); }
		[CLSCompliant(false)]
		public override void SetNullableUInt16  (object o, int index, UInt16?   value) { this[index].SetNullableUInt16  (o, value); }
		[CLSCompliant(false)]
		public override void SetNullableUInt32  (object o, int index, UInt32?   value) { this[index].SetNullableUInt32  (o, value); }
		[CLSCompliant(false)]
		public override void SetNullableUInt64  (object o, int index, UInt64?   value) { this[index].SetNullableUInt64  (o, value); }

		public override void SetNullableBoolean (object o, int index, Boolean?  value) { this[index].SetNullableBoolean (o, value); }
		public override void SetNullableChar    (object o, int index, Char?     value) { this[index].SetNullableChar    (o, value); }
		public override void SetNullableSingle  (object o, int index, Single?   value) { this[index].SetNullableSingle  (o, value); }
		public override void SetNullableDouble  (object o, int index, Double?   value) { this[index].SetNullableDouble  (o, value); }
		public override void SetNullableDecimal (object o, int index, Decimal?  value) { this[index].SetNullableDecimal (o, value); }
		public override void SetNullableGuid    (object o, int index, Guid?     value) { this[index].SetNullableGuid    (o, value); }
		public override void SetNullableDateTime(object o, int index, DateTime? value) { this[index].SetNullableDateTime(o, value); }
#endif

		// SQL type setters.
		//
		public override void SetSqlByte    (object o, int index, SqlByte     value) { this[index].SetSqlByte    (o, value); }
		public override void SetSqlInt16   (object o, int index, SqlInt16    value) { this[index].SetSqlInt16   (o, value); }
		public override void SetSqlInt32   (object o, int index, SqlInt32    value) { this[index].SetSqlInt32   (o, value); }
		public override void SetSqlInt64   (object o, int index, SqlInt64    value) { this[index].SetSqlInt64   (o, value); }
		public override void SetSqlSingle  (object o, int index, SqlSingle   value) { this[index].SetSqlSingle  (o, value); }
		public override void SetSqlBoolean (object o, int index, SqlBoolean  value) { this[index].SetSqlBoolean (o, value); }
		public override void SetSqlDouble  (object o, int index, SqlDouble   value) { this[index].SetSqlDouble  (o, value); }
		public override void SetSqlDateTime(object o, int index, SqlDateTime value) { this[index].SetSqlDateTime(o, value); }
		public override void SetSqlDecimal (object o, int index, SqlDecimal  value) { this[index].SetSqlDecimal (o, value); }
		public override void SetSqlMoney   (object o, int index, SqlMoney    value) { this[index].SetSqlMoney   (o, value); }
		public override void SetSqlGuid    (object o, int index, SqlGuid     value) { this[index].SetSqlGuid    (o, value); }
		public override void SetSqlString  (object o, int index, SqlString   value) { this[index].SetSqlString  (o, value); }

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return _members.GetEnumerator();
		}

		#endregion
	}
}
