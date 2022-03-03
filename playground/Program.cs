using System;
using System.Drawing;
using Silk.NET.Windowing;
using Silk.NET.Maths;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using System.Numerics;
using System.Linq;

namespace playground
{
	class Program
	{
		private static IWindow window;
		private static GL Gl;
		private static IKeyboard primaryKeyboard;

		private const int Width = 800;
		private const int Height = 600;

		private static BufferObject<float> Vbo;
		private static VertexArrayObject<float> VaoCube;

		private static Shader LightingShader;
		private static Shader LampShader;
		private static Vector3 LampPosition = new Vector3(1.2f, 1.0f, 2.0f);

		private static Framebuffer MainCanvas;
		private static Shader PostShader;
		private static BufferObject<uint> EboScreen;
		private static BufferObject<float> VboScreen;
		private static VertexArrayObject<float> VaoScreen;

		private static Texture DiffuseMap;
		private static Texture SpecularMap;

		private static Camera Camera;

		private static Vector2 LastMousePosition;

		private static readonly float[] Vertices =
		{
            //X    Y      Z       Normals             U     V
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 0.0f, 0.0f,
			 0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 1.0f, 0.0f,
			 0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 1.0f, 1.0f,
			 0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 1.0f, 1.0f,
			-0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 0.0f, 1.0f,
			-0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 0.0f, 0.0f,

			-0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 0.0f, 0.0f,
			 0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 1.0f, 0.0f,
			 0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 1.0f, 1.0f,
			 0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 1.0f, 1.0f,
			-0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 0.0f, 1.0f,
			-0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 0.0f, 0.0f,

			-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f, 0.0f, 0.0f,
			-0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f, 1.0f, 0.0f,
			-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f, 1.0f, 1.0f,
			-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f, 1.0f, 1.0f,
			-0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f, 0.0f, 1.0f,
			-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f, 0.0f, 0.0f,

			 0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f, 0.0f, 0.0f,
			 0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f, 1.0f, 0.0f,
			 0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f, 1.0f, 1.0f,
			 0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f, 1.0f, 1.0f,
			 0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f, 0.0f, 1.0f,
			 0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f, 0.0f, 0.0f,

			-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f, 0.0f, 0.0f,
			 0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f, 1.0f, 0.0f,
			 0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f, 1.0f, 1.0f,
			 0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f, 1.0f, 1.0f,
			-0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f, 0.0f, 1.0f,
			-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f, 0.0f, 0.0f,

			-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f, 0.0f, 0.0f,
			 0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f, 1.0f, 0.0f,
			 0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f, 1.0f, 1.0f,
			 0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f, 1.0f, 1.0f,
			-0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f, 0.0f, 1.0f,
			-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f, 0.0f, 0.0f
		};

		private static readonly uint[] ScreenIndices =
		{
			0, 1, 3,
			1, 2, 3
		};

		private static readonly float[] ScreenVertices =
		{
			//X,   Y,  U,  V
			-1f, -1f, 0f, 0f,
			 1f, -1f, 1f, 0,
			 1f,  1f, 1f, 1f,
			-1f,  1f, 0f, 1f
		};

		private static void Main(/*string[] args*/)
		{
			var options = WindowOptions.Default;
			options.Size = new Vector2D<int>(Width, Height);
			options.Title = "LearnOpenGL with Silk.NET";
			options.PreferredDepthBufferBits = 24;
			window = Window.Create(options);

			window.Load += OnLoad;
			window.Update += OnUpdate;
			window.Render += OnRender;
			window.Closing += OnClose;

			window.Run();
		}

		private static void OnLoad()
		{
			IInputContext input = window.CreateInput();
			primaryKeyboard = input.Keyboards.FirstOrDefault();
			if (primaryKeyboard != null)
			{
				primaryKeyboard.KeyDown += KeyDown;
			}
			for (int i = 0; i < input.Mice.Count; i++)
			{
				input.Mice[i].Cursor.CursorMode = CursorMode.Raw;
				input.Mice[i].MouseMove += OnMouseMove;
				input.Mice[i].Scroll += OnMouseWheel;
			}

			Gl = GL.GetApi(window);

			//Ebo = new BufferObject<uint>(Gl, Indices, BufferTargetARB.ElementArrayBuffer);
			Vbo = new BufferObject<float>(Gl, Vertices, BufferTargetARB.ArrayBuffer);
			VaoCube = new VertexArrayObject<float>(Gl, Vbo);

			VaoCube.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 8, 0);
			VaoCube.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 8, 3);
			VaoCube.VertexAttributePointer(2, 2, VertexAttribPointerType.Float, 8, 6);

			LightingShader = new Shader(Gl, "shader.vert", "lighting.frag");
			LampShader = new Shader(Gl, "shader.vert", "shader.frag");

			Camera = new Camera(Vector3.UnitZ * 6, Vector3.UnitZ * -1, Vector3.UnitY, Width / Height);

			DiffuseMap = new Texture(Gl, "silkBoxed.png");
			SpecularMap = new Texture(Gl, "silkSpecular.png");

			MainCanvas = new Framebuffer(Gl);
			MainCanvas.AddTexture("color", FramebufferAttachment.ColorAttachment0, new Texture(Gl, Span<byte>.Empty, (uint)window.Size.X, (uint)window.Size.Y, TextureType.Rgb24));
			MainCanvas.AddTexture("depth", FramebufferAttachment.DepthAttachment, new Texture(Gl, Span<byte>.Empty, (uint)window.Size.X, (uint)window.Size.Y, TextureType.Depth24));
			MainCanvas.Complete();

