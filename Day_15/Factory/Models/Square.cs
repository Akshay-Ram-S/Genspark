using Factory.interfaces;

namespace Factory.models
{
    public class Square : IShape
    {
        public void Draw() => Console.WriteLine("Drawing a Square!");
    }
}