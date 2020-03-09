using Sense.Led;

namespace lab3
{
    public class GarageDoorReading : IReading<bool>
    {
        public Quadrant Quadrant { get; set; }
        public bool Value { get; set; }
        public Color GetColor()
        {
            return Value ? new Color(255, 0, 0) : new Color(0, 255, 0);
        }
    }
}
