using System;
using System.Linq;

namespace Utils
{
	public static class BitConverter
	{
		public static bool IsBigEndian
		{
			get
			{
				return !System.BitConverter.IsLittleEndian;
			}
			set
			{
			}
		}

		public static long DoubleToInt64Bits(double value)
		{
			throw new NotImplementedException();
		}

		public static byte[] GetBytes(bool value)
		{
			if (IsBigEndian)
			{
				return System.BitConverter.GetBytes(value);
			}
			return System.BitConverter.GetBytes(value).Reverse().ToArray();
		}

		public static byte[] GetBytes(char value)
		{
			if (IsBigEndian)
			{
				return System.BitConverter.GetBytes(value);
			}
			return System.BitConverter.GetBytes(value).Reverse().ToArray();
		}

		public static byte[] GetBytes(double value)
		{
			if (IsBigEndian)
			{
				return System.BitConverter.GetBytes(value);
			}
			return System.BitConverter.GetBytes(value).Reverse().ToArray();
		}

		public static byte[] GetBytes(float value)
		{
			if (IsBigEndian)
			{
				return System.BitConverter.GetBytes(value);
			}
			return System.BitConverter.GetBytes(value).Reverse().ToArray();
		}

		public static byte[] GetBytes(int value)
		{
			if (IsBigEndian)
			{
				return System.BitConverter.GetBytes(value);
			}
			return System.BitConverter.GetBytes(value).Reverse().ToArray();
		}

		public static byte[] GetBytes(long value)
		{
			if (IsBigEndian)
			{
				return System.BitConverter.GetBytes(value);
			}
			return System.BitConverter.GetBytes(value).Reverse().ToArray();
		}

		public static byte[] GetBytes(short value)
		{
			if (IsBigEndian)
			{
				return System.BitConverter.GetBytes(value);
			}
			return System.BitConverter.GetBytes(value).Reverse().ToArray();
		}

		public static byte[] GetBytes(uint value)
		{
			if (IsBigEndian)
			{
				return System.BitConverter.GetBytes(value);
			}
			return System.BitConverter.GetBytes(value).Reverse().ToArray();
		}

		public static byte[] GetBytes(ulong value)
		{
			if (IsBigEndian)
			{
				return System.BitConverter.GetBytes(value);
			}
			return System.BitConverter.GetBytes(value).Reverse().ToArray();
		}

		public static byte[] GetBytes(ushort value)
		{
			if (IsBigEndian)
			{
				return System.BitConverter.GetBytes(value);
			}
			return System.BitConverter.GetBytes(value).Reverse().ToArray();
		}

		public static byte[] GetBytes(Guid guid)
		{
			byte[] array = guid.ToByteArray();
			if (IsBigEndian)
			{
				return array;
			}
			byte[] array2 = new byte[array.Length];
			Buffer.BlockCopy(array.SubArray(0, 4).Reverse().ToArray(), 0, array2, 0, 4);
			Buffer.BlockCopy(array.SubArray(4, 2).Reverse().ToArray(), 0, array2, 4, 2);
			Buffer.BlockCopy(array.SubArray(6, 2).Reverse().ToArray(), 0, array2, 6, 2);
			Buffer.BlockCopy(array.SubArray(8, 8), 0, array2, 8, 8);
			return array2;
		}

		public static double Int64BitsToDouble(long value)
		{
			throw new NotImplementedException();
		}

		public static bool ToBoolean(byte[] value, int startIndex)
		{
			throw new NotImplementedException();
		}

		public static char ToChar(byte[] value, int startIndex)
		{
			throw new NotImplementedException();
		}

		public static double ToDouble(byte[] value, int startIndex)
		{
			throw new NotImplementedException();
		}

		public static short ToInt16(byte[] value, int startIndex)
		{
			if (IsBigEndian)
			{
				return System.BitConverter.ToInt16(value, startIndex);
			}
			return System.BitConverter.ToInt16(value.Reverse().ToArray(), value.Length - 2 - startIndex);
		}

		public static int ToInt32(byte[] value, int startIndex)
		{
			if (IsBigEndian)
			{
				return System.BitConverter.ToInt32(value, startIndex);
			}
			return System.BitConverter.ToInt32(value.Reverse().ToArray(), value.Length - 4 - startIndex);
		}

		public static long ToInt64(byte[] value, int startIndex)
		{
			if (IsBigEndian)
			{
				return System.BitConverter.ToInt64(value, startIndex);
			}
			return System.BitConverter.ToInt64(value.Reverse().ToArray(), value.Length - 8 - startIndex);
		}

		public static float ToSingle(byte[] value, int startIndex)
		{
			if (IsBigEndian)
			{
				return System.BitConverter.ToSingle(value, startIndex);
			}
			return System.BitConverter.ToSingle(value.Reverse().ToArray(), value.Length - 4 - startIndex);
		}

		public static string ToString(byte[] value)
		{
			if (IsBigEndian)
			{
				return System.BitConverter.ToString(value);
			}
			return System.BitConverter.ToString(value.Reverse().ToArray());
		}

		public static string ToString(byte[] value, int startIndex)
		{
			if (IsBigEndian)
			{
				return System.BitConverter.ToString(value, startIndex);
			}
			return System.BitConverter.ToString(value.Reverse().ToArray(), startIndex);
		}

		public static string ToString(byte[] value, int startIndex, int length)
		{
			if (IsBigEndian)
			{
				return System.BitConverter.ToString(value, startIndex, length);
			}
			return System.BitConverter.ToString(value.Reverse().ToArray(), startIndex, length);
		}

		public static ushort ToUInt16(byte[] value, int startIndex)
		{
			if (IsBigEndian)
			{
				return System.BitConverter.ToUInt16(value, startIndex);
			}
			return System.BitConverter.ToUInt16(value.Reverse().ToArray(), value.Length - 2 - startIndex);
		}

		public static uint ToUInt32(byte[] value, int startIndex)
		{
			if (IsBigEndian)
			{
				return System.BitConverter.ToUInt32(value, startIndex);
			}
			return System.BitConverter.ToUInt32(value.Reverse().ToArray(), value.Length - 4 - startIndex);
		}

		public static ulong ToUInt64(byte[] value, int startIndex)
		{
			if (IsBigEndian)
			{
				return System.BitConverter.ToUInt64(value, startIndex);
			}
			return System.BitConverter.ToUInt64(value.Reverse().ToArray(), value.Length - 8 - startIndex);
		}

		public static Guid ToGuid(byte[] value, int startIndex)
		{
			if (IsBigEndian)
			{
				return new Guid(value.SubArray(startIndex, 16));
			}
			return new Guid(ToInt32(value, startIndex), ToInt16(value, startIndex + 4), ToInt16(value, startIndex + 6), value.SubArray(startIndex + 8, 8));
		}
	}
}
