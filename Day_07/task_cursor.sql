---------- CURSORS ---------------

select * from film;
select * from rental;
select * from inventory;
select * from film_category;
select * from category;

-- Write a cursor to list all customers and how many rentals each made. Insert these into a summary table.

create table rental_summary(
	id serial primary key,
	customer_id int,
	customer_name varchar(50),
	rentals_made int
);

do $$
declare
    customer_cursor cursor for select customer_id, concat(first_name,' ',last_name) name from customer;
    cust_record record;
    rental_count integer;
begin
    open customer_cursor;
    loop
        fetch customer_cursor into cust_record;
        exit when not found;

		select count(*) into rental_count from rental where customer_id = cust_record.customer_id;
		insert into rental_summary (customer_id,customer_name,rentals_made)
		values(cust_record.customer_id,cust_record.name, rental_count);
    end loop;

    close customer_cursor;
end;
$$ language plpgsql;

select * from rental_summary;


-- Using a cursor, print the titles of films in the 'Comedy' category rented more than 10 times.


do $$
declare 
    film_cursor cursor for 
        select film_id from film_category 
        where category_id = (select category_id from category where name = 'Comedy');

    film_rec record;
    rental_count int;
    film_title text;
begin
    open film_cursor; 
    loop
        fetch film_cursor into film_rec;
        exit when not found;

        select f.title, count(*) into film_title, rental_count from film f 
        join inventory i on i.film_id = f.film_id
        join rental r on r.inventory_id = i.inventory_id 
        where f.film_id = film_rec.film_id
        group by f.title;

        if rental_count > 10 then
            raise notice 'title: %, rental count: %', film_title, rental_count;
        end if;
    end loop;
    close film_cursor;
end;
$$ language plpgsql;



-- Create a cursor to go through each store and count the number of distinct films available, and insert results into a report table.

create table store_report(
	report_id serial primary key,
	store_id int,
	distinct_films int
);

do $$
declare
    store_cursor cursor for select store_id from store;
    curr_id record;
    distinct_films int;
begin
    open store_cursor;
    loop
        fetch store_cursor into curr_id;
        exit when not found;

        select count(distinct film_id) into distinct_films
        from inventory where store_id = curr_id.store_id;

		insert into store_report values (curr_id.store_id,distinct_films);
    end loop;
    close store_cursor;
end;
$$ language plpgsql;

select * from store_report;


-- Loop through all customers who haven't rented in the last 6 months and insert their details into an inactive_customers table.

create table inactive_customers(
	inactive_id serial primary key,
	customer_id int,
	customer_name varchar(50)
)

do $$ 
declare
    cur_cursor cursor for 
        select c.customer_id, concat(first_name,' ',last_name) cust_name
        from customer c;
    
    customer_record record;
    last_rental_date date;
begin
    open cur_cursor;
    loop
        fetch cur_cursor into customer_record;
        exit when not found;
		
        select max(rental_date) into last_rental_date from rental 
        where customer_id = customer_record.customer_id;
        
        if last_rental_date is null or last_rental_date < current_date - interval '6 months' then
            insert into inactive_customers (customer_id, customer_name)
            values (customer_record.customer_id, customer_record.cust_name);
        end if;
		
    end loop;
    close cur_cursor;
end; 
$$ language plpgsql;

select * from inactive_customers;



