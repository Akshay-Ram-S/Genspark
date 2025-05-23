using System;
using System.Collections.Generic;
using Factory.interfaces;
using Factory.models;

class Program
{
    static void Main()
    {
        IShape shape1 = ShapeFactory.GetShape("circle");
        shape1.Draw(); 

        IShape shape2 = ShapeFactory.GetShape("square");
        shape2.Draw(); 
    }
}