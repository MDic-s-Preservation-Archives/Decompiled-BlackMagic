using System;
using System.Collections.Generic;
using System.Diagnostics;
using Fasm;

namespace Magic;

public sealed class BlackMagic
{
	private class PatternDataEntry
	{
		public byte[] bData;

		public uint Start;

		public int Size;

		public PatternDataEntry()
		{
		}

		public PatternDataEntry(uint Start, int Size, byte[] bData)
		{
			this.Start = Start;
			this.Size = Size;
			this.bData = bData;
		}
	}

	private const uint RETURN_ERROR = 0u;

	public bool SetDebugPrivileges = true;

	private bool m_bProcessOpen = false;

	private bool m_bThreadOpen = false;

	private IntPtr m_hProcess = IntPtr.Zero;

	private int m_ProcessId = 0;

	private IntPtr m_hWnd = IntPtr.Zero;

	private int m_ThreadId = 0;

	private IntPtr m_hThread = IntPtr.Zero;

	private ProcessModule m_MainModule;

	private ProcessModuleCollection m_Modules;

	private List<PatternDataEntry> m_Data;

	public bool IsProcessOpen => m_bProcessOpen;

	public bool IsThreadOpen => m_bThreadOpen;

	public IntPtr ProcessHandle => m_hProcess;

	public int ProcessId => m_ProcessId;

	public IntPtr WindowHandle => m_hWnd;

	public int ThreadId => m_ThreadId;

	public IntPtr ThreadHandle => m_hThread;

	public ProcessModule MainModule => m_MainModule;

	public ProcessModuleCollection Modules => m_Modules;

	public ManagedFasm Asm { get; set; }

	public uint Execute(uint dwStartAddress, uint dwParameter)
	{
		UIntPtr lpExitCode = UIntPtr.Zero;
		bool flag = false;
		IntPtr intPtr = CreateRemoteThread(dwStartAddress, dwParameter);
		if (intPtr == IntPtr.Zero)
		{
			throw new Exception("Thread could not be remotely created.");
		}
		flag = SThread.WaitForSingleObject(intPtr, 10000u) == 0;
		if (flag)
		{
			flag = Imports.GetExitCodeThread(intPtr, out lpExitCode);
		}
		Imports.CloseHandle(intPtr);
		if (!flag)
		{
			throw new Exception("Error waiting for thread to exit or getting exit code.");
		}
		return (uint)lpExitCode;
	}

	public uint Execute(uint dwStartAddress)
	{
		return Execute(dwStartAddress, 0u);
	}

	public IntPtr CreateRemoteThread(uint dwStartAddress, uint dwParameter, uint dwCreationFlags, out uint dwThreadId)
	{
		if (m_bProcessOpen)
		{
			return SThread.CreateRemoteThread(m_hProcess, dwStartAddress, dwParameter, dwCreationFlags, out dwThreadId);
		}
		dwThreadId = 0u;
		return IntPtr.Zero;
	}

	public IntPtr CreateRemoteThread(uint dwStartAddress, uint dwParameter, out uint dwThreadId)
	{
		return CreateRemoteThread(dwStartAddress, dwParameter, 0u, out dwThreadId);
	}

	public IntPtr CreateRemoteThread(uint dwStartAddress, uint dwParameter)
	{
		uint dwThreadId;
		return CreateRemoteThread(dwStartAddress, dwParameter, 0u, out dwThreadId);
	}

	public bool SuspendThread(IntPtr hThread)
	{
		return SThread.SuspendThread(hThread) != uint.MaxValue;
	}

	public bool SuspendThread()
	{
		return m_bThreadOpen && SuspendThread(m_hThread);
	}

	public bool ResumeThread(IntPtr hThread)
	{
		return SThread.ResumeThread(hThread) != uint.MaxValue;
	}

	public bool ResumeThread()
	{
		return m_bThreadOpen && ResumeThread(m_hThread);
	}

	public BlackMagic()
	{
		Asm = new ManagedFasm();
		m_Data = new List<PatternDataEntry>();
		if (m_bProcessOpen && m_hProcess != IntPtr.Zero)
		{
			Asm.SetProcessHandle(m_hProcess);
		}
	}

	public BlackMagic(int ProcessId)
		: this()
	{
		m_bProcessOpen = Open(ProcessId);
	}