			PostShader = new Shader(Gl, "post.vert", "post.frag");

			EboScreen = new BufferObject<uint>(Gl, ScreenIndices, BufferTargetARB.ElementArrayBuffer);
			VboScreen = new BufferObject<float>(Gl, ScreenVertices, BufferTargetARB.ArrayBuffer);
			VaoScreen = new VertexArrayObject<float>(Gl, VboScreen);

			VaoScreen.VertexAttributePointer(0, 2, VertexAttribPointerType.Float, 4, 0); // position
			VaoScreen.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 4, 2); // texcoords
		}

		private static void OnRender(double deltaTime)
		{
			MainCanvas.Bind();
			Gl.ClearColor(0.5f, 0.5f, 0.5f, 1f);
			Gl.Enable(EnableCap.DepthTest);
			Gl.Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

			VaoCube.Bind();
			RenderLitCube();
			RenderLampCube();

			Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			Gl.Disable(EnableCap.DepthTest);
			Gl.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
			Gl.Clear(ClearBufferMask.ColorBufferBit);

			PostShader.Use();
			PostShader.SetUniform("screenSize", (Vector2)window.Size);
			MainCanvas.BindTexture("color", TextureUnit.Texture0);
			PostShader.SetUniform("screenTexture", 0);
			MainCanvas.BindTexture("depth", TextureUnit.Texture1);
			PostShader.SetUniform("depthTexture", 1);
			VaoScreen.Bind();
			//Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
			EboScreen.Bind();
			Gl.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, ReadOnlySpan<uint>.Empty);
			//Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

		}

		private static void RenderLitCube()
		{
			LightingShader.Use();

			LightingShader.SetUniform("uModel", Matrix4x4.CreateRotationY(25f) * Matrix4x4.CreateRotationX(25f));
			LightingShader.SetUniform("uView", Camera.GetViewMatrix());
			LightingShader.SetUniform("uProjection", Camera.GetProjectionMatrix());
			LightingShader.SetUniform("viewPos", Camera.Position);

			DiffuseMap.Bind(TextureUnit.Texture0);
			SpecularMap.Bind(TextureUnit.Texture1);
			LightingShader.SetUniform("material.diffuse", 0);
			LightingShader.SetUniform("material.specular", 1);
			LightingShader.SetUniform("material.shininess", 32.0f);

			var lightColor = Vector3.One;

			var diffuseColor = lightColor * 0.5f;
			var ambientColor = diffuseColor * 0.2f;

			LightingShader.SetUniform("light.ambient", ambientColor);
			LightingShader.SetUniform("light.diffuse", diffuseColor);
			LightingShader.SetUniform("light.specular", new Vector3(1.0f));
			LightingShader.SetUniform("light.position", LampPosition);

			//EboScreen.Draw(36);
			//Gl.DrawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, (ReadOnlySpan<uint>)Indices);
			Gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
		}

		private static void RenderLampCube()
		{
			LampShader.Use();

			var lampMatrix = Matrix4x4.Identity;
			lampMatrix *= Matrix4x4.CreateScale(0.2f);
			lampMatrix *= Matrix4x4.CreateTranslation(LampPosition);

			LampShader.SetUniform("uModel", lampMatrix);
			LampShader.SetUniform("uView", Camera.GetViewMatrix());
			LampShader.SetUniform("uProjection", Camera.GetProjectionMatrix());

			Gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
		}

		private static void OnUpdate(double deltaTime)
		{
			var moveSpeed = 2.5f * (float)deltaTime;

			if (primaryKeyboard.IsKeyPressed(Key.W))
			{
				Camera.Position += moveSpeed * Camera.Front;
			}
			if (primaryKeyboard.IsKeyPressed(Key.S))
			{
				Camera.Position -= moveSpeed * Camera.Front;
			}
			if (primaryKeyboard.IsKeyPressed(Key.A))
			{
				Camera.Position -= moveSpeed * Camera.Right;
			}
			if (primaryKeyboard.IsKeyPressed(Key.D))
			{
				Camera.Position += moveSpeed * Camera.Right;
			}
		}

		private static void KeyDown(IKeyboard keyboard, Key key, int arg3)
		{
			if (key == Key.Escape)
			{
				window.Close();
			}
		}

		private static void OnMouseMove(IMouse mouse, Vector2 position)
		{
			var lookSensitivity = 0.1f;
			if (LastMousePosition == default) { LastMousePosition = position; }
			else
			{
				var offset = (position - LastMousePosition) * lookSensitivity;
				LastMousePosition = position;

				Camera.ModifyDirection(offset.X, offset.Y);
			}
		}

		private static void OnMouseWheel(IMouse mouse, ScrollWheel scrollWheel)
		{
			Camera.ModifyZoom(scrollWheel.Y);
		}

		private static void OnClose()
		{
			Vbo.Dispose();
			VaoCube.Dispose();
			LightingShader.Dispose();
			LampShader.Dispose();
			DiffuseMap.Dispose();
			SpecularMap.Dispose();
			MainCanvas.DisposeTextures();
			MainCanvas.Dispose();
			PostShader.Dispose();
			VboScreen.Dispose();
			VaoScreen.Dispose();
		}
	}
}
