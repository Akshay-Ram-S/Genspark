/*
Tables to Design (Normalized to 3NF):

1. **students**

   * `student_id (PK)`, `name`, `email`, `phone`

2. **courses**

   * `course_id (PK)`, `course_name`, `category`, `duration_days`

3. **trainers**

   * `trainer_id (PK)`, `trainer_name`, `expertise`

4. **enrollmentsnrollment**

   * `enrollment_id (PK)`, `student_id (FK)`, `course_id (FK)`, `enroll_date`

5. **certificates**

   * `certificate_id (PK)`, `enrollment_id (FK)`, `issue_date`, `serial_no`

6. **course\_trainers** (Many-to-Many if needed)

   * `course_id`, `trainer_id`

*/

create table students(
	student_id serial primary key,
	student_name varchar(100) not null,
	email varchar(50) unique not null,
	phone varchar(20) unique not null
);

create table trainers(
	trainer_id serial primary key,
	trainer_name varchar(100) not null,
	email varchar(50) unique not null,
	phone varchar(20) unique not null,
	expertise varchar(30) not null
);

create table courses(
	course_id serial primary key,
	course_name varchar(100) not null,
	trainer_id int not null references trainers(trainer_id), 
	description text,
	category varchar(30) not null
);

create table enrollments(
	enrollment_id serial primary key,
	student_id int not null references students(student_id),
	course_id int not null references courses(course_id),
	enroll_date timestamp default current_timestamp
);

create table certificates(
	certificate_id serial primary key,
	serial_number text unique not null,
	enrollment_id int unique not null references enrollments(enrollment_id),
	issue_date timestamp default current_timestamp
);

create table course_trainer(
	course_id int not null references courses(course_id),
	trainer_id int not null references trainers(trainer_id),
	primary key(course_id, trainer_id)
)

-------------------------- PHASE 2 : DDL & DML -----------------------------------------------
/*
* Create all tables with appropriate constraints (PK, FK, UNIQUE, NOT NULL)
* Insert sample data using `INSERT` statements
* Create indexes on `student_id`, `email`, and `course_id`
*/
-- Insert into students
insert into students(student_name, email, phone)
values('Ramu','ramu@std.org','9999999999');

insert into students(student_name, email, phone)
values('Somu','somu@std.org','9999999900');

insert into students(student_name, email, phone)
values('Jack','jack@std.org','9999912999');

select * from students;

-- Insert into trainers
insert into trainers(trainer_name, email, phone, expertise)
values('Trainer1','1@tra.org','9999999923','RDBMS');

insert into trainers(trainer_name, email, phone, expertise)
values('Trainer2','2@tra.org','9910999923','Python');

select * from trainers;

-- Insert into courses
insert into courses(course_name, trainer_id, description, category)
values('SQL',1,'Covers basics of SQL','RDBMS');

insert into courses(course_name, trainer_id, description, category)
values('PostgreSQL',1,'Covers basics of PostgreSQL','RDBMS');

insert into courses(course_name, trainer_id, description, category)
values('Python Basics',2,'Covers basics of Python','Python');

insert into courses(course_name, trainer_id, description, category)
values('Python Intermediate',2,'Covers intermediate concepts of Python','Python');

select * from courses;

-- Insert into enrollments
insert into enrollments(student_id, course_id)
values(1,3);

insert into enrollments(student_id, course_id)
values(1,4);

insert into enrollments(student_id, course_id)
values(2,1);

insert into enrollments(student_id, course_id)
values(2,4);

insert into enrollments(student_id, course_id)
values(1,1);

select * from enrollments;

-- Insert into certificates
insert into certificates(serial_number, enrollment_id)
values ('1234567890',1);

insert into certificates(serial_number, enrollment_id)
values('1234567891',2);

insert into certificates(serial_number, enrollment_id)
values('1234567892',4);

select * from certificates;

-- Creating indexes
create index idx_students_student_id on students(student_id);

create index idx_students_email on students(email);

create index idx_courses_course_id on courses(course_id);



------------------ PHASE 3 : SQL Joins Practice -----------------------------------

-- 1. List students and the courses they enrolled in
select s.student_id, s.student_name, c.course_name from enrollments e
join students s on s.student_id = e.student_id
join courses c on c.course_id = e.course_id;

