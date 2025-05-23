using Factory.interfaces;

namespace Factory.models
{
    public class ShapeFactory
    {
        private static readonly Dictionary<string, Func<IShape>> shapeRegistry =
            new Dictionary<string, Func<IShape>>
            {
                { "circle", () => new Circle() },
                { "square", () => new Square() }
            };

        public static IShape GetShape(string shapeType)
        {
            if (shapeRegistry.TryGetValue(shapeType.ToLower(), out var shapeCreator))
            {
                return shapeCreator();
            }
            else
            {
                throw new ArgumentException("Invalid shape type");
            }
        }
    }    
}
