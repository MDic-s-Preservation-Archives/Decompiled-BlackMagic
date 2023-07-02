namespace Magic;

public static class MemoryProtectType
{
	public const uint PAGE_EXECUTE = 16u;

	public const uint PAGE_EXECUTE_READ = 32u;

	public const uint PAGE_EXECUTE_READWRITE = 64u;

	public const uint PAGE_EXECUTE_WRITECOPY = 128u;

	public const uint PAGE_NOACCESS = 1u;

	public const uint PAGE_READONLY = 2u;

	public const uint PAGE_READWRITE = 4u;

	public const uint PAGE_WRITECOPY = 8u;

	public const uint PAGE_GUARD = 256u;

	public const uint PAGE_NOCACHE = 512u;

	public const uint PAGE_WRITECOMBINE = 1024u;
}
