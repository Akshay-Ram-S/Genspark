use pubs;

select * from titleauthor;
select * from titles;
select * from publishers;
select * from authors;
select * from sales;
select * from stores;

-- select the author_id for all the books. Print the author_id and the book name
select au_id, title 
from titleauthor join titles 
on titleauthor.title_id = titles.title_id;

select concat(au_fname,' ',au_lname) Author_name, title Book_name from authors a
join titleauthor ta on a.au_id = ta.au_id
join titles t on t.title_id = ta.title_id;

-- Print the publisher's name, book name and the order date of the  books

select pub_name Publisher_Name, title Book_Name, ord_date Order_Date from publishers p
join titles t on p.pub_id = t.pub_id
join sales s on t.title_id = s.title_id
order by 3 desc;

-- Print publisher name and the first book sale date for all the publishers

select pub_name Publisher_Name, min(ord_date) First_order_date from publishers p
left outer join titles t on p.pub_id = t.pub_id
left outer join sales s on t.title_id = s.title_id
group by pub_name
order by 2 desc;

--print the bookname and the store address of the sale

select title Book_Name, stor_address Store_Address from titles t
join sales s on t.title_id = s.title_id
join stores st on s.stor_id = st.stor_id
order by 1;

-- Procedures
go

create procedure proc_FirstProcedure
as
begin
	print 'Hello World'
end

exec proc_FirstProcedure;

create table Products
(id int identity(1,1) constraint pk_productId primary key,
name nvarchar(100) not null,
details nvarchar(max))
go

-- procedure for inserting
create or alter proc proc_InsertProduct(@pname nvarchar(100),@pdetails nvarchar(max))
as
begin
    insert into Products(name, details) values(@pname,@pdetails)
end

proc_InsertProduct 'Laptop','{"brand":"Dell","spec":{"ram":"16GB","cpu":"i5"}}'

select * from Products;

-- Query JSON
select JSON_QUERY(details, '$.spec') Product_Specification from Products;


-- procedure to update JSON value
create or alter proc proc_UpdateProdSpec(@pid int, @newvalue varchar(max))
as
begin
	update Products set details = JSON_MODIFY(details, '$.spec.ram', @newvalue) where id = @pid;
end

proc_UpdateProdSpec 1,'8GB'


-- Query JSON
select id, name, JSON_VALUE(details, '$.brand') Brand from Products;


-- bulk insert for JSON

create table Posts
(id int primary key,
 title nvarchar(100),
 user_id int,
 body nvarchar(max))

 create or alter proc proc_BulkInsertPosts(@jsondata nvarchar(max))
 as
 begin
	insert into Posts(user_id,id,title,body)
	select userId,id,title,body from openjson(@jsondata)
	with (userId int,id int, title nvarchar(100), body nvarchar(max))
 end

  proc_BulkInsertPosts '
[
  {
    "userId": 1,
    "id": 1,
    "title": "sunt aut facere repellat provident occaecati excepturi optio reprehenderit",
    "body": "quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\nreprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt rem eveniet architecto"
  },
  {
    "userId": 1,
    "id": 2,
    "title": "qui est esse",
    "body": "est rerum tempore vitae\nsequi sint nihil reprehenderit dolor beatae ea dolores neque\nfugiat blanditiis voluptate porro vel nihil molestiae ut reiciendis\nqui aperiam non debitis possimus qui neque nisi nulla"
  }]'

-- try_cast for comparison

select * from Products where
try_cast(json_value(details, '$.spec.cpu') as nvarchar(20))='i5';

-- 

create or alter proc proc_PostsByUser(@id int)
as
begin
	select * from Posts where user_id = @id;
end

proc_PostsByUser 1