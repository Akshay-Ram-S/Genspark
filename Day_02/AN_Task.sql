-- Create Tables with Integrity Constrains: 
 
-- a) EMP (empno - primary key, empname, salary, deptname - references entries in a deptname of department table with null constraint, bossno - references entries in an empno of emp table with null constraint) 

-- b) DEPARTMENT (deptname - primary key, floor, phone, empno - references entries in an empno of emp table not null) 

-- c) SALES (salesno - primary key, saleqty, itemname -references entries in a itemname of item table with not null constraint, deptname - references entries in a deptname of department table with not null constraint) 

-- d) ITEM (itemname - primary key, itemtype, itemcolor) 

create database training;
use training;

create table ITEM(
	itemName nvarchar(50) primary key,
	itemType nvarchar(50) not null,
	itemColour nvarchar(50)
)

create table DEPARTMENT(
	deptName nvarchar(50) primary key,
	floor int,
	phone varchar(20),
	empNo int not null,
);

create table EMP(
	empNo int primary key,
	empName nvarchar(50),
	salary decimal(12,2),
	deptName nvarchar(50) not null,
	bossNo int not null,

	foreign key (deptName) references DEPARTMENT(deptName),
	foreign key (bossNo) references EMP(empNo)
);

create table SALES(
	salesNo int primary key,
	salesQty int,
	itemName nvarchar(50),
	deptName nvarchar(50),

	foreign key(itemName) references ITEM(itemName),
	foreign key(deptName) references DEPARTMENT(deptName)
 );

alter table DEPARTMENT add constraint fk_bossNo foreign key (empNo) references EMP(empNo);



insert into DEPARTMENT values
('Management', 5, '34', 1),
('Books', 1, '81', 4),
('Clothes', 2, '24', 4),
('Equipment', 3, '57', 3),
('Furniture', 4, '14', 3),
('Navigation', 1, '41', 3),
('Recreation', 2, '29', 4),
('Accounting', 5, '35', 5),
('Purchasing', 5, '36', 7),
('Personnel', 5, '37', 9),
('Marketing', 5, '38', 2);

insert into EMP values 
(1, 'Alice', 75000, 'Management', NULL),
(2, 'Ned', 45000, 'Marketing', 1),
(3, 'Andrew', 25000, 'Marketing', 2),
(4, 'Clare', 22000, 'Marketing', 2),
(5, 'Todd', 38000, 'Accounting', 1),
(6, 'Nancy', 22000, 'Accounting', 5),
(7, 'Brier', 43000, 'Purchasing', 1),
(8, 'Sarah', 56000, 'Purchasing', 7),
(9, 'Sophile', 35000, 'Personnel', 1),
(10, 'Sanjay', 15000, 'Navigation', 3),
(11, 'Rita', 15000, 'Books', 4),
(12, 'Gigi', 16000, 'Clothes', 4),
(13, 'Maggie', 11000, 'Clothes', 4),
(14, 'Paul', 15000, 'Equipment', 3),
(15, 'James', 15000, 'Equipment', 3),
(16, 'Pat', 15000, 'Furniture', 3),
(17, 'Mark', 15000, 'Recreation', 3);

insert into ITEM values
('Pocket Knife-Nile', 'E', 'Brown'),
('Pocket Knife-Avon', 'E', 'Brown'),
('Compass', 'N', NULL),
('Geo positioning system', 'N', NULL),
('Elephant Polo stick', 'R', 'Bamboo'),
('Camel Saddle', 'R', 'Brown'),
('Sextant', 'N', NULL),
('Map Measure', 'N', NULL),
('Boots-snake proof', 'C', 'Green'),
('Pith Helmet', 'C', 'Khaki'),
('Hat-polar Explorer', 'C', 'White'),
('Exploring in 10 Easy Lessons', 'B', NULL),
('Hammock', 'F', 'Khaki'),
('How to win Foreign Friends', 'B', NULL),
('Map case', 'E', 'Brown'),
('Safari Chair', 'F', 'Khaki'),
('Safari cooking kit', 'F', 'Khaki'),
('Stetson', 'C', 'Black'),
('Tent - 2 person', 'F', 'Khaki'),
('Tent -8 person', 'F', 'Khaki');

insert into SALES values
(101, 'Boots-snake proof', 'Clothes'),
(102,  'Pith Helmet', 'Clothes'),
(103,  'Sextant', 'Navigation'),
(104,  'Hat-polar Explorer', 'Clothes'),
(105,  'Pith Helmet', 'Equipment'),
(106,  'Pocket Knife-Nile', 'Clothes'),
(107,  'Pocket Knife-Nile', 'Recreation'),
(108,  'Compass', 'Navigation'),
(109,  'Geo positioning system', 'Navigation'),
(110,  'Map Measure', 'Navigation'),
(111,  'Geo positioning system', 'Books'),
(112,  'Sextant', 'Books'),
(113,  'Pocket Knife-Nile', 'Books'),
(114,  'Pocket Knife-Nile', 'Navigation'),
(115,  'Pocket Knife-Nile', 'Equipment'),
(116,  'Sextant', 'Clothes'),
(117,  'Pocket Knife-Nile', 'Equipment'),
(118,  'Pocket Knife-Nile', 'Recreation'),
(119,  'Pocket Knife-Nile', 'Furniture'),
(120,  'Pocket Knife-Nile', NULL),
(121,  'Exploring in 10 Easy Lessons', 'Books'),
(122,  'How to win Foreign Friends', NULL),
(123,  'Compass', NULL),
(124,  'Pith Helmet', NULL),
(125,  'Elephant Polo stick', 'Recreation'),
(126,  'Camel Saddle', 'Recreation');

select * from EMP;
select * from DEPARTMENT;
select * from ITEM;
select * from SALES;