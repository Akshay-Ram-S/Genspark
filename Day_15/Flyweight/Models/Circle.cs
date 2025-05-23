namespace Flyweight.models
{
    // Flyweight class
    public class Circle
    {
        public string Color { get; }

        public Circle(string color)
        {
            Color = color;
        }

        public void Draw(int x, int y)
        {
            Console.WriteLine($"Drawing {Color} Circle at ({x}, {y})");
        }
    }
}
