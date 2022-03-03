using System;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;

namespace playground
{
	public class VertexArrayObject<TVertexType> : IDisposable
		where TVertexType : unmanaged
	{
		private uint _handle;
		private GL _gl;

		public VertexArrayObject(GL gl, BufferObject<TVertexType> vbo)
		{
			_gl = gl;

			_handle = _gl.GenVertexArray();
			Bind();
			vbo.Bind();
		}

		public unsafe void VertexAttributePointer(uint index, int count, VertexAttribPointerType type, uint vertexSize, int offSet)
		{
			_gl.VertexAttribPointer(index, count, type, false, vertexSize * (uint) Marshal.SizeOf<TVertexType>(), (void*)(offSet * Marshal.SizeOf<TVertexType>()));
			_gl.EnableVertexAttribArray(index);
		}

		public void Bind()
		{
			_gl.BindVertexArray(_handle);
		}

		public void Dispose()
		{
			_gl.DeleteVertexArray(_handle);
		}
	}
}
