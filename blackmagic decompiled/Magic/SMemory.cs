using System;
using System.Runtime.InteropServices;

namespace Magic;

public static class SMemory
{
	public const byte ASCII_CHAR_LENGTH = 1;

	public const byte UNICODE_CHAR_LENGTH = 2;

	public const int DEFAULT_MEMORY_SIZE = 4096;

	public static int ReadRawMemory(IntPtr hProcess, uint dwAddress, IntPtr lpBuffer, int nSize)
	{
		int lpBytesRead = 0;
		try
		{
			if (!Imports.ReadProcessMemory(hProcess, dwAddress, lpBuffer, nSize, out lpBytesRead))
			{
				throw new Exception("ReadProcessMemory failed");
			}
			return lpBytesRead;
		}
		catch
		{
			return 0;
		}
	}

	public static byte[] ReadBytes(IntPtr hProcess, uint dwAddress, int nSize)
	{
		IntPtr intPtr = IntPtr.Zero;
		byte[] array;
		try
		{
			intPtr = Marshal.AllocHGlobal(nSize);
			int num = ReadRawMemory(hProcess, dwAddress, intPtr, nSize);
			if (num != nSize)
			{
				throw new Exception("ReadProcessMemory error in ReadBytes");
			}
			array = new byte[num];
			Marshal.Copy(intPtr, array, 0, num);
		}
		catch
		{
			return null;
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}
		return array;
	}

	public static object ReadObject(IntPtr hProcess, uint dwAddress, Type objType)
	{
		IntPtr intPtr = IntPtr.Zero;
		object result;
		try
		{
			int num = Marshal.SizeOf(objType);
			intPtr = Marshal.AllocHGlobal(num);
			int num2 = ReadRawMemory(hProcess, dwAddress, intPtr, num);
			if (num2 != num)
			{
				throw new Exception("ReadProcessMemory error in ReadObject.");
			}
			result = Marshal.PtrToStructure(intPtr, objType);
		}
		catch
		{
			return null;
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}
		return result;
	}

	public static byte ReadByte(IntPtr hProcess, uint dwAddress)
	{
		byte[] array = ReadBytes(hProcess, dwAddress, 1);
		if (array == null)
		{
			throw new Exception("ReadByte failed.");
		}
		return array[0];
	}

	public static sbyte ReadSByte(IntPtr hProcess, uint dwAddress)
	{
		byte[] array = ReadBytes(hProcess, dwAddress, 1);
		if (array == null)
		{
			throw new Exception("ReadSByte failed.");
		}
		return (sbyte)array[0];
	}

	public static ushort ReadUShort(IntPtr hProcess, uint dwAddress, bool bReverse)
	{
		byte[] array = ReadBytes(hProcess, dwAddress, 2);
		if (array == null)
		{
			throw new Exception("ReadUShort failed.");
		}
		if (bReverse)
		{
			Array.Reverse(array);
		}
		return BitConverter.ToUInt16(array, 0);
	}

	public static ushort ReadUShort(IntPtr hProcess, uint dwAddress)
	{
		return ReadUShort(hProcess, dwAddress, bReverse: false);
	}

	public static short ReadShort(IntPtr hProcess, uint dwAddress, bool bReverse)
	{
		byte[] array = ReadBytes(hProcess, dwAddress, 2);
		if (array == null)
		{
			throw new Exception("ReadShort failed.");
		}
		if (bReverse)
		{
			Array.Reverse(array);
		}
		return BitConverter.ToInt16(array, 0);
	}

	public static short ReadShort(IntPtr hProcess, uint dwAddress)
	{
		return ReadShort(hProcess, dwAddress, bReverse: false);
	}

	public static uint ReadUInt(IntPtr hProcess, uint dwAddress, bool bReverse)
	{
		byte[] array = ReadBytes(hProcess, dwAddress, 4);
		if (array == null)
		{
			throw new Exception("ReadUInt failed.");
		}
		if (bReverse)
		{
			Array.Reverse(array);
		}
		return BitConverter.ToUInt32(array, 0);
	}

	public static uint ReadUInt(IntPtr hProcess, uint dwAddress)
	{
		return ReadUInt(hProcess, dwAddress, bReverse: false);
	}

	public static int ReadInt(IntPtr hProcess, uint dwAddress, bool bReverse)
	{
		byte[] array = ReadBytes(hProcess, dwAddress, 4);
		if (array == null)
		{
			throw new Exception("ReadInt failed.");
		}
		if (bReverse)
		{
			Array.Reverse(array);
		}
		return BitConverter.ToInt32(array, 0);
	}

	public static int ReadInt(IntPtr hProcess, uint dwAddress)
	{
		return ReadInt(hProcess, dwAddress, bReverse: false);
	}

	public static ulong ReadUInt64(IntPtr hProcess, uint dwAddress, bool bReverse)
	{
		byte[] array = ReadBytes(hProcess, dwAddress, 8);
		if (array == null)
		{
			throw new Exception("ReadUInt64 failed.");
		}
		if (bReverse)
		{
			Array.Reverse(array);
		}
		return BitConverter.ToUInt64(array, 0);
	}