	public BlackMagic(IntPtr WindowHandle)
		: this(SProcess.GetProcessFromWindow(WindowHandle))
	{
	}

	~BlackMagic()
	{
		Close();
	}

	public bool Open(int ProcessId)
	{
		if (ProcessId == 0)
		{
			return false;
		}
		if (ProcessId == m_ProcessId)
		{
			return true;
		}
		if (m_bProcessOpen)
		{
			CloseProcess();
		}
		if (SetDebugPrivileges)
		{
			Process.EnterDebugMode();
		}
		m_bProcessOpen = (m_hProcess = SProcess.OpenProcess(ProcessId)) != IntPtr.Zero;
		if (m_bProcessOpen)
		{
			m_ProcessId = ProcessId;
			m_hWnd = SWindow.FindWindowByProcessId(ProcessId);
			m_Modules = Process.GetProcessById(m_ProcessId).Modules;
			m_MainModule = m_Modules[0];
			if (Asm == null)
			{
				Asm = new ManagedFasm(m_hProcess);
			}
			else
			{
				Asm.SetProcessHandle(m_hProcess);
			}
		}
		return m_bProcessOpen;
	}

	public bool Open(IntPtr WindowHandle)
	{
		if (WindowHandle == IntPtr.Zero)
		{
			return false;
		}
		return Open(SProcess.GetProcessFromWindow(WindowHandle));
	}

	public bool OpenThread(int dwThreadId)
	{
		if (dwThreadId == 0)
		{
			return false;
		}
		if (dwThreadId == m_ThreadId)
		{
			return true;
		}
		if (m_bThreadOpen)
		{
			CloseThread();
		}
		m_bThreadOpen = (m_hThread = SThread.OpenThread(dwThreadId)) != IntPtr.Zero;
		if (m_bThreadOpen)
		{
			m_ThreadId = dwThreadId;
		}
		return m_bThreadOpen;
	}

	public bool OpenThread()
	{
		if (m_bProcessOpen)
		{
			return OpenThread(SThread.GetMainThreadId(m_ProcessId));
		}
		return false;
	}

	public bool OpenProcessAndThread(int dwProcessId)
	{
		if (Open(dwProcessId) && OpenThread())
		{
			return true;
		}
		Close();
		return false;
	}

	public bool OpenProcessAndThread(IntPtr WindowHandle)
	{
		if (Open(WindowHandle) && OpenThread())
		{
			return true;
		}
		Close();
		return false;
	}

	public void Close()
	{
		Asm.Dispose();
		CloseProcess();
		CloseThread();
	}

	public void CloseProcess()
	{
		if (m_hProcess != IntPtr.Zero)
		{
			Imports.CloseHandle(m_hProcess);
		}
		m_hProcess = IntPtr.Zero;
		m_hWnd = IntPtr.Zero;
		m_ProcessId = 0;
		m_MainModule = null;
		m_Modules = null;
		m_bProcessOpen = false;
		Asm.SetProcessHandle(IntPtr.Zero);
	}

	public void CloseThread()
	{
		if (m_hThread != IntPtr.Zero)
		{
			Imports.CloseHandle(m_hThread);
		}
		m_hThread = IntPtr.Zero;
		m_ThreadId = 0;
		m_bThreadOpen = false;
	}

	public string GetModuleFilePath()
	{
		return m_MainModule.FileName;
	}

	public string GetModuleFilePath(int index)
	{
		return m_Modules[index].FileName;
	}

	public string GetModuleFilePath(string sModuleName)
	{
		foreach (ProcessModule module in m_Modules)
		{
			if (module.ModuleName.ToLower().Equals(sModuleName.ToLower()))
			{
				return module.FileName;
			}
		}
		return string.Empty;
	}

	public ProcessModule GetModule(string sModuleName)
	{
		foreach (ProcessModule module in m_Modules)
		{
			if (module.ModuleName.ToLower().Equals(sModuleName.ToLower()))
			{
				return module;
			}
		}
		return null;
	}

	public ProcessModule GetModule(uint dwAddress)
	{
		foreach (ProcessModule module in m_Modules)
		{
			if ((uint)(int)module.BaseAddress <= dwAddress && (uint)(int)module.BaseAddress + module.ModuleMemorySize >= dwAddress)
			{
				return module;
			}
		}
		return null;
	}

