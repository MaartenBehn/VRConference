using System;
using System.Collections.Generic;
using System.Text;
using Unity.Mathematics;
using UnityEngine;

namespace Network.Both
{
    
    public class Packet : IDisposable
    {
        private List<byte> buffer;
        private byte[] readableBuffer;
        private int readPos;

        /// <summary>Creates a new packet with a given ID. Used for sending.</summary>
        /// <param name="id">The packet ID.</param>
        public Packet(byte id, byte userID)
        {
            buffer = new List<byte>(); 
            readPos = 0;
            Write(id); 
            Write(userID);
        }

        /// <summary>Creates a packet from which data can be read. Used for receiving.</summary>
        /// <param name="data">The bytes to add to the packet.</param>
        public Packet(byte[] data)
        {
            buffer = new List<byte>();
            readPos = 0;
            Write(data);
        }

        #region Functions

        /// <summary>Gets the packet's content in array form.</summary>
        public byte[] ToArray()
        {
            return buffer.ToArray();
        }

        /// <summary>Gets the length of the packet's content.</summary>
        public int Length()
        {
            return buffer.Count; // Return the length of buffer
        }
        
        public int ReadPos()
        {
            return readPos; // Return the readPos
        }
        
        public int UnReadLength()
        {
            return buffer.Count - readPos; // Return the readPos
        }
        
        private bool readyToRead;
        public void PrepareForRead()
        {
            if (readyToRead)
                return;
            
            readableBuffer = ToArray();
            readyToRead = true;
        }

        private bool readyToSend;
        public void PrepareForSend()
        {
            if(readyToSend) 
                return;
            
            Write(0, Length());
            readyToSend = true;
        }
        #endregion
        
        #region Write Data
        public void Write(byte value)
        {
            buffer.Add(value);
        }
        public void Write(int pos, byte value)
        {
            buffer.Insert(pos, value);
        }
        
        public void Write(byte[] value)
        {
            buffer.AddRange(value);
        }
        public void Write(int pos, byte[] value)
        {
            buffer.InsertRange(pos, value);
        }
        
        public void Write(Int16 value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }
        public void Write(int pos, Int16 value)
        {
            buffer.InsertRange(pos, BitConverter.GetBytes(value));
        }
        
        public void Write(Int32 value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }
        public void Write(int pos, Int32 value)
        {
            buffer.InsertRange(pos, BitConverter.GetBytes(value));
        }
        
        public void Write(Int64 value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }
        public void Write(int pos, Int64 value)
        {
            buffer.InsertRange(pos, BitConverter.GetBytes(value));
        }
        
        public void Write(float value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }
        public void Write(int pos, float value)
        {
            buffer.InsertRange(pos, BitConverter.GetBytes(value));
        }
        
        public void Write(bool value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }
        public void Write(string value)
        {
            Write(value.Length); // Add the length of the string to the packet
            buffer.AddRange(Encoding.ASCII.GetBytes(value)); // Add the string itself
        }
        public void Write(int2 value)
        {
            Write(value.x);
            Write(value.y);
        }
        public void Write(int3 value)
        {
            Write(value.x);
            Write(value.y);
            Write(value.z);
        }
        public void Write(float2 value)
        {
            Write(value.x);
            Write(value.y);
        }
        public void Write(float3 value)
        {
            Write(value.x);
            Write(value.y);
            Write(value.z);
        }
        public void Write(float4 value)
        {
            Write(value.x);
            Write(value.y);
            Write(value.z);
            Write(value.w);
        }
        public void Write(Quaternion value)
        {
            Write(value.x);
            Write(value.y);
            Write(value.z);
            Write(value.w);
        }
        #endregion

        #region Read Data
        
        public byte ReadByte(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                byte value = readableBuffer[readPos];
                if (moveReadPos)
                {
                    readPos++;
                }
                return value; 
            }
            else
            {
                throw new Exception("Could not read value of type 'byte'!");
            }
        }
        
        public byte[] ReadBytes(int length, bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                byte[] value = buffer.GetRange(readPos, length).ToArray();
                if (moveReadPos)
                {
                    readPos += length;
                }
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'byte[]'!");
            }
        }
        
        public Int16 ReadInt16(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                Int16 value = BitConverter.ToInt16(readableBuffer, readPos);
                if (moveReadPos)
                {
                    readPos += 2;
                }
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'Int16'!");
            }
        }
        
        public UInt16 ReadUInt16(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                UInt16 value = BitConverter.ToUInt16(readableBuffer, readPos);
                if (moveReadPos)
                {
                    readPos += 2;
                }
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'UInt16'!");
            }
        }
        
        public Int32 ReadInt32(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                Int32 value = BitConverter.ToInt32(readableBuffer, readPos);
                if (moveReadPos)
                {
                    readPos += 4;
                }
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'Int32'!");
            }
        }
        
        public UInt32 ReadUInt32(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                UInt32 value = BitConverter.ToUInt32(readableBuffer, readPos);
                if (moveReadPos)
                {
                    readPos += 4;
                }
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'UInt32'!");
            }
        }
        
        public Int64 ReadInt64(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                Int64 value = BitConverter.ToInt64(readableBuffer, readPos);
                if (moveReadPos)
                {
                    readPos += 8;
                }
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'Int64'!");
            }
        }
        
        public UInt64 ReadUInt64(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                UInt64 value = BitConverter.ToUInt64(readableBuffer, readPos);
                if (moveReadPos)
                {
                    readPos += 8;
                }
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'UInt64'!");
            }
        }
        
        public float ReadFloat(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                float value = BitConverter.ToSingle(readableBuffer, readPos);
                if (moveReadPos)
                {
                    readPos += 4;
                }
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'float'!");
            }
        }
        
        public bool ReadBool(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                bool value = BitConverter.ToBoolean(readableBuffer, readPos); 
                if (moveReadPos)
                {
                    readPos += 1; 
                }
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'bool'!");
            }
        }
        
        public string ReadString(bool moveReadPos = true)
        {
            try
            {
                int length = ReadInt32();
                string value = Encoding.ASCII.GetString(readableBuffer, readPos, length); 
                if (moveReadPos && value.Length > 0)
                {
                    readPos += length;
                }
                return value;
            }
            catch
            {
                throw new Exception("Could not read value of type 'String'!");
            }
        }
        public int2 ReadInt2(bool moveReadPos = true)
        {
            return new int2(ReadInt32(moveReadPos), ReadInt32(moveReadPos));
        }
        public int3 ReadInt3(bool moveReadPos = true)
        {
            return new int3(ReadInt32(moveReadPos), ReadInt32(moveReadPos), ReadInt32(moveReadPos));
        }
        public float2 ReadFloat2(bool moveReadPos = true)
        {
            return new float2(ReadFloat(moveReadPos), ReadFloat(moveReadPos));
        }
        public float3 ReadFloat3(bool moveReadPos = true)
        {
            return new float3(ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos));
        }
        public float4 ReadFloat4(bool moveReadPos = true)
        {
            return new float4(ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos));
        }
        public Quaternion ReadQuaternion(bool moveReadPos = true)
        {
            return new Quaternion(ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos));
        }
        #endregion

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    buffer = null;
                    readableBuffer = null;
                    readPos = 0;
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}