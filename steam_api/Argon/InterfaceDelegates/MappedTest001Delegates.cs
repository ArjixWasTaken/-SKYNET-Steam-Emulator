using System;
using System.Runtime.InteropServices;

// Autogenerated @ 21/08/18
namespace InterfaceCore
{
    /// <summary>
    /// Exports the delegates for all interfaces that implement MappedTest001
    /// </summary>
    [Core.Interface.Delegate(Name = "MappedTest001")]
    class MappedTest001_Delegates
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int PointerTest(IntPtr _, ref int a, ref int b, ref int c);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int BufferTest(IntPtr _, IntPtr b_pointer, int b_length);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void StructTest(IntPtr _, IntPtr b_pointer, int b_length);
    }
}