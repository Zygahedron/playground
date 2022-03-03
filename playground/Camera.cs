using System;
using System.Numerics;

namespace playground
{
	public class Camera
	{
		public Vector3 Position { get; set; }
		public Vector3 Front { get; set; }

		public Vector3 Up { get; private set; }
		public Vector3 Right => Vector3.Normalize(Vector3.Cross(Front, Up));
		public float AspectRatio { get; set; }

		public float Yaw { get; set; } = -90f;
		public float Pitch { get; set; }

		private float Zoom = 45f;

		public Camera(Vector3 position, Vector3 front, Vector3 up, float aspectRatio)
		{
			Position = position;
			AspectRatio = aspectRatio;
			Front = front;
			Up = up;
		}

		public void ModifyZoom(float zoomAmount)
		{
			Zoom = Math.Clamp(Zoom - zoomAmount, 1.0f, 45f);
		}

		public void ModifyDirection(float xOffset, float yOffset)
		{
			Yaw += xOffset;
			Pitch = Math.Clamp(Pitch - yOffset, -89.99f, 89.99f);

			var cameraDirection = Vector3.Zero;
			cameraDirection.X = MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
			cameraDirection.Y = MathF.Sin(MathHelper.DegreesToRadians(Pitch));
			cameraDirection.Z = MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));

			Front = Vector3.Normalize(cameraDirection);
		}

		public Matrix4x4 GetViewMatrix()
		{
			return Matrix4x4.CreateLookAt(Position, Position + Front, Up);
		}

		public Matrix4x4 GetProjectionMatrix()
		{
			return Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Zoom), AspectRatio, 0.1f, 100.0f);
		}
	}
}
