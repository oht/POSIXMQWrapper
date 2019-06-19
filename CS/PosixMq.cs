using System;
using System.Runtime.InteropServices;
using System.Text;

namespace POSIXComm
{
    /// <summary>
    /// /proc interfaces for POSIX message queues:
    /// 
    /// /proc/sys/fs/mqueue/msg_default     (Default: 10) (Linux 3.5+)
    /// /proc/sys/fs/mqueue/msg_max         (Default: 10) (HARD_MSGMAX: 65536 in Linux 3.5+) 
    /// /proc/sys/fs/mqueue/msgsize_default (Default: 8192) (Linux 3.5+)
    /// /proc/sys/fs/mqueue/msgsize_max     (Default: 8192) (HARD_MSGSIZEMAX: 16,777,216 in Linux 3.5+) 
    /// /proc/sys/fs/mqueue/queues_max      (Default: 256)
    /// </summary>

    [Flags]
	public enum OpenFlags
    {
		//
		// One of these
		//
		O_RDONLY    = 0,    // Open the queue to receive messages only.
        O_WRONLY    = 1,    // Open the queue to send messages only.
        O_RDWR      = 2,    // Open the queue to both send and receive messages.

        //
        // Or-ed with zero or more of these
        //
        O_CREAT     = 64, //Octal - 0100            // Create message queue if it does not exist. Also needs Mode (File permissions), mq_maxmsg (Max # of msgs on queue), and mq_msgsize (Max msg size), see pmq_open_attr() below.
		O_EXCL      = 128, // Octal - 0200          // If O_CREAT was specified in oflag, and a queue with the given name already exists, then fail with the error EEXIST.
        O_NOCTTY    = 256, // Octal - 0400          
		O_TRUNC     = 512, //Octal - 01000 
		O_APPEND    = 1024, //Octal -02000
		O_NONBLOCK  = 2048, // Octal -04000         // Open the queue in nonblocking mode. In circumstances where mq_receive(3) and mq_send(3) would normally block, these functions instead fail with the error EAGAIN.
        O_SYNC      = 1052672, //Octal -04010000

		//
		// These are non-Posix, think of a way of exposing
		// this for Linux users.
		//

		// O_NOFOLLOW  = 512,
		// O_DIRECTORY = 1024,
		// O_DIRECT    = 2048,
		// O_ASYNC     = 4096,
		// O_LARGEFILE = 8192
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct sigval                /* Data passed with notification */
    {          
		[FieldOffset(0)]
		public int sival_int;           /* Integer value */
		//[FieldOffset(0)]
		//public IntPtr? sival_ptr;     /* Pointer value */
	};

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void NotifyHandler(sigval sigVal);


	public class PosixMq
	{
		const string Posixlibpath = "./libposixwrapper.so";
		
        [DllImport(Posixlibpath, EntryPoint = "pmq_open", CallingConvention = CallingConvention.Cdecl)]
		public static extern int pmq_open(string mqName, OpenFlags flags, ref Int32 err);
		
        [DllImport(Posixlibpath, EntryPoint = "pmq_open_attr", CallingConvention = CallingConvention.Cdecl)]
		public static extern int pmq_open_attr(string mqName, OpenFlags flags, Int32 mode, Int32 maxMsg, Int32 msgSize, ref Int32 err);

        [DllImport(Posixlibpath, EntryPoint = "pmq_open_defaults", CallingConvention = CallingConvention.Cdecl)]
        public static extern int pmq_open_defaults(string mqName, OpenFlags flags, Int32 mode, ref Int32 err);

        [DllImport(Posixlibpath, EntryPoint = "pmq_close", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int pmq_close(int mqHandle, ref Int32 err);

        [DllImport(Posixlibpath, EntryPoint = "pmq_notify", CallingConvention = CallingConvention.Cdecl)]
        public static extern int pmq_notify(int mqHandle, NotifyHandler notifyHandler, ref Int32 err);

        [DllImport(Posixlibpath, EntryPoint = "pmq_receive", CharSet = CharSet.None, CallingConvention = CallingConvention.Cdecl)]
        public static extern int pmq_receive(int mqHandle, byte[] message, int size, ref UInt32 msgPriorty, Int32 timeOut, ref Int32 err);

        [DllImport(Posixlibpath, EntryPoint = "pmq_send", CharSet = CharSet.None, CallingConvention = CallingConvention.Cdecl)]
		public static extern int pmq_send(int mqHandle, byte[] message, int size, uint msgPriorty, Int32 timeOut, ref Int32 err);

        [DllImport(Posixlibpath, EntryPoint = "pmq_unlink", CallingConvention = CallingConvention.Cdecl)]
		public static extern int pmq_unlink(string mqName, ref Int32 err);

	}
}

