using Factory.interfaces;

namespace Factory.models
{
    public class Circle : IShape
    {
        public void Draw() => Console.WriteLine("Drawing a Circle!");
    }
}