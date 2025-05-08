select * from Customers;
select * from orders;
select * from employees;
select * from products;
select * from categories;
select * from suppliers;
select * from orders;
select * from [Order Details];

-- 1) List all orders with the customer name and the employee who handled the order.

select ContactName Customer_Name, concat(FirstName,' ',LastName) Employee_Name from Customers c
join orders o on c.CustomerID = o.CustomerID 
join employees e on o.EmployeeID = e.EmployeeID
order by Customer_Name;

-- 2) Get a list of products along with their category and supplier name.

select ProductName, CategoryName, CompanyName from Products p
join Categories c on p.CategoryID = c.CategoryID 
join Suppliers s on p.SupplierID = s.SupplierID
order by CategoryName;

-- 3) Show all orders and the products included in each order with quantity and unit price.

select o.OrderID, ProductName, Quantity, od.UnitPrice from Orders o 
join [Order Details] od on o.OrderID = od.OrderID
join Products p on od.ProductID = p.ProductID
order by o.OrderID;

-- 4) List employees who report to other employees (manager-subordinate relationship).

select concat(e1.FirstName,' ',e1.LastName)EmployeeName, concat(e2.FirstName,' ',e2.LastName)ReportsTo_Name from Employees e1
join Employees e2 on e1.ReportsTo = e2.EmployeeID
order by EmployeeName;

-- 5) Display each customer and their total order count.

select c.ContactName Customer_Name, count(o.OrderID) as total_count from Customers c
join Orders o on c.CustomerID = o.CustomerID
group by ContactName;

-- 6) Find the average unit price of products per category.

select CategoryName, avg(UnitPrice) Avg_UnitPrice from products p
join Categories c on p.CategoryID = c.CategoryID
group by CategoryName;

-- 7) List customers where the contact title starts with 'Owner'.

select ContactName, ContactTitle from Customers where ContactTitle like 'Owner%'
order by ContactName;

-- 8) Show the top 5 most expensive products.

select top 5 ProductID, ProductName, UnitPrice from Products
order by UnitPrice desc;

-- 9) Return the total sales amount (quantity × unit price) per order.

select OrderID, sum(Quantity * UnitPrice) Total_Amount from [Order Details]
group by OrderID;

-- 10) Create a stored procedure that returns all orders for a given customer ID.

create proc proc_ReturnAllOrders(@CustID varchar(10))
as 
begin
 select OrderID, OrderDate from Orders
 where CustomerID = @CustID;
end

proc_ReturnAllOrders 'KOENE'

-- 11) Write a stored procedure that inserts a new product.

create or alter proc proc_insertProduct(@name nvarchar(50), @sid int, @cid int, @qpu nvarchar(50), @price decimal(10,2), @stock int, @order int, @rl int, @dis bit)
as
begin
	insert into Products (ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued)
	values(@name, @sid, @cid, @qpu, @price, @stock, @order, @rl, @dis);
end

proc_insertProduct 'Product_78',12,7,'10kg',10.50,25,5,10,0

-- 12) Create a stored procedure that returns total sales per employee.

create or alter proc proc_GetTotalSalesPerEmployee
as
begin
    select e.EmployeeID, concat(e.FirstName,' ',e.LastName) EmployeeName, SUM(od.UnitPrice * od.Quantity * (1 - od.Discount)) TotalSales
    from Employees e
    inner join Orders o on e.EmployeeID = o.EmployeeID
    inner join [Order Details] od on o.OrderID = od.OrderID
	group by e.EmployeeID, e.FirstName, e.LastName
    order by TotalSales desc;
end;
proc_GetTotalSalesPerEmployee


-- 13) Use a CTE to rank products by unit price within each category.

select ProductID, ProductName, UnitPrice, CategoryID, ROW_NUMBER() over(partition by CategoryID order by UnitPrice) Rank
from Products;

-- 14) Create a CTE to calculate total revenue per product and filter products with revenue > 10,000. 

with ProductRevenue as (
    select p.ProductName, SUM(od.Quantity * od.UnitPrice * (1 - od.Discount)) total_revenue from [Order Details] od
	join Products p on od.ProductID = p.ProductID
    group by ProductName
)

select ProductName, total_revenue from ProductRevenue
where total_revenue > 10000;

-- 15) Use a CTE with recursion to display employee hierarchy.

with EmployeeHierarchy as (
    select EmployeeID, ReportsTo from Employees
    where ReportsTo IS NULL

    union all

    select e.EmployeeID, e.ReportsTo from Employees e
    inner join EmployeeHierarchy eh on e.ReportsTo = eh.EmployeeID
)

select EmployeeID, ReportsTo from EmployeeHierarchy
order by ReportsTo, EmployeeID;
