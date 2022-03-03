using System;
namespace playground
{
	public class Grid
	{
		private readonly int[,] _data;
		private readonly int[] _vertices;
		private readonly int[] _indices;

		//                            xy, tile
		public const int vertex_size = 2 + 1;

		public Grid(int size)
		{
			_data = new int[size, size];
			_vertices = new int[(size + 1) * (size + 1) * vertex_size];
			_indices = new int[size * size * 6];

			for (int i = 0; i <= size; i++)
			{
				for (int j = 0; j <= size; j++)
				{
					int r = size + 1;
					int vertexIndex = (j * (size + 1) + i) * vertex_size;
					_vertices[vertexIndex + 0] = i; // x
					_vertices[vertexIndex + 1] = j; // y
					_vertices[vertexIndex + 2] = 0; // tile
					if (i < size && j < size) // not last row/column
					{
						_data[i, j] = 0;

						int indexIndex = (j * size + i) * 6;
						// top left vertex should be last for flat interpolation of tile index
						// 52 - 0 +1
						// | \\ |
						// 4 - 31 +1
						// +r   +r
						_indices[indexIndex + 0] = vertexIndex + 1;     // top right
						_indices[indexIndex + 1] = vertexIndex + r + 1; // bottom right
						_indices[indexIndex + 2] = vertexIndex;         // top left
						_indices[indexIndex + 3] = vertexIndex + r + 1; // bottom right
						_indices[indexIndex + 4] = vertexIndex + r;     // bottom left
						_indices[indexIndex + 5] = vertexIndex;         // top left
					}
				}
			}
		}
	}
}