	public uint InjectDllCreateThread(string szDllPath)
	{
		if (!m_bProcessOpen)
		{
			return 0u;
		}
		return SInject.InjectDllCreateThread(m_hProcess, szDllPath);
	}

	public uint InjectDllRedirectThread(string szDllPath)
	{
		if (!m_bProcessOpen)
		{
			return 0u;
		}
		if (m_bThreadOpen)
		{
			return SInject.InjectDllRedirectThread(m_hProcess, m_hThread, szDllPath);
		}
		return SInject.InjectDllRedirectThread(m_hProcess, m_ProcessId, szDllPath);
	}

	public uint InjectDllRedirectThread(IntPtr hThread, string szDllPath)
	{
		if (!m_bProcessOpen)
		{
			return 0u;
		}
		return SInject.InjectDllRedirectThread(m_hProcess, hThread, szDllPath);
	}

	public bool WriteBytes(uint dwAddress, byte[] Value, int nSize)
	{
		return SMemory.WriteBytes(m_hProcess, dwAddress, Value, nSize);
	}

	public bool WriteBytes(uint dwAddress, byte[] Value)
	{
		return WriteBytes(dwAddress, Value, Value.Length);
	}

	public bool WriteByte(uint dwAddress, byte Value)
	{
		return SMemory.WriteByte(m_hProcess, dwAddress, Value);
	}

	public bool WriteSByte(uint dwAddress, sbyte Value)
	{
		return SMemory.WriteSByte(m_hProcess, dwAddress, Value);
	}

	public bool WriteUShort(uint dwAddress, ushort Value)
	{
		return SMemory.WriteUShort(m_hProcess, dwAddress, Value);
	}

	public bool WriteShort(uint dwAddress, short Value)
	{
		return SMemory.WriteShort(m_hProcess, dwAddress, Value);
	}

	public bool WriteUInt(uint dwAddress, uint Value)
	{
		return SMemory.WriteUInt(m_hProcess, dwAddress, Value);
	}

	public bool WriteInt(uint dwAddress, int Value)
	{
		return SMemory.WriteInt(m_hProcess, dwAddress, Value);
	}

	public bool WriteUInt64(uint dwAddress, ulong Value)
	{
		return SMemory.WriteUInt64(m_hProcess, dwAddress, Value);
	}

	public bool WriteInt64(uint dwAddress, long Value)
	{
		return SMemory.WriteInt64(m_hProcess, dwAddress, Value);
	}

	public bool WriteFloat(uint dwAddress, float Value)
	{
		return SMemory.WriteFloat(m_hProcess, dwAddress, Value);
	}

	public bool WriteDouble(uint dwAddress, double Value)
	{
		return SMemory.WriteDouble(m_hProcess, dwAddress, Value);
	}

	public bool WriteObject(uint dwAddress, object Value, Type objType)
	{
		return SMemory.WriteObject(m_hProcess, dwAddress, Value, objType);
	}

	public bool WriteObject(uint dwAddress, object Value)
	{
		return SMemory.WriteObject(m_hProcess, dwAddress, Value, Value.GetType());
	}

	public bool WriteASCIIString(uint dwAddress, string Value)
	{
		return SMemory.WriteASCIIString(m_hProcess, dwAddress, Value);
	}

	public bool WriteUnicodeString(uint dwAddress, string Value)
	{
		return SMemory.WriteUnicodeString(m_hProcess, dwAddress, Value);
	}

	public byte[] ReadBytes(uint dwAddress, int nSize)
	{
		if (!m_bProcessOpen || m_hProcess == IntPtr.Zero)
		{
			throw new Exception("Process is not open for read/write.");
		}
		return SMemory.ReadBytes(m_hProcess, dwAddress, nSize);
	}

	public byte ReadByte(uint dwAddress)
	{
		if (!m_bProcessOpen || m_hProcess == IntPtr.Zero)
		{
			throw new Exception("Process is not open for read/write.");
		}
		return SMemory.ReadByte(m_hProcess, dwAddress);
	}

	public sbyte ReadSByte(uint dwAddress)
	{
		if (!m_bProcessOpen || m_hProcess == IntPtr.Zero)
		{
			throw new Exception("Process is not open for read/write.");
		}
		return SMemory.ReadSByte(m_hProcess, dwAddress);
	}

	public ushort ReadUShort(uint dwAddress)
	{
		return ReadUShort(dwAddress, bReverse: false);
	}

