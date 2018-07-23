﻿using System.Collections;
using System;
using UnityEngine;

namespace Voxel.Core {
	public class VxlReader{
		public VxlReader()
		{
			Width = 512;           
			Height = 512;           
			Depth = 64;          
		}

			public int Width { get; set; }
			public int Height { get; set; }           
			public int Depth { get; set; }                 

			public Map LoadMap(byte[] bytes)
			{
				int w = Width, h = Height, d = Depth;
				var map = new Map(w, h, d);
				var r = new System.Random();

				int pos = 0;

				Debug.Log("Reading voxels");

				for (int y = 0; y < h; ++y)
				{
					for (int x = 0; x < w; ++x)
					{
						int z = 0;
						uint color = 0;

						for (; z < d; ++z)
						{
							uint col = 0x284067;
							col ^= 0x070707 & (uint) r.Next();
							map[x, y, z] = col;
						}

						z = 0;
						while (true)
						{
							int number_4byte_chunks = bytes[pos];
							int top_color_start = bytes[pos + 1];
							int top_color_end = bytes[pos + 2];
							int bottom_color_start;
							int bottom_color_end;
							int len_top;
							int len_bottom;

							for (; z < top_color_start; ++z)
							{
								map[x, y, z] = Map.EmptyVoxel;
							}

							int colorpos = pos + 4;
							for (; z <= top_color_end; z++)
							{
								color = BitConverter.ToUInt32(bytes, colorpos);
								colorpos += 4;
								map[x, y, z] = color;
							}

							if (top_color_end == d - 2)
							{
								map[x, y, d - 1] = map[x, y, d - 2];
							}

							len_bottom = top_color_end - top_color_start + 1;

							if (number_4byte_chunks == 0)
							{
								pos += 4 * (len_bottom + 1);
								break;
							}

							len_top = (number_4byte_chunks - 1) - len_bottom;

							pos += (int) bytes[pos] * 4;

							bottom_color_end = bytes[pos + 3];
							bottom_color_start = bottom_color_end - len_top;

							for (z = bottom_color_start; z < bottom_color_end; z++)
							{
								color = BitConverter.ToUInt32(bytes, colorpos);
								colorpos += 4;
								map[x, y, z] = color;
							}
							if (bottom_color_end == d - 1)
							{
								map[x, y, d - 1] = map[x, y, d - 2];
							}
						}
					}
				}
			return map;
		}
	}
}
