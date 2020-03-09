using System;
using Sense.Led;

namespace lab4
{
    public class HumidityReading : IReading<float>
    {
        public Quadrant Quadrant { get; set; }
        public float Value { get; set; }
        public Color GetColor()
        {
            var color = new Color(
            	(byte)(Value * 150f / 100f),
            	(byte)(Value * 75f / 100f),
            	(byte)(Value * 255f / 100f));

            return color;
        }
    }
}
