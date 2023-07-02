using System;
using System.Diagnostics;

namespace Magic;

public static class SPattern
{
	public static uint FindPattern(IntPtr hProcess, ProcessModule pMod, string szPattern, string szMask, params char[] Delimiter)
	{
		return FindPattern(hProcess, (uint)(int)pMod.BaseAddress, pMod.ModuleMemorySize, szPattern, szMask, Delimiter);
	}

	public static uint FindPattern(IntPtr hProcess, ProcessModule[] pMods, string szPattern, string szMask, params char[] Delimiter)
	{
		return FindPattern(hProcess, new ProcessModuleCollection(pMods), szPattern, szMask, Delimiter);
	}

	public static uint FindPattern(IntPtr hProcess, ProcessModuleCollection pMods, string szPattern, string szMask, params char[] Delimiter)
	{
		uint result = 0u;
		foreach (ProcessModule pMod in pMods)
		{
			if ((result = FindPattern(hProcess, pMod, szPattern, szMask, Delimiter)) != 0)
			{
				break;
			}
		}
		return result;
	}

	public static uint FindPattern(IntPtr hProcess, uint dwStart, int nSize, string szPattern, string szMask, params char[] Delimiter)
	{
		if (Delimiter == null)
		{
			Delimiter = new char[1] { ' ' };
		}
		string[] array = szPattern.Split(Delimiter);
		byte[] array2 = new byte[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = Convert.ToByte(array[i], 16);
		}
		return FindPattern(hProcess, dwStart, nSize, array2, szMask);
	}

	public static uint FindPattern(IntPtr hProcess, ProcessModule pMod, byte[] bPattern, string szMask)
	{
		return FindPattern(hProcess, (uint)(int)pMod.BaseAddress, pMod.ModuleMemorySize, bPattern, szMask);
	}

	public static uint FindPattern(IntPtr hProcess, ProcessModule[] pMods, byte[] bPattern, string szMask)
	{
		return FindPattern(hProcess, new ProcessModuleCollection(pMods), bPattern, szMask);
	}

	public static uint FindPattern(IntPtr hProcess, ProcessModuleCollection pMods, byte[] bPattern, string szMask)
	{
		uint result = 0u;
		foreach (ProcessModule pMod in pMods)
		{
			if ((result = FindPattern(hProcess, pMod, bPattern, szMask)) != 0)
			{
				break;
			}
		}
		return result;
	}

	public static uint FindPattern(IntPtr hProcess, uint dwStart, int nSize, byte[] bPattern, string szMask)
	{
		if (bPattern == null || bPattern.Length == 0)
		{
			throw new ArgumentNullException("bData");
		}
		if (bPattern.Length != szMask.Length)
		{
			throw new ArgumentException("bData and szMask must be of the same size");
		}
		byte[] array = SMemory.ReadBytes(hProcess, dwStart, nSize);
		if (array == null)
		{
			throw new Exception("Could not read memory in FindPattern.");
		}
		return dwStart + FindPattern(array, bPattern, szMask);
	}

	public static uint FindPattern(byte[] bData, byte[] bPattern, string szMask)
	{
		if (bData == null || bData.Length == 0)
		{
			throw new ArgumentNullException("bData");
		}
		if (bPattern == null || bPattern.Length == 0)
		{
			throw new ArgumentNullException("bPattern");
		}
		if (szMask == string.Empty)
		{
			throw new ArgumentNullException("szMask");
		}
		if (bPattern.Length != szMask.Length)
		{
			throw new ArgumentException("Pattern and Mask lengths must be the same.");
		}
		bool flag = false;
		int num = bPattern.Length;
		int num2 = bData.Length - num;
		for (int i = 0; i < num2; i++)
		{
			flag = true;
			for (int j = 0; j < num; j++)
			{
				if ((szMask[j] == 'x' && bPattern[j] != bData[i + j]) || (szMask[j] == '!' && bPattern[j] == bData[i + j]))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				return (uint)i;
			}
		}
		return 0u;
	}
}
