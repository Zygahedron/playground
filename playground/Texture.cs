using System;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Advanced;

namespace playground
{
	public class Texture : IDisposable
	{
		private uint _handle;
		private GL _gl;

		public Texture(GL gl, string path, TextureType? textureType = null, GLEnum wrap = GLEnum.Repeat, GLEnum filter = GLEnum.Nearest)
		{

			Image<Rgba32> img = Image.Load<Rgba32>("assets/" + path);
			img.Mutate(x => x.Flip(FlipMode.Vertical));

			Load(gl, img.DangerousGetPixelRowMemory(0).Span, (uint) img.Width, (uint) img.Height, textureType ?? TextureType.Rgba32, wrap, filter);

			img.Dispose();
		}

		public Texture(GL gl, Span<byte> data, uint width, uint height, TextureType? textureType = null, GLEnum wrap = GLEnum.Repeat, GLEnum filter = GLEnum.Nearest)
		{

			Load(gl, data, width, height, textureType ?? TextureType.Rgba32, wrap, filter);
		}

		private void Load<T>(GL gl, Span<T> data, uint width, uint height, TextureType textureType, GLEnum wrap, GLEnum filter) where T : unmanaged
		{
			_gl = gl;

			_handle = _gl.GenTexture();
			Bind(TextureUnit.Texture0);

			_gl.TexImage2D<T>(TextureTarget.Texture2D, 0, textureType.InternalFormat, width, height, 0, textureType.PixelFormat, textureType.PixelType, data);
			_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) wrap);
			_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) wrap);
			_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) filter);
			_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) filter);

			_gl.GenerateMipmap(TextureTarget.Texture2D);
		}

		public void Bind(TextureUnit textureSlot)
		{
			_gl.ActiveTexture(textureSlot);
			_gl.BindTexture(TextureTarget.Texture2D, _handle);
			_gl.Enable(EnableCap.Texture2D);
		}

		public void Attach(FramebufferAttachment attachment)
		{
			_gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, attachment, TextureTarget.Texture2D, _handle, 0);
		}

		public void Dispose()
		{
			_gl.DeleteTexture(_handle);
		}
	}

	public struct TextureType
	{
		public InternalFormat InternalFormat;
		public PixelFormat PixelFormat;
		public PixelType PixelType;

		public static TextureType Rgba32 = new TextureType()
		{
			InternalFormat = InternalFormat.Rgba,
			PixelFormat = PixelFormat.Rgba,
			PixelType = PixelType.UnsignedByte,
		};
		public static TextureType Rgb24 = new TextureType()
		{
			InternalFormat = InternalFormat.Rgb,
			PixelFormat = PixelFormat.Rgb,
			PixelType = PixelType.UnsignedByte,
		};
		public static TextureType Depth24 = new TextureType()
		{
			InternalFormat = InternalFormat.DepthComponent24,
			PixelFormat = PixelFormat.DepthComponent,
			PixelType = PixelType.UnsignedInt,
		};
	}
}
