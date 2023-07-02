using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Magic;

public static class Imports
{
	public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

	[DllImport("user32")]
	public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

	[DllImport("user32", EntryPoint = "GetWindowText")]
	private static extern int _GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

	public static string GetWindowTitle(IntPtr hWnd, int nMaxCount)
	{
		StringBuilder stringBuilder = new StringBuilder(nMaxCount);
		int num;
		if ((num = _GetWindowText(hWnd, stringBuilder, nMaxCount)) > 0)
		{
			return stringBuilder.ToString(0, num);
		}
		return null;
	}

	public static string GetWindowTitle(IntPtr hWnd)
	{
		return GetWindowTitle(hWnd, 256);
	}

	[DllImport("user32", EntryPoint = "GetClassName")]
	private static extern int _GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

	public static string GetClassName(IntPtr hWnd, int nMaxCount)
	{
		StringBuilder stringBuilder = new StringBuilder(nMaxCount);
		int num;
		if ((num = _GetClassName(hWnd, stringBuilder, nMaxCount)) > 0)
		{
			return stringBuilder.ToString(0, num);
		}
		return null;
	}

	public static string GetClassName(IntPtr hWnd)
	{
		return GetClassName(hWnd, 256);
	}

	[DllImport("user32")]
	public static extern bool IsWindowVisible(IntPtr hWnd);

	[DllImport("user32")]
	public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int dwProcessId);

	[DllImport("kernel32")]
	public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

	[DllImport("kernel32")]
	public static extern bool CloseHandle(IntPtr hObject);

	[DllImport("kernel32", EntryPoint = "GetModuleHandleW")]
	public static extern UIntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string lpModuleName);

	[DllImport("kernel32")]
	public static extern UIntPtr GetProcAddress(UIntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

	[DllImport("kernel32")]
	public static extern bool ReadProcessMemory(IntPtr hProcess, uint dwAddress, IntPtr lpBuffer, int nSize, out int lpBytesRead);

	[DllImport("kernel32")]
	public static extern bool WriteProcessMemory(IntPtr hProcess, uint dwAddress, IntPtr lpBuffer, int nSize, out IntPtr iBytesWritten);

	[DllImport("kernel32")]
	public static extern uint VirtualAllocEx(IntPtr hProcess, uint dwAddress, int nSize, uint dwAllocationType, uint dwProtect);

	[DllImport("kernel32")]
	public static extern bool VirtualFreeEx(IntPtr hProcess, uint dwAddress, int nSize, uint dwFreeType);

	[DllImport("kernel32")]
	public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out IntPtr dwThreadId);

	[DllImport("kernel32")]
	public static extern uint WaitForSingleObject(IntPtr hObject, uint dwMilliseconds);

	[DllImport("kernel32")]
	public static extern bool GetExitCodeThread(IntPtr hThread, out UIntPtr lpExitCode);

	[DllImport("kernel32")]
	public static extern IntPtr OpenThread(uint dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

	[DllImport("kernel32")]
	public static extern uint SuspendThread(IntPtr hThread);

	[DllImport("kernel32")]
	public static extern uint ResumeThread(IntPtr hThread);

	[DllImport("kernel32")]
	public static extern uint TerminateThread(IntPtr hThread, uint dwExitCode);

	[DllImport("kernel32")]
	public static extern bool GetThreadContext(IntPtr hThread, ref CONTEXT lpContext);

	[DllImport("kernel32")]
	public static extern bool SetThreadContext(IntPtr hThread, ref CONTEXT lpContext);
}