	public ushort ReadUShort(uint dwAddress, bool bReverse)
	{
		if (!m_bProcessOpen || m_hProcess == IntPtr.Zero)
		{
			throw new Exception("Process is not open for read/write.");
		}
		return SMemory.ReadUShort(m_hProcess, dwAddress, bReverse);
	}

	public short ReadShort(uint dwAddress)
	{
		return ReadShort(dwAddress, bReverse: false);
	}

	public short ReadShort(uint dwAddress, bool bReverse)
	{
		if (!m_bProcessOpen || m_hProcess == IntPtr.Zero)
		{
			throw new Exception("Process is not open for read/write.");
		}
		return SMemory.ReadShort(m_hProcess, dwAddress, bReverse);
	}

	public uint ReadUInt(uint dwAddress)
	{
		return ReadUInt(dwAddress, bReverse: false);
	}

	public uint ReadUInt(uint dwAddress, bool bReverse)
	{
		if (!m_bProcessOpen || m_hProcess == IntPtr.Zero)
		{
			throw new Exception("Process is not open for read/write.");
		}
		return SMemory.ReadUInt(m_hProcess, dwAddress, bReverse);
	}

	public int ReadInt(uint dwAddress)
	{
		return ReadInt(dwAddress, bReverse: false);
	}

	public int ReadInt(uint dwAddress, bool bReverse)
	{
		if (!m_bProcessOpen || m_hProcess == IntPtr.Zero)
		{
			throw new Exception("Process is not open for read/write.");
		}
		return SMemory.ReadInt(m_hProcess, dwAddress, bReverse);
	}

	public ulong ReadUInt64(uint dwAddress)
	{
		return ReadUInt64(dwAddress, bReverse: false);
	}

	public ulong ReadUInt64(uint dwAddress, bool bReverse)
	{
		if (!m_bProcessOpen || m_hProcess == IntPtr.Zero)
		{
			throw new Exception("Process is not open for read/write.");
		}
		return SMemory.ReadUInt64(m_hProcess, dwAddress, bReverse);
	}

	public long ReadInt64(uint dwAddress)
	{
		return ReadInt64(dwAddress, bReverse: false);
	}

	public long ReadInt64(uint dwAddress, bool bReverse)
	{
		if (!m_bProcessOpen || m_hProcess == IntPtr.Zero)
		{
			throw new Exception("Process is not open for read/write.");
		}
		return SMemory.ReadInt64(m_hProcess, dwAddress, bReverse);
	}

	public float ReadFloat(uint dwAddress)
	{
		return ReadFloat(dwAddress, bReverse: false);
	}

	public float ReadFloat(uint dwAddress, bool bReverse)
	{
		if (!m_bProcessOpen || m_hProcess == IntPtr.Zero)
		{
			throw new Exception("Process is not open for read/write.");
		}
		return SMemory.ReadFloat(m_hProcess, dwAddress, bReverse);
	}

	public double ReadDouble(uint dwAddress)
	{
		return ReadDouble(dwAddress, bReverse: false);
	}

	public double ReadDouble(uint dwAddress, bool bReverse)
	{
		if (!m_bProcessOpen || m_hProcess == IntPtr.Zero)
		{
			throw new Exception("Process is not open for read/write.");
		}
		return SMemory.ReadDouble(m_hProcess, dwAddress, bReverse);
	}

	public object ReadObject(uint dwAddress, Type objType)
	{
		if (!m_bProcessOpen || m_hProcess == IntPtr.Zero)
		{
			throw new Exception("Process is not open for read/write.");
		}
		return SMemory.ReadObject(m_hProcess, dwAddress, objType);
	}

	public string ReadASCIIString(uint dwAddress, int nLength)
	{
		return SMemory.ReadASCIIString(m_hProcess, dwAddress, nLength);
	}

	public string ReadUnicodeString(uint dwAddress, int nLength)
	{
		return SMemory.ReadUnicodeString(m_hProcess, dwAddress, nLength);
	}

	public uint AllocateMemory(int nSize, uint dwAllocationType, uint dwProtect)
	{
		return SMemory.AllocateMemory(m_hProcess, nSize, dwAllocationType, dwProtect);
	}

	public uint AllocateMemory(int nSize)
	{
		return AllocateMemory(nSize, 4096u, 64u);
	}

