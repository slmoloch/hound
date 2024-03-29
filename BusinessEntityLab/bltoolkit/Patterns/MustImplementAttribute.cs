using System;

namespace BLToolkit.Patterns
{
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Property)]
	public sealed class MustImplementAttribute : Attribute
	{
		public MustImplementAttribute(bool implement, bool throwException, string exceptionMessage)
		{
			_implement        = implement;
			_throwException   = throwException;
			_exceptionMessage = exceptionMessage;
		}

		public MustImplementAttribute(bool implement, bool throwException)
			: this(implement, throwException, null)
		{
		}

		public MustImplementAttribute(bool implement, string exceptionMessage)
			: this(implement, true, exceptionMessage)
		{
		}

		public MustImplementAttribute(bool implement)
			: this(implement, true, null)
		{
		}

		public MustImplementAttribute()
			: this(true, true, null)
		{
		}

		private readonly bool _implement;
		public           bool  Implement
		{
			get { return _implement;  }
		}

		private bool _throwException;
		public  bool  ThrowException
		{
			get { return _throwException;  }
			set { _throwException = value; }
		}

		private string _exceptionMessage;
		public  string  ExceptionMessage
		{
			get { return _exceptionMessage;  }
			set { _exceptionMessage = value; }
		}

		// Default is optional method which throws NotImplementedException at run tune.
		//
		public static readonly MustImplementAttribute Default = new MustImplementAttribute(false, true, null);
	}
}