	public static ulong ReadUInt64(IntPtr hProcess, uint dwAddress)
	{
		return ReadUInt64(hProcess, dwAddress, bReverse: false);
	}

	public static long ReadInt64(IntPtr hProcess, uint dwAddress, bool bReverse)
	{
		byte[] array = ReadBytes(hProcess, dwAddress, 8);
		if (array == null)
		{
			throw new Exception("ReadInt64 failed.");
		}
		if (bReverse)
		{
			Array.Reverse(array);
		}
		return BitConverter.ToInt64(array, 0);
	}

	public static long ReadInt64(IntPtr hProcess, uint dwAddress)
	{
		return ReadInt64(hProcess, dwAddress, bReverse: false);
	}

	public static float ReadFloat(IntPtr hProcess, uint dwAddress, bool bReverse)
	{
		byte[] array = ReadBytes(hProcess, dwAddress, 4);
		if (array == null)
		{
			throw new Exception("ReadFloat failed.");
		}
		if (bReverse)
		{
			Array.Reverse(array);
		}
		return BitConverter.ToSingle(array, 0);
	}

	public static float ReadFloat(IntPtr hProcess, uint dwAddress)
	{
		return ReadFloat(hProcess, dwAddress, bReverse: false);
	}

	public static double ReadDouble(IntPtr hProcess, uint dwAddress, bool bReverse)
	{
		byte[] array = ReadBytes(hProcess, dwAddress, 8);
		if (array == null)
		{
			throw new Exception("ReadDouble failed.");
		}
		if (bReverse)
		{
			Array.Reverse(array);
		}
		return BitConverter.ToDouble(array, 0);
	}

	public static double ReadDouble(IntPtr hProcess, uint dwAddress)
	{
		return ReadDouble(hProcess, dwAddress, bReverse: false);
	}

	public static string ReadASCIIString(IntPtr hProcess, uint dwAddress, int nLength)
	{
		IntPtr intPtr = IntPtr.Zero;
		string result;
		try
		{
			intPtr = Marshal.AllocHGlobal(nLength + 1);
			Marshal.WriteByte(intPtr, nLength, 0);
			int num = ReadRawMemory(hProcess, dwAddress, intPtr, nLength);
			if (num != nLength)
			{
				throw new Exception();
			}
			result = Marshal.PtrToStringAnsi(intPtr);
		}
		catch
		{
			return string.Empty;
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}
		return result;
	}

	public static string ReadUnicodeString(IntPtr hProcess, uint dwAddress, int nLength)
	{
		IntPtr intPtr = IntPtr.Zero;
		string result;
		try
		{
			int num = nLength * 2;
			intPtr = Marshal.AllocHGlobal(num + 2);
			Marshal.WriteInt16(intPtr, nLength * 2, 0);
			int num2 = ReadRawMemory(hProcess, dwAddress, intPtr, num);
			if (num2 != num)
			{
				throw new Exception();
			}
			result = Marshal.PtrToStringUni(intPtr);
		}
		catch
		{
			return string.Empty;
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}
		return result;
	}

	private static int WriteRawMemory(IntPtr hProcess, uint dwAddress, IntPtr lpBuffer, int nSize)
	{
		IntPtr iBytesWritten = IntPtr.Zero;
		if (!Imports.WriteProcessMemory(hProcess, dwAddress, lpBuffer, nSize, out iBytesWritten))
		{
			return 0;
		}
		return (int)iBytesWritten;
	}

