using System;
using System.Diagnostics;

namespace Magic;

public static class SThread
{
	public static IntPtr OpenThread(uint dwDesiredAccess, int dwThreadId)
	{
		return Imports.OpenThread(dwDesiredAccess, bInheritHandle: false, (uint)dwThreadId);
	}

	public static IntPtr OpenThread(int dwThreadId)
	{
		return Imports.OpenThread(2032639u, bInheritHandle: false, (uint)dwThreadId);
	}

	public static int GetMainThreadId(int dwProcessId)
	{
		if (dwProcessId == 0)
		{
			return 0;
		}
		Process processById = Process.GetProcessById(dwProcessId);
		return processById.Threads[0].Id;
	}

	public static int GetMainThreadId(IntPtr hWindowHandle)
	{
		if (hWindowHandle == IntPtr.Zero)
		{
			return 0;
		}
		return GetMainThreadId(SProcess.GetProcessFromWindow(hWindowHandle));
	}

	public static ProcessThread GetMainThread(int dwProcessId)
	{
		if (dwProcessId == 0)
		{
			return null;
		}
		Process processById = Process.GetProcessById(dwProcessId);
		return processById.Threads[0];
	}

	public static ProcessThread GetMainThread(IntPtr hWindowHandle)
	{
		if (hWindowHandle == IntPtr.Zero)
		{
			return null;
		}
		return GetMainThread(SProcess.GetProcessFromWindow(hWindowHandle));
	}

	public static CONTEXT GetThreadContext(IntPtr hThread, uint ContextFlags)
	{
		CONTEXT lpContext = default(CONTEXT);
		lpContext.ContextFlags = ContextFlags;
		if (!Imports.GetThreadContext(hThread, ref lpContext))
		{
			lpContext.ContextFlags = 0u;
		}
		return lpContext;
	}

	public static bool SetThreadContext(IntPtr hThread, CONTEXT ctx)
	{
		return Imports.SetThreadContext(hThread, ref ctx);
	}

	public static uint SuspendThread(IntPtr hThread)
	{
		return Imports.SuspendThread(hThread);
	}

	public static uint ResumeThread(IntPtr hThread)
	{
		return Imports.ResumeThread(hThread);
	}

	public static uint TerminateThread(IntPtr hThread, uint dwExitCode)
	{
		return Imports.TerminateThread(hThread, dwExitCode);
	}

	public static IntPtr CreateRemoteThread(IntPtr hProcess, uint dwStartAddress, uint dwParameter)
	{
		uint dwThreadId;
		return CreateRemoteThread(hProcess, dwStartAddress, dwParameter, 0u, out dwThreadId);
	}

	public static IntPtr CreateRemoteThread(IntPtr hProcess, uint dwStartAddress, uint dwParameter, out uint dwThreadId)
	{
		return CreateRemoteThread(hProcess, dwStartAddress, dwParameter, 0u, out dwThreadId);
	}

	public static IntPtr CreateRemoteThread(IntPtr hProcess, uint dwStartAddress, uint dwParameter, uint dwCreationFlags, out uint dwThreadId)
	{
		IntPtr dwThreadId2;
		IntPtr result = Imports.CreateRemoteThread(hProcess, IntPtr.Zero, 0u, (IntPtr)dwStartAddress, (IntPtr)dwParameter, dwCreationFlags, out dwThreadId2);
		dwThreadId = (uint)(int)dwThreadId2;
		return result;
	}

	public static uint GetExitCodeThread(IntPtr hThread)
	{
		if (!Imports.GetExitCodeThread(hThread, out var lpExitCode))
		{
			throw new Exception("GetExitCodeThread failed.");
		}
		return (uint)lpExitCode;
	}

	public static uint WaitForSingleObject(IntPtr hObject)
	{
		return Imports.WaitForSingleObject(hObject, uint.MaxValue);
	}

	public static uint WaitForSingleObject(IntPtr hObject, uint dwMilliseconds)
	{
		return Imports.WaitForSingleObject(hObject, dwMilliseconds);
	}
}
