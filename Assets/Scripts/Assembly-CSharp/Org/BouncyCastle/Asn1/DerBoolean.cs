using System;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1
{
	public class DerBoolean : Asn1Object
	{
		private readonly byte value;

		public static readonly DerBoolean False = new DerBoolean(false);

		public static readonly DerBoolean True = new DerBoolean(true);

		public bool IsTrue
		{
			get
			{
				return value != 0;
			}
		}

		public DerBoolean(byte[] val)
		{
			if (val.Length != 1)
			{
				throw new ArgumentException("byte value should have 1 byte in it", "val");
			}
			value = val[0];
		}

		private DerBoolean(bool value)
		{
			this.value = (byte)(value ? byte.MaxValue : 0);
		}

		public static DerBoolean GetInstance(object obj)
		{
			if (obj == null || obj is DerBoolean)
			{
				return (DerBoolean)obj;
			}
			throw new ArgumentException("illegal object in GetInstance: " + Platform.GetTypeName(obj));
		}

		public static DerBoolean GetInstance(bool value)
		{
			return (!value) ? False : True;
		}

		public static DerBoolean GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			Asn1Object asn1Object = obj.GetObject();
			if (isExplicit || asn1Object is DerBoolean)
			{
				return GetInstance(asn1Object);
			}
			return FromOctetString(((Asn1OctetString)asn1Object).GetOctets());
		}

		internal override void Encode(DerOutputStream derOut)
		{
			derOut.WriteEncoded(1, new byte[1] { value });
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerBoolean derBoolean = asn1Object as DerBoolean;
			if (derBoolean == null)
			{
				return false;
			}
			return IsTrue == derBoolean.IsTrue;
		}

		protected override int Asn1GetHashCode()
		{
			return IsTrue.GetHashCode();
		}

		public override string ToString()
		{
			return (!IsTrue) ? "FALSE" : "TRUE";
		}

		internal static DerBoolean FromOctetString(byte[] value)
		{
			if (value.Length != 1)
			{
				throw new ArgumentException("BOOLEAN value should have 1 byte in it", "value");
			}
			DerBoolean result;
			switch (value[0])
			{
			case 0:
				result = False;
				break;
			case byte.MaxValue:
				result = True;
				break;
			default:
				result = new DerBoolean(value);
				break;
			}
			return result;
		}
	}
}
