using System;
using System.IO;
using System.Text;
using System.Threading;
using Fasm;

namespace Magic;

public static class SInject
{
	private const uint RETURN_ERROR = 0u;

	public static uint InjectDllCreateThread(IntPtr hProcess, string szDllPath)
	{
		if (hProcess == IntPtr.Zero)
		{
			throw new ArgumentNullException("hProcess");
		}
		if (szDllPath.Length == 0)
		{
			throw new ArgumentNullException("szDllPath");
		}
		if (!szDllPath.Contains("\\"))
		{
			szDllPath = Path.GetFullPath(szDllPath);
		}
		if (!File.Exists(szDllPath))
		{
			throw new ArgumentException("DLL not found.", "szDllPath");
		}
		uint result = 0u;
		uint num = (uint)Imports.GetProcAddress(Imports.GetModuleHandle("kernel32.dll"), "LoadLibraryA");
		if (num != 0)
		{
			uint num2 = SMemory.AllocateMemory(hProcess);
			if (num2 != 0)
			{
				if (SMemory.WriteASCIIString(hProcess, num2, szDllPath))
				{
					IntPtr intPtr = SThread.CreateRemoteThread(hProcess, num, num2);
					if (SThread.WaitForSingleObject(intPtr, 5000u) == 0)
					{
						result = SThread.GetExitCodeThread(intPtr);
					}
					Imports.CloseHandle(intPtr);
				}
				SMemory.FreeMemory(hProcess, num2);
			}
		}
		return result;
	}

	public static uint InjectDllRedirectThread(IntPtr hProcess, IntPtr hThread, string szDllPath)
	{
		if (hProcess == IntPtr.Zero)
		{
			throw new ArgumentNullException("hProcess");
		}
		if (hThread == IntPtr.Zero)
		{
			throw new ArgumentNullException("hThread");
		}
		if (szDllPath.Length == 0)
		{
			throw new ArgumentNullException("szDllPath");
		}
		if (!szDllPath.Contains("\\"))
		{
			szDllPath = Path.GetFullPath(szDllPath);
		}
		if (!File.Exists(szDllPath))
		{
			throw new ArgumentException("DLL not found.", "szDllPath");
		}
		uint result = 0u;
		StringBuilder stringBuilder = new StringBuilder();
		ManagedFasm managedFasm = new ManagedFasm(hProcess);
		uint num = (uint)Imports.GetProcAddress(Imports.GetModuleHandle("kernel32.dll"), "LoadLibraryA");
		if (num == 0)
		{
			return 0u;
		}
		uint num2 = SMemory.AllocateMemory(hProcess);
		if (num2 == 0)
		{
			return 0u;
		}
		if (SThread.SuspendThread(hThread) != uint.MaxValue)
		{
			CONTEXT threadContext = SThread.GetThreadContext(hThread, 65537u);
			if (threadContext.Eip != 0)
			{
				try
				{
					managedFasm.AddLine("lpExitCode dd 0x{0:X}", uint.MaxValue);
					managedFasm.AddLine("push 0x{0:X}", threadContext.Eip);
					managedFasm.AddLine("pushad");
					managedFasm.AddLine("push szDllPath");
					managedFasm.AddLine("call 0x{0:X}", num);
					managedFasm.AddLine("mov [lpExitCode], eax");
					managedFasm.AddLine("popad");
					managedFasm.AddLine("retn");
					managedFasm.AddLine("szDllPath db '{0}',0", szDllPath);
					managedFasm.Inject(num2);
				}
				catch
				{
					SMemory.FreeMemory(hProcess, num2);
					SThread.ResumeThread(hThread);
					return 0u;
				}
				threadContext.ContextFlags = 65537u;
				threadContext.Eip = num2 + 4;
				if (SThread.SetThreadContext(hThread, threadContext) && SThread.ResumeThread(hThread) != uint.MaxValue)
				{
					for (int i = 0; i < 400; i++)
					{
						Thread.Sleep(5);
						if ((result = SMemory.ReadUInt(hProcess, num2)) != uint.MaxValue)
						{
							break;
						}
					}
				}
			}
		}
		if (managedFasm != null)
		{
			managedFasm.Dispose();
			managedFasm = null;
		}
		SMemory.FreeMemory(hProcess, num2);
		return result;
	}

	public static uint InjectDllRedirectThread(IntPtr hProcess, int dwProcessId, string szDllPath)
	{
		IntPtr intPtr = SThread.OpenThread(SThread.GetMainThreadId(dwProcessId));
		if (intPtr == IntPtr.Zero)
		{
			return 0u;
		}
		uint result = InjectDllRedirectThread(hProcess, intPtr, szDllPath);
		Imports.CloseHandle(intPtr);
		return result;
	}

	public static bool InjectCode(IntPtr hProcess, uint dwAddress, string szAssembly)
	{
		if (hProcess == IntPtr.Zero || szAssembly.Length == 0 || dwAddress == 0)
		{
			return false;
		}
		byte[] array;
		try
		{
			array = ManagedFasm.Assemble(szAssembly);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return SMemory.WriteBytes(hProcess, dwAddress, array, array.Length);
	}

	public static bool InjectCode(IntPtr hProcess, uint dwAddress, string szFormat, params object[] args)
	{
		return InjectCode(hProcess, dwAddress, string.Format(szFormat, args));
	}

	public static uint InjectCode(IntPtr hProcess, string szAssembly)
	{
		uint num = SMemory.AllocateMemory(hProcess);
		return InjectCode(hProcess, num, szAssembly) ? num : 0u;
	}

	public static uint InjectCode(IntPtr hProcess, string szFormatString, params object[] args)
	{
		return InjectCode(hProcess, string.Format(szFormatString, args));
	}
}
