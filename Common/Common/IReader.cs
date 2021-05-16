using System;

namespace GR
{
  namespace IO
  {
	  /// <summary>
	  /// Zusammenfassung f�r BinaryReader.
	  /// </summary>
	  public abstract class IReader
	  {

      public abstract float ReadF32();
      public abstract System.UInt32 ReadUInt32();
      public abstract System.Int32 ReadInt32();
      public abstract System.UInt16 ReadUInt16();
      public abstract System.Byte ReadUInt8();
      public abstract System.UInt32 ReadUInt32NetworkOrder();
      public abstract System.Int32 ReadInt32NetworkOrder();
      public abstract System.UInt16 ReadUInt16NetworkOrder();
      public abstract UInt32 ReadBlock( GR.Memory.ByteBuffer BufferTarget, UInt32 BytesToRead );
      public abstract String ReadString();
      public abstract bool ReadLine( out String Line );
      public abstract void Close();
      public abstract long Size { get; }
      public abstract long Position { get; }
      public abstract void Skip( int BytesToSkip );
      public abstract bool DataAvailable
      {
        get;
      }
    }


  }
}