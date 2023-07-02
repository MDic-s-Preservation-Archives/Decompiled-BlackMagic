using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Magic;

public static class SWindow
{
	private static object lWindowsLock = new object();

	private static List<IntPtr> lWindows;

	private static bool EnumWindowsCallback(IntPtr hWnd, IntPtr lParam)
	{
		lWindows.Add(hWnd);
		return true;
	}

	private static bool _EnumWindows()
	{
		lWindows = new List<IntPtr>();
		Imports.EnumWindowsProc lpEnumFunc = EnumWindowsCallback;
		return Imports.EnumWindows(lpEnumFunc, IntPtr.Zero);
	}

	public static IntPtr[] EnumWindows()
	{
		lock (lWindowsLock)
		{
			if (!_EnumWindows())
			{
				return null;
			}
			return lWindows.ToArray();
		}
	}

	public static IntPtr[] EnumMainWindows()
	{
		List<IntPtr> list = new List<IntPtr>();
		Process[] processes = Process.GetProcesses();
		Process[] array = processes;
		foreach (Process process in array)
		{
			list.Add(process.MainWindowHandle);
		}
		return list.ToArray();
	}

	public static IntPtr[] FindWindows(string Classname, string WindowTitle)
	{
		if (Classname == null)
		{
			Classname = string.Empty;
		}
		if (WindowTitle == null)
		{
			WindowTitle = string.Empty;
		}
		List<IntPtr> list = new List<IntPtr>();
		lock (lWindowsLock)
		{
			if (!_EnumWindows())
			{
				return null;
			}
			foreach (IntPtr lWindow in lWindows)
			{
				if ((WindowTitle.Length > 0 && Imports.GetWindowTitle(lWindow) == WindowTitle) || (Classname.Length > 0 && Imports.GetClassName(lWindow) == Classname))
				{
					list.Add(lWindow);
				}
			}
		}
		return list.ToArray();
	}

	public static IntPtr FindWindow(string Classname, string WindowTitle)
	{
		if (Classname == null)
		{
			Classname = string.Empty;
		}
		if (WindowTitle == null)
		{
			WindowTitle = string.Empty;
		}
		lock (lWindowsLock)
		{
			if (!_EnumWindows())
			{
				return IntPtr.Zero;
			}
			foreach (IntPtr lWindow in lWindows)
			{
				if ((WindowTitle.Length > 0 && Imports.GetWindowTitle(lWindow) == WindowTitle) || (Classname.Length > 0 && Imports.GetClassName(lWindow) == Classname))
				{
					return lWindow;
				}
			}
		}
		return IntPtr.Zero;
	}

	public static IntPtr[] FindMainWindows(string Classname, string WindowTitle)
	{
		if (Classname == null)
		{
			Classname = string.Empty;
		}
		if (WindowTitle == null)
		{
			WindowTitle = string.Empty;
		}
		List<IntPtr> list = new List<IntPtr>();
		Process[] processes = Process.GetProcesses();
		Process[] array = processes;
		foreach (Process process in array)
		{
			if (process.MainWindowHandle != IntPtr.Zero && ((WindowTitle.Length > 0 && process.MainWindowTitle == WindowTitle) || (Classname.Length > 0 && Imports.GetClassName(process.MainWindowHandle) == Classname)))
			{
				list.Add(process.MainWindowHandle);
			}
		}
		return list.ToArray();
	}

	public static IntPtr FindMainWindow(string Classname, string WindowTitle)
	{
		if (Classname == null)
		{
			Classname = string.Empty;
		}
		if (WindowTitle == null)
		{
			WindowTitle = string.Empty;
		}
		Process[] processes = Process.GetProcesses();
		Process[] array = processes;
		foreach (Process process in array)
		{
			if (process.MainWindowHandle != IntPtr.Zero && ((WindowTitle.Length > 0 && process.MainWindowTitle == WindowTitle) || (Classname.Length > 0 && Imports.GetClassName(process.MainWindowHandle) == Classname)))
			{
				return process.MainWindowHandle;
			}
		}
		return IntPtr.Zero;
	}

