using System;

using BLToolkit.Mapping.ValueMappingInternal;

#if FW2
using BLToolkit.Common;

using KeyValue = System.Collections.Generic.KeyValuePair<System.Type, System.Type>;
using Table    = System.Collections.Generic.Dictionary<System.Collections.Generic.KeyValuePair<System.Type, System.Type>, BLToolkit.Mapping.IValueMapper>;
#else
using KeyValue = BLToolkit.Common.CompoundValue;
using Table    = System.Collections.Hashtable;
#endif

namespace BLToolkit.Mapping
{
	public
#if FW2
		static
#endif
		class ValueMapping
	{
		#region Init

		private static readonly Table _mappers = new Table();

		#endregion

		#region Default Mapper

		class DefaultValueMapper : IValueMapper
		{
			public void Map(
				IMapDataSource      source, object sourceObject, int sourceIndex,
				IMapDataDestination dest,   object destObject,   int destIndex)
			{
				dest.SetValue(destObject, destIndex, source.GetValue(sourceObject, sourceIndex));

				//object o = source.GetValue(sourceObject, sourceIndex);

				//if (o == null) dest.SetNull (destObject, destIndex);
				//else           dest.SetValue(destObject, destIndex, o);
			}
		}

		private static IValueMapper _defaultMapper = new DefaultValueMapper();
		[CLSCompliant(false)]
		public  static IValueMapper  DefaultMapper
		{
			get { return _defaultMapper;  }
			set { _defaultMapper = value; }
		}

		#endregion

		#region GetMapper

		private static readonly object _sync = new object();

		[CLSCompliant(false)]
		public static IValueMapper GetMapper(Type t1, Type t2)
		{
			if (t1 == null) t1 = typeof(object);
			if (t2 == null) t2 = typeof(object);

			if (t1.IsEnum) t1 = Enum.GetUnderlyingType(t1);
			if (t2.IsEnum) t2 = Enum.GetUnderlyingType(t2);

			KeyValue key = new KeyValue(t1, t2);

			lock (_sync)
			{
				IValueMapper t;
#if FW2
				if (_mappers.TryGetValue(key, out t))
					return t;

				t = MapperSelector.GetMapper(t1, t2);

				if (null == t)
				{
					Type type = typeof(GetSetDataChecker<,>).MakeGenericType(t1, t2);

					if (((IGetSetDataChecker)Activator.CreateInstance(type)).Check() == false)
					{
						t = _defaultMapper;
					}
					else
					{
						type = t1 == t2 ?
							typeof(ValueMapper<>).MakeGenericType(t1) :
							typeof(ValueMapper<,>).MakeGenericType(t1, t2);

						t = (IValueMapper)Activator.CreateInstance(type);
					}
				}
#else
				t = (IValueMapper)_mappers[key];

				if (t != null)
					return t;

				t = MapperSelector.GetMapper(t1, t2);

				if (null == t)
					t = _defaultMapper;
#endif
				_mappers.Add(key, t);

				return t;
			}
		}

		#endregion

#if FW2

		#region Generic Mappers

		interface IGetSetDataChecker
		{
			bool Check();
		}

		class GetSetDataChecker<S,D> : IGetSetDataChecker
		{
			public bool Check()
			{
				return
					!(MapGetData<S>.I is MapGetData<S>.Default<S>) &&
					!(MapSetData<S>.I is MapSetData<S>.Default<S>) &&
					!(MapGetData<D>.I is MapGetData<D>.Default<D>) &&
					!(MapSetData<D>.I is MapSetData<D>.Default<D>);
			}
		}

		class ValueMapper<T> : IValueMapper
		{
			public void Map(
				IMapDataSource      source, object sourceObject, int sourceIndex,
				IMapDataDestination dest,   object destObject,   int destIndex)
			{
				if (source.IsNull(sourceObject, sourceIndex))
					dest.SetNull(destObject, destIndex);
				else
				{
					MapSetData<T>.MB<T> setter    = MapSetData<T>.I;
					MapGetData<T>.MB<T> getter    = MapGetData<T>.I;

					setter.To(dest, destObject, destIndex,
						getter.From(source, sourceObject, sourceIndex));
				}
			}
		}

		class ValueMapper<S,D> : IValueMapper
		{
			public void Map(
				IMapDataSource      source, object sourceObject, int sourceIndex,
				IMapDataDestination dest,   object destObject,   int destIndex)
			{
				if (source.IsNull(sourceObject, sourceIndex))
					dest.SetNull(destObject, destIndex);
				else
				{
					MapSetData<D>.MB<D>        setter    = MapSetData<D>.I;
					MapGetData<S>.MB<S>        getter    = MapGetData<S>.I;
					Convert<D,S>.ConvertMethod converter = Convert<D,S>.From;

					setter.To(dest, destObject, destIndex,
						converter(getter.From(source, sourceObject, sourceIndex)));
				}
			}
		}

		#endregion

#endif
	}
}
