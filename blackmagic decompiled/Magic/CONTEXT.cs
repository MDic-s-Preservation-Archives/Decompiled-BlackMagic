using System.Runtime.InteropServices;

namespace Magic;

public struct CONTEXT
{
	public uint ContextFlags;

	public uint Dr0;

	public uint Dr1;

	public uint Dr2;

	public uint Dr3;

	public uint Dr6;

	public uint Dr7;

	[MarshalAs(UnmanagedType.Struct)]
	public FLOATING_SAVE_AREA FloatSave;

	public uint SegGs;

	public uint SegFs;

	public uint SegEs;

	public uint SegDs;

	public uint Edi;

	public uint Esi;

	public uint Ebx;

	public uint Edx;

	public uint Ecx;

	public uint Eax;

	public uint Ebp;

	public uint Eip;

	public uint SegCs;

	public uint EFlags;

	public uint Esp;

	public uint SegSs;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
	public byte[] ExtendedRegisters;
}
