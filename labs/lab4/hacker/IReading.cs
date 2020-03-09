using Sense.Led;

namespace lab4
{
    public interface IReading<T>
    {
        Quadrant Quadrant { get; set; }
        T Value { get; set; }
        Color GetColor();
    }
}
