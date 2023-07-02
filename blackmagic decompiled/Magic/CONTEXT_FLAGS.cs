namespace Magic;

public static class CONTEXT_FLAGS
{
	private const uint CONTEXT_i386 = 65536u;

	private const uint CONTEXT_i486 = 65536u;

	public const uint CONTEXT_CONTROL = 65537u;

	public const uint CONTEXT_INTEGER = 65538u;

	public const uint CONTEXT_SEGMENTS = 65540u;

	public const uint CONTEXT_FLOATING_POINT = 65544u;

	public const uint CONTEXT_DEBUG_REGISTERS = 65552u;

	public const uint CONTEXT_EXTENDED_REGISTERS = 65568u;

	public const uint CONTEXT_FULL = 65543u;

	public const uint CONTEXT_ALL = 65599u;
}
