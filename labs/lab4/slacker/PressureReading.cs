using Sense.Led;

namespace lab4
{
    public class PressureReading : IReading<float>
    {
        public Quadrant Quadrant { get; set; }
        public float Value { get; set; }
        public Color GetColor()
        {
            return Value > 900f ? new Color(255, 0, 0) : new Color(0, 255, 0);
        }
    }
}
