using Sense.Led;

namespace lab3
{
    public class HumidityReading : IReading<float>
    {
        public Quadrant Quadrant { get; set; }
        public float Value { get; set; }
        public Color GetColor()
        {
            return Value > 60 ? new Color(255, 0, 0) : new Color(0, 0, 255);
        }
    }
}
