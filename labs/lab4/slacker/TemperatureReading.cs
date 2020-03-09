using System;
using Sense.Led;

namespace lab4
{
    public class TemperatureReading : IReading<float>
    {
        public Quadrant Quadrant { get; set; }
        public float Value { get; set; }
        public Color GetColor()
        {
        	float maxTemp = 180f;
			var fahrenheit = (Value * (9 / 5)) + 32f;
			
            return new Color(
            	(byte)((fahrenheit * 255f) / maxTemp),
            	0,
            	(byte)((maxTemp / 255f) * fahrenheit));
        }
    }
}