	public static bool WriteBytes(IntPtr hProcess, uint dwAddress, byte[] lpBytes, int nSize)
	{
		IntPtr intPtr = IntPtr.Zero;
		int num = 0;
		try
		{
			intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(lpBytes[0]) * nSize);
			Marshal.Copy(lpBytes, 0, intPtr, nSize);
			num = WriteRawMemory(hProcess, dwAddress, intPtr, nSize);
			if (nSize != num)
			{
				throw new Exception("WriteBytes failed!  Number of bytes actually written differed from request.");
			}
		}
		catch
		{
			return false;
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}
		return true;
	}

	public static bool WriteBytes(IntPtr hProcess, uint dwAddress, byte[] lpBytes)
	{
		return WriteBytes(hProcess, dwAddress, lpBytes, lpBytes.Length);
	}

	public static bool WriteObject(IntPtr hProcess, uint dwAddress, object objBuffer, Type objType)
	{
		int num = 0;
		int num2 = 0;
		IntPtr intPtr = IntPtr.Zero;
		try
		{
			num = Marshal.SizeOf(objType);
			intPtr = Marshal.AllocHGlobal(num);
			Marshal.StructureToPtr(objBuffer, intPtr, fDeleteOld: false);
			num2 = WriteRawMemory(hProcess, dwAddress, intPtr, num);
			if (num != num2)
			{
				throw new Exception("WriteObject failed!  Number of bytes actually written differed from request.");
			}
		}
		catch
		{
			return false;
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				Marshal.DestroyStructure(intPtr, objType);
				Marshal.FreeHGlobal(intPtr);
			}
		}
		return true;
	}

	public static bool WriteObject(IntPtr hProcess, uint dwAddress, object objBuffer)
	{
		return WriteObject(hProcess, dwAddress, objBuffer, objBuffer.GetType());
	}

	public static bool WriteByte(IntPtr hProcess, uint dwAddress, byte Value)
	{
		byte[] lpBytes = new byte[1] { Value };
		return WriteBytes(hProcess, dwAddress, lpBytes, 1);
	}

	public static bool WriteSByte(IntPtr hProcess, uint dwAddress, sbyte Value)
	{
		byte[] lpBytes = new byte[1] { (byte)Value };
		return WriteBytes(hProcess, dwAddress, lpBytes, 1);
	}

	public static bool WriteUShort(IntPtr hProcess, uint dwAddress, ushort Value)
	{
		byte[] bytes = BitConverter.GetBytes(Value);
		return WriteBytes(hProcess, dwAddress, bytes, 2);
	}

	public static bool WriteShort(IntPtr hProcess, uint dwAddress, short Value)
	{
		byte[] bytes = BitConverter.GetBytes(Value);
		return WriteBytes(hProcess, dwAddress, bytes, 2);
	}

	public static bool WriteUInt(IntPtr hProcess, uint dwAddress, uint Value)
	{
		byte[] bytes = BitConverter.GetBytes(Value);
		return WriteBytes(hProcess, dwAddress, bytes, 4);
	}

	public static bool WriteInt(IntPtr hProcess, uint dwAddress, int Value)
	{
		byte[] bytes = BitConverter.GetBytes(Value);
		return WriteBytes(hProcess, dwAddress, bytes, 4);
	}

	public static bool WriteUInt64(IntPtr hProcess, uint dwAddress, ulong Value)
	{
		byte[] bytes = BitConverter.GetBytes(Value);
		return WriteBytes(hProcess, dwAddress, bytes, 8);
	}

	public static bool WriteInt64(IntPtr hProcess, uint dwAddress, long Value)
	{
		byte[] bytes = BitConverter.GetBytes(Value);
		return WriteBytes(hProcess, dwAddress, bytes, 8);
	}

	public static bool WriteFloat(IntPtr hProcess, uint dwAddress, float Value)
	{
		byte[] bytes = BitConverter.GetBytes(Value);
		return WriteBytes(hProcess, dwAddress, bytes, 4);
	}

	public static bool WriteDouble(IntPtr hProcess, uint dwAddress, double Value)
	{
		byte[] bytes = BitConverter.GetBytes(Value);
		return WriteBytes(hProcess, dwAddress, bytes, 8);
	}

	public static bool WriteASCIIString(IntPtr hProcess, uint dwAddress, string Value)
	{
		IntPtr intPtr = IntPtr.Zero;
		int num = 0;
		int num2 = 0;
		try
		{
			num2 = Value.Length;
			intPtr = Marshal.StringToHGlobalAnsi(Value);
			num = WriteRawMemory(hProcess, dwAddress, intPtr, num2);
			if (num2 != num)
			{
				throw new Exception();
			}
		}
		catch
		{
			return false;
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}
		return true;
	}

	public static bool WriteUnicodeString(IntPtr hProcess, uint dwAddress, string Value)
	{
		IntPtr intPtr = IntPtr.Zero;
		int num = 0;
		int num2 = 0;
		try
		{
			num2 = Value.Length * 2;
			intPtr = Marshal.StringToHGlobalUni(Value);
			num = WriteRawMemory(hProcess, dwAddress, intPtr, num2);
			if (num2 != num)
			{
				throw new Exception();
			}
		}
		catch
		{
			return false;
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}
		return true;
	}

	public static uint AllocateMemory(IntPtr hProcess, int nSize, uint dwAllocationType, uint dwProtect)
	{
		return Imports.VirtualAllocEx(hProcess, 0u, nSize, dwAllocationType, dwProtect);
	}

	public static uint AllocateMemory(IntPtr hProcess, int nSize)
	{
		return AllocateMemory(hProcess, nSize, 4096u, 64u);
	}

	public static uint AllocateMemory(IntPtr hProcess)
	{
		return AllocateMemory(hProcess, 4096);
	}

	public static bool FreeMemory(IntPtr hProcess, uint dwAddress, int nSize, uint dwFreeType)
	{
		if (dwFreeType == 32768)
		{
			nSize = 0;
		}
		return Imports.VirtualFreeEx(hProcess, dwAddress, nSize, dwFreeType);
	}

	public static bool FreeMemory(IntPtr hProcess, uint dwAddress)
	{
		return FreeMemory(hProcess, dwAddress, 0, 32768u);
	}
}
