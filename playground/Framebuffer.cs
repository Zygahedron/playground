using System;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Advanced;
using System.Collections.Generic;

namespace playground
{
	public class Framebuffer : IDisposable
	{
		public readonly uint _handle;
		private readonly GL _gl;
		private readonly Dictionary<string, Texture> _textures = new Dictionary<string, Texture>();

		public Framebuffer(GL gl)
		{
			_gl = gl;

			_handle = _gl.GenFramebuffer();
			Bind();
		}

		public void AddTexture(string name, FramebufferAttachment attachment, Texture texture)
		{
			_textures[name] = texture;
			texture.Attach(attachment);
		}

		public void Complete()
		{
			_gl.BindTexture(TextureTarget.Texture2D, 0);

			if (_gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != GLEnum.FramebufferComplete)
			{
				throw new Exception("Framebuffer is not complete!");
			}
			_gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		}

		public Texture GetTexture(string name)
		{
			return _textures[name];
		}

		public void Bind()
		{
			_gl.BindFramebuffer(FramebufferTarget.Framebuffer, _handle);
		}

		public void BindTexture(string name, TextureUnit textureSlot)
		{
			GetTexture(name).Bind(textureSlot);
		}

		public void DisposeTextures()
		{
			foreach (var texture in _textures.Values)
			{
				texture.Dispose();
			}
		}

		public void Dispose()
		{
			_gl.DeleteFramebuffer(_handle);
		}
	}
}
