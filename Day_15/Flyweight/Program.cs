using System;
using System.Collections.Generic;

using Flyweight.models;



class Program
{
    static void Main()
    {
        Circle redCircle1 = CircleFactory.GetCircle("Red");
        redCircle1.Draw(5, 10);

        Circle redCircle2 = CircleFactory.GetCircle("Red"); // Reuses the existing Red Circle
        redCircle2.Draw(15, 20);

        Circle blueCircle = CircleFactory.GetCircle("Blue"); // New Blue Circle
        blueCircle.Draw(30, 40);

        Console.WriteLine(ReferenceEquals(redCircle1, redCircle2));
    }
}