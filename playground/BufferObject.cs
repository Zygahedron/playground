using System;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;

namespace playground
{
	public class BufferObject<TDataType> : IDisposable
		where TDataType : unmanaged
	{
		private uint _handle;
		private BufferTargetARB _bufferType;
		private GL _gl;

		public BufferObject(GL gl, Span<TDataType> data, BufferTargetARB bufferType)
		{
			_gl = gl;
			_bufferType = bufferType;

			_handle = _gl.GenBuffer();
			Bind();
			_gl.BufferData<TDataType>(bufferType, (uint)(data.Length * Marshal.SizeOf<TDataType>()), data, BufferUsageARB.StaticDraw);
		}

		public void Bind()
		{
			_gl.BindBuffer(_bufferType, _handle);
		}

		public void Draw(uint count, PrimitiveType type = PrimitiveType.Triangles)
		{
			_gl.DrawElements(type, count, DrawElementsType.UnsignedInt, _handle);
		}

		public void Dispose()
		{
			_gl.DeleteBuffer(_handle);
		}
	}
}
