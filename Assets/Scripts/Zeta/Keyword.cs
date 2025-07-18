using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Keyword // namespace
{
    public class Representation
    {
        #region 数据类型
        public bool @bool; // bool

        public char @char; // char
        public string @string; // string

        public sbyte @sbyte; // sbyte
        public byte @byte; // byte
        public short @short; // short
        public ushort @ushort; // ushort
        public int @int; // int
        public uint @uint; // uint
        public long @long; // long
        public ulong @ulong; // ulong

        public float @float; // float
        public double @double; // double
        public decimal @decimal; // decimal

        public dynamic @dynamic; // dynamic

        public void Void() // void
        {
            dynamic @dynamic = "dynamic";
            var @var = "var"; // var
            Console.WriteLine($"{@var}-void");
            Console.WriteLine($"{@dynamic}-void");
        }
        #endregion

    }

    #region 访问修饰符-顶级类型
    public class PublicClass { }
    internal class InternalClass { }
    //file class FileClass{ }  C# 11.0

    public struct PublicStruct { }
    internal struct InternalStruct { }
    //file struct FileStruct{ }  C# 11.0

    public interface IPublicInterface { }
    internal interface IInternalInterface { }
    //file interface IFileInterface{ }  C# 11.0

    public delegate void PublicDelegate();
    internal delegate void InternalDelegate();
    //file delegate void FileDelegate();  C# 11.0

    public enum PublicEnum { }
    internal enum InternalEnum { }
    //file enum FileEnum { }  C# 11.0
    #endregion

    #region 类型声明
    // class partial sealed abstract static //
    class Class { }
    partial class PartialClass { }
    sealed class SealedClass { }
    sealed partial class SealedPartialClass { }
    abstract class AbstractClass { }
    abstract partial class AbstractPartialClass { }
    static class StaticClass { }
    static partial class StaticPartialClass { }
    // struct partial ref readonly "ref readonly" //
    struct Struct { }
    partial struct PartialStruct { }
    ref struct RefStruct { }
    ref partial struct RefPartialStruct { }
    readonly struct ReadonlyStruct { }
    readonly partial struct ReadonlyPartialStruct { }
    readonly ref struct RefReadonlyStruct { }
    readonly ref partial struct RefReadonlyPartialStruct { }

    interface Interface { }
    partial interface PartialInterface { }

    delegate void Delegate();

    enum Enum { };

    record Record { }
    partial record PartialRecord { }
    sealed record StaticRecord { }
    sealed partial record StaticPartialRecord { }
    abstract record AbstractRecord { }
    abstract partial record AbstractPartialRecord { }
    //record class Record { }  C# 10.0
    //record struct RecordStruct { } C# 10.0
    #endregion

    class _Class
    {
        #region 访问修饰符-嵌套类型与成员
        public int @public; // public
        protected internal int @protectedInternal;
        internal int @internal; // internal
        private protected int @privateProtected;
        protected int @protected; // protected
        private int @private; // private
        #endregion
        const int @const = 1;
        readonly int @readonly;
        static int @static;
        static readonly int @staticReadonly;

        //ref int @ref;  C# 11.0
        int Property1 { get; }
        int Property11
        {
            get { return default; }
        }
        int Property2 { get; set; }
        int property22;
        int Property22
        {
            get { return property22; }
            set { property22 = value; }
        }
        int Property3 { get; set; } = default;

    }
    struct _Struct
    {
        #region 访问修饰符-嵌套类型与成员
        public int @public;
        internal int @internal;
        private int @private;
        #endregion
        const int @const = 1;
        readonly int @readonly;
        static int @static;
        static readonly int @staticReadonly;

        //ref int @ref;  C# 11.0


        int Property1 { get; }
        int Property2 { get; set; }
        //int Property3 { get; set; } = default;  C# 10.0

        event Action Event { add { } remove { } }
        ////static event Action StaticEvent;

        void Out(out int @out, ref int @ref, in int @in)
        {
            @out = 1;
        }
    }
}