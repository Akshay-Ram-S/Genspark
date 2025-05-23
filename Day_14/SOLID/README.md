## SOLID Principles

- ***S - Single Responsibility Principle*** <br>
- ***O - Open/Closed Principle*** <br>
- ***L - Liskov Substituion Principle*** <br>
- ***I - Interface Segregation Principle*** <br>
- ***D - Dependency Inversion*** <br>
<br>

![image](Images/Solid-Principles.png)
<br>
<br>
The program in the file follows all the SOLID principles.
<br>

**S - Single Repsonsibilty Principle**
    An object should only have one reason to change.

Why factor?
- When a class has only one responsibility, changes related to that responsibility won’t unintentionally affect other unrelated logic. <br>
- Improves object management in heap memory. <br>

**O - Open Closed Principle**
    Open for extension, closed for modification.

Why factor?
- To promote code reusability and prevent regressions. <br>

**L - Liskov Substitution Principle**
    Passing an object’s inheritor in place of the base class shouldn’t break 
    any existing functionality in the called method.

Why factor?
- Ensures Code Reliability and Correctness.
- Polymorphism

**I - Interface Segregation Principle**
    Each interface should have a specific purpose.

Why factor?
- To avoid bloated interfaces.

**D - Dependency Inversion Principle**
    Dependency Inversion means to depend on abstractions instead of concrete types.

Why factor?
- Promotes decoupling of code<br>

**Example folder shows the good and bad practices of SOLID principles.** <br>
**Each principle is shown in different file. It got nothing to do with the execution of main program.**
<br>

**By following the SOLID principles and using repository pattern in C#, our main program is executed.**