	public uint AllocateMemory()
	{
		return AllocateMemory(4096);
	}

	public bool FreeMemory(uint dwAddress, int nSize, uint dwFreeType)
	{
		return SMemory.FreeMemory(m_hProcess, dwAddress, nSize, dwFreeType);
	}

	public bool FreeMemory(uint dwAddress)
	{
		return FreeMemory(dwAddress, 0, 32768u);
	}

	public uint FindPattern(byte[] bPattern, string szMask)
	{
		return FindPattern((uint)(int)MainModule.BaseAddress, MainModule.ModuleMemorySize, bPattern, szMask);
	}

	public uint FindPattern(string szPattern, string szMask, char Delimiter)
	{
		string[] array = szPattern.Split(Delimiter);
		byte[] array2 = new byte[array.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = Convert.ToByte(array[i], 16);
		}
		return FindPattern(array2, szMask);
	}

	public uint FindPattern(string szPattern, string szMask)
	{
		return FindPattern(szPattern, szMask, ' ');
	}

	public uint FindPattern(ProcessModule pModule, byte[] bPattern, string szMask)
	{
		return FindPattern((uint)(int)pModule.BaseAddress, pModule.ModuleMemorySize, bPattern, szMask);
	}

	public uint FindPattern(ProcessModule pModule, string szPattern, string szMask, char Delimiter)
	{
		string[] array = szPattern.Split(Delimiter);
		byte[] array2 = new byte[array.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = Convert.ToByte(array[i], 16);
		}
		return FindPattern(pModule, array2, szMask);
	}

	public uint FindPattern(ProcessModule pModule, string szPattern, string szMask)
	{
		return FindPattern(pModule, szPattern, szMask, ' ');
	}

	public uint FindPattern(ProcessModuleCollection pModules, byte[] bPattern, string szMask)
	{
		uint num = 0u;
		foreach (ProcessModule pModule in pModules)
		{
			num = FindPattern(pModule, bPattern, szMask);
			if (num != 0)
			{
				break;
			}
		}
		return num;
	}

	public uint FindPattern(ProcessModuleCollection pModules, string szPattern, string szMask, char Delimiter)
	{
		string[] array = szPattern.Split(Delimiter);
		byte[] array2 = new byte[array.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = Convert.ToByte(array[i], 16);
		}
		return FindPattern(pModules, array2, szMask);
	}

	public uint FindPattern(ProcessModuleCollection pModules, string szPattern, string szMask)
	{
		return FindPattern(pModules, szPattern, szMask, ' ');
	}

	public uint FindPattern(ProcessModule[] pModules, byte[] bPattern, string szMask)
	{
		return FindPattern(new ProcessModuleCollection(pModules), bPattern, szMask);
	}

	public uint FindPattern(ProcessModule[] pModules, string szPattern, string szMask, char Delimiter)
	{
		return FindPattern(new ProcessModuleCollection(pModules), szPattern, szMask, Delimiter);
	}

	public uint FindPattern(ProcessModule[] pModules, string szPattern, string szMask)
	{
		return FindPattern(new ProcessModuleCollection(pModules), szPattern, szMask, ' ');
	}

	public uint FindPattern(uint dwStart, int nSize, byte[] bPattern, string szMask)
	{
		PatternDataEntry patternDataEntry = null;
		foreach (PatternDataEntry datum in m_Data)
		{
			if (dwStart == datum.Start && nSize == datum.Size)
			{
				patternDataEntry = datum;
				break;
			}
		}
		if (patternDataEntry == null)
		{
			patternDataEntry = new PatternDataEntry(dwStart, nSize, ReadBytes(dwStart, nSize));
			m_Data.Add(patternDataEntry);
		}
		return dwStart + SPattern.FindPattern(patternDataEntry.bData, bPattern, szMask);
	}

	public uint FindPattern(uint dwStart, int nSize, string szPattern, string szMask, char Delimiter)
	{
		string[] array = szPattern.Split(Delimiter);
		byte[] array2 = new byte[array.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = Convert.ToByte(array[i], 16);
		}
		return FindPattern(dwStart, nSize, array2, szMask);
	}

	public uint FindPattern(uint dwStart, int nSize, string szPattern, string szMask)
	{
		return FindPattern(dwStart, nSize, szPattern, szMask, ' ');
	}
}
