using System;
using System.Diagnostics;

namespace Magic;

public static class SProcess
{
	public static IntPtr OpenProcess(int dwProcessId, uint dwAccessRights)
	{
		return Imports.OpenProcess(dwAccessRights, bInheritHandle: false, dwProcessId);
	}

	public static IntPtr OpenProcess(int dwProcessId)
	{
		return OpenProcess(dwProcessId, 2035711u);
	}

	public static IntPtr OpenProcess(IntPtr hWnd, uint dwAccessRights)
	{
		return OpenProcess(GetProcessFromWindow(hWnd), dwAccessRights);
	}

	public static IntPtr OpenProcess(IntPtr hWnd)
	{
		return OpenProcess(GetProcessFromWindow(hWnd), 2035711u);
	}

	public static int GetProcessFromWindow(IntPtr hWnd)
	{
		int dwProcessId = 0;
		Imports.GetWindowThreadProcessId(hWnd, out dwProcessId);
		return dwProcessId;
	}

	public static int GetProcessFromWindowTitle(string WindowTitle)
	{
		IntPtr intPtr = SWindow.FindWindow(null, WindowTitle);
		if (intPtr == IntPtr.Zero)
		{
			return 0;
		}
		return GetProcessFromWindow(intPtr);
	}

	public static int[] GetProcessesFromWindowTitle(string WindowTitle)
	{
		IntPtr[] array = SWindow.FindWindows(null, WindowTitle);
		if (array == null || array.Length == 0)
		{
			return null;
		}
		int[] array2 = new int[array.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = GetProcessFromWindow(array[i]);
		}
		return array2;
	}

	public static int GetProcessFromClassname(string Classname)
	{
		IntPtr intPtr = SWindow.FindWindow(Classname, null);
		if (intPtr == IntPtr.Zero)
		{
			return 0;
		}
		return GetProcessFromWindow(intPtr);
	}

	public static int[] GetProcessesFromClassname(string Classname)
	{
		IntPtr[] array = SWindow.FindWindows(Classname, null);
		if (array == null || array.Length == 0)
		{
			return null;
		}
		int[] array2 = new int[array.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = GetProcessFromWindow(array[i]);
		}
		return array2;
	}

	public static int GetProcessFromProcessName(string ProcessName)
	{
		if (ProcessName.EndsWith(".exe"))
		{
			ProcessName = ProcessName.Remove(ProcessName.Length - 4, 4);
		}
		Process[] processesByName = Process.GetProcessesByName(ProcessName);
		if (processesByName == null || processesByName.Length == 0)
		{
			return 0;
		}
		return processesByName[0].Id;
	}
}
