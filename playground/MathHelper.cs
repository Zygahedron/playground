﻿using System;
namespace playground
{
	public static class MathHelper
	{
		public static float DegreesToRadians(float degrees)
		{
			return MathF.PI / 180f * degrees;
		}
	}
}