-- 2. Show students who received certificates with trainer names
select ce.certificate_id, ce.serial_number, s.student_id, s.student_name, t.trainer_name from certificates ce
join enrollments e on ce.enrollment_id = e.enrollment_id
join students s on e.student_id = s.student_id
join courses c on e.course_id = c.course_id
join trainers t on c.trainer_id = t.trainer_id;

-- 3. Count number of students per course
select c.course_id, c.course_name, count(e.course_id) Total_Count from courses c
left join enrollments e on e.course_id = c.course_id
group by c.course_id, c.course_name;



--------------------- PHASE 4 : Functions & Stored Procedures ----------------------------

-- Create function `get_certified_students(course_id INT)`
-- → Returns a list of students who completed the given course and received certificates.

create or replace function get_certified_students(p_course_id int)
returns table (
	student_id int,
	student_name varchar(100)
)
as $$
begin
	return query
	select s.student_id, s.student_name from certificates c
	join enrollments e on c.enrollment_id = e.enrollment_id
	join students s on e.student_id = s.student_id
	where e.course_id = p_course_id;

	exception when others then
		raise notice 'Error : %', sqlerrm;
end;
$$ language plpgsql;

select * from get_certified_students(3);

-- Create stored procedure `sp_enroll_student(p_student_id, p_course_id)`
-- → Inserts into `enrollments` and conditionally adds a certificate if completed (simulate with status flag).

create or replace procedure sp_enroll_student(p_student_id int, p_course_id int, completed boolean)
language plpgsql
as $$ 
declare
enroll_id int;
begin
	insert into enrollments(student_id, course_id)
	values(p_student_id,p_course_id)
	returning enrollment_id into enroll_id;
	if completed then
		insert into certificates(serial_number, enrollment_id)
		values('123456789',enroll_id);
	end if;
	exception when others then
		raise notice 'Error : %',sqlerrm;
end $$;

call sp_enroll_student(3, 2, true);

call sp_enroll_student(3, 4, false);



-------------------------- PHASE 5 : Cursor --------------------------------------------------

-- Use a cursor to:
-- Loop through all students in a course
-- Print name and email of those who do not yet have certificates

create or replace procedure sp_stud_without_certificates(p_course_id int)
language plpgsql
as $$
declare
    rec record;
    cursor_students cursor for
        select s.student_id, s.student_name, s.email, e.enrollment_id from students s
        join enrollments e on s.student_id = e.student_id
        where e.course_id = p_course_id;
begin
    open cursor_students;
    
    loop
        fetch cursor_students into rec;
        exit when not found;
        
        if not exists(select 1 from certificates c where c.enrollment_id = rec.enrollment_id) then
            raise notice 'Student Name = %, Email = %', rec.student_name, rec.email;
        end if;
    end loop;

    close cursor_students;
end;
$$;

call sp_stud_without_certificates(1);



------------------- Phase 6: Security & Roles ----------------------------------------

/*
1. Create a `readonly_user` role:
- Can run `SELECT` on `students`, `courses`, and `certificates`
- Cannot `INSERT`, `UPDATE`, or `DELETE`
*/

create role readonly_user with login password 'readonly';

grant usage on schema public to readonly_user;
grant select on students, courses, certificates to readonly_user;


/*
2. Create a `data_entry_user` role:
- Can `INSERT` into `students`, `enrollments`
- Cannot modify certificates directly
*/

create role data_entry_user with login password 'dataentry';

grant usage on schema public to data_entry_user;
grant insert on students to data_entry_user;
grant insert on enrollments to data_entry_user;

revoke all on certificates from data_entry_user;



---------------- Phase 7: Transactions & Atomicity ----------------------------------------------------
/*
Write a transaction block that:
- Enrolls a student
- Issues a certificate
- Fails if certificate generation fails (rollback)
*/

create or replace procedure sp_enroll_certificate(
	p_student_id int, 
	p_course_id int,
	p_serial_number text
)
language plpgsql
as $$
declare
enroll_id int;
begin
	begin
	insert into enrollments (student_id, course_id)
	values (p_student_id, p_course_id)
	returning enrollment_id into enroll_id;
	insert into certificates (serial_number, enrollment_id)
	values (p_serial_number, enroll_id);
	
	exception when others then
		raise notice 'Error : Certification generation failed, Details : %',sqlerrm;
		
	end;
end $$;

call sp_enroll_certificate(2,3,'1234567893');
call sp_enroll_certificate(2,3,'1234567893');

-------------------------------------------------------------------------------------------------------------------------

