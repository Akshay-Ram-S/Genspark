namespace Flyweight.models
{
    // Flyweight Factory
    public class CircleFactory
    {
        private static readonly Dictionary<string, Circle> _circles = new Dictionary<string, Circle>();

        public static Circle GetCircle(string color)
        {
            if (!_circles.ContainsKey(color))
            {
                _circles[color] = new Circle(color);
            }
            return _circles[color];
        }
    }
}
