namespace Magic;

public static class AccessRights
{
	public const uint STANDARD_RIGHTS_REQUIRED = 983040u;

	public const uint SYNCHRONIZE = 1048576u;

	public const uint PROCESS_TERMINATE = 1u;

	public const uint PROCESS_CREATE_THREAD = 2u;

	public const uint PROCESS_VM_OPERATION = 8u;

	public const uint PROCESS_VM_READ = 16u;

	public const uint PROCESS_VM_WRITE = 32u;

	public const uint PROCESS_DUP_HANDLE = 64u;

	public const uint PROCESS_CREATE_PROCESS = 128u;

	public const uint PROCESS_SET_QUOTA = 256u;

	public const uint PROCESS_SET_INFORMATION = 512u;

	public const uint PROCESS_QUERY_INFORMATION = 1024u;

	public const uint PROCESS_SUSPEND_RESUME = 2048u;

	public const uint PROCESS_QUERY_LIMITED_INFORMATION = 4096u;

	public const uint PROCESS_ALL_ACCESS = 2035711u;

	public const uint THREAD_TERMINATE = 1u;

	public const uint THREAD_SUSPEND_RESUME = 2u;

	public const uint THREAD_GET_CONTEXT = 8u;

	public const uint THREAD_SET_CONTEXT = 16u;

	public const uint THREAD_QUERY_INFORMATION = 64u;

	public const uint THREAD_SET_INFORMATION = 32u;

	public const uint THREAD_SET_THREAD_TOKEN = 128u;

	public const uint THREAD_IMPERSONATE = 256u;

	public const uint THREAD_DIRECT_IMPERSONATION = 512u;

	public const uint THREAD_ALL_ACCESS = 2032639u;
}