	public static IntPtr[] FindWindowsContains(string Classname, string WindowTitle)
	{
		if (Classname == null)
		{
			Classname = string.Empty;
		}
		if (WindowTitle == null)
		{
			WindowTitle = string.Empty;
		}
		List<IntPtr> list = new List<IntPtr>();
		lock (lWindowsLock)
		{
			if (!_EnumWindows())
			{
				return null;
			}
			foreach (IntPtr lWindow in lWindows)
			{
				if ((WindowTitle.Length > 0 && Imports.GetWindowTitle(lWindow).Contains(WindowTitle)) || (Classname.Length > 0 && Imports.GetClassName(lWindow).Contains(Classname)))
				{
					list.Add(lWindow);
				}
			}
		}
		return list.ToArray();
	}

	public static IntPtr FindWindowContains(string Classname, string WindowTitle)
	{
		if (Classname == null)
		{
			Classname = string.Empty;
		}
		if (WindowTitle == null)
		{
			WindowTitle = string.Empty;
		}
		lock (lWindowsLock)
		{
			if (!_EnumWindows())
			{
				return IntPtr.Zero;
			}
			foreach (IntPtr lWindow in lWindows)
			{
				if ((WindowTitle.Length > 0 && Imports.GetWindowTitle(lWindow).Contains(WindowTitle)) || (Classname.Length > 0 && Imports.GetClassName(lWindow).Contains(Classname)))
				{
					return lWindow;
				}
			}
		}
		return IntPtr.Zero;
	}

	public static IntPtr[] FindMainWindowsContains(string Classname, string WindowTitle)
	{
		if (Classname == null)
		{
			Classname = string.Empty;
		}
		if (WindowTitle == null)
		{
			WindowTitle = string.Empty;
		}
		List<IntPtr> list = new List<IntPtr>();
		Process[] processes = Process.GetProcesses();
		Process[] array = processes;
		foreach (Process process in array)
		{
			if (process.MainWindowHandle != IntPtr.Zero && ((WindowTitle.Length > 0 && process.MainWindowTitle.Contains(WindowTitle)) || (Classname.Length > 0 && Imports.GetClassName(process.MainWindowHandle).Contains(Classname))))
			{
				list.Add(process.MainWindowHandle);
			}
		}
		return list.ToArray();
	}

	public static IntPtr FindMainWindowContains(string Classname, string WindowTitle)
	{
		if (Classname == null)
		{
			Classname = string.Empty;
		}
		if (WindowTitle == null)
		{
			WindowTitle = string.Empty;
		}
		Process[] processes = Process.GetProcesses();
		Process[] array = processes;
		foreach (Process process in array)
		{
			if (process.MainWindowHandle != IntPtr.Zero && ((WindowTitle.Length > 0 && process.MainWindowTitle.Contains(WindowTitle)) || (Classname.Length > 0 && Imports.GetClassName(process.MainWindowHandle).Contains(Classname))))
			{
				return process.MainWindowHandle;
			}
		}
		return IntPtr.Zero;
	}

	public static IntPtr FindWindowByProcessName(string ProcessName)
	{
		if (ProcessName.EndsWith(".exe"))
		{
			ProcessName = ProcessName.Remove(ProcessName.Length - 4, 4);
		}
		Process[] processesByName = Process.GetProcessesByName(ProcessName);
		if (processesByName == null || processesByName.Length == 0)
		{
			return IntPtr.Zero;
		}
		return processesByName[0].MainWindowHandle;
	}

	public static IntPtr[] FindWindowsByProcessName(string ProcessName)
	{
		List<IntPtr> list = new List<IntPtr>();
		if (ProcessName.EndsWith(".exe"))
		{
			ProcessName = ProcessName.Remove(ProcessName.Length - 4, 4);
		}
		Process[] processesByName = Process.GetProcessesByName(ProcessName);
		if (processesByName == null || processesByName.Length == 0)
		{
			return null;
		}
		Process[] array = processesByName;
		foreach (Process process in array)
		{
			list.Add(process.MainWindowHandle);
		}
		return list.ToArray();
	}

	public static IntPtr FindWindowByProcessId(int dwProcessId)
	{
		Process processById = Process.GetProcessById(dwProcessId);
		return processById.MainWindowHandle;
	}
}
