using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class HeightGenerator
    {
        private readonly float[] heightLevels;

        public HeightGenerator(float[] heightLevels)
        {
            this.heightLevels = heightLevels;
        }

        public Dictionary<IntVector2, VirtualCell> AddHeights(Dictionary<IntVector2, VirtualCell> map)
        {
            var max = 0.0;
            var min = 1.0;


            foreach (var cell in map)
            {
                if (cell.Value.TypeOfField == VirtualCell.FieldType.NONE)
                {
                    continue;
                }

                var pos = cell.Value.GetXZCoordinates();
                cell.Value.YCoordinate = Mathf.PerlinNoise(pos.x, pos.y);

                if (cell.Value.YCoordinate > max)
                {
                    max = cell.Value.YCoordinate;
                }
                else if (cell.Value.YCoordinate < min)
                {
                    min = cell.Value.YCoordinate;
                }
            }

            var step = (max - min) / heightLevels.Length;
            
            foreach (var cell in map)
            {
                cell.Value.YCoordinate = cell.Value.TypeOfField == VirtualCell.FieldType.NONE ? 0 : heightLevels[(int)(cell.Value.YCoordinate / step) % heightLevels.Length];
            }
            return map;
        }
    }
}
