-------- CURSORS ----------

-- Write a cursor that loops through all films and prints titles longer than 120 minutes.

do $$
declare 
	film_cursor cursor for 
	select title from film where length > 120;
	film_row record;
begin
	open film_cursor;
	loop
		fetch film_cursor into film_row;
		exit when not found;
		raise notice 'Title : %', film_row.title;
	end loop;
	close film_cursor;
end $$	


-- Create a cursor that iterates through all customers and counts how many rentals each made.

create or replace function count_customer_rentals()
returns setof text as $$
declare
    customer_cursor cursor for select customer_id from customer;
    curr_id integer;
    rental_count integer;
begin
    open customer_cursor;
    loop
        fetch customer_cursor into curr_id;
        exit when not found;
        select count(*) into rental_count from rental
        where customer_id = curr_id;
        return next 'Customer ID : ' || curr_id || ', Total Rental : ' || rental_count;
    end loop;

    close customer_cursor;
end;
$$ language plpgsql;

select * from count_customer_rentals();


-- Using a cursor, update rental rates: Increase rental rate by $1 for films with less than 5 rentals.

do $$
declare
    film_cursor cursor for
    select f.film_id from film f
    join inventory i on f.film_id = i.film_id
    join rental r on i.inventory_id = r.inventory_id
    group by f.film_id
    having count(r.rental_id) < 5;
	film_row record;

begin
    open film_cursor;
    loop
		fetch film_cursor into film_row;
		exit when not found;
        update film set rental_rate = rental_rate + 1 where film_id = film_row.film_id;
    end loop;
    close film_cursor;
end $$;


-- Create a function using a cursor that collects titles of all films from a particular category.

create or replace function get_films_by_category(category text)
returns setof text as $$
declare
    film_cursor cursor for
    select f.title from film f 
	join film_category fc on f.film_id = fc.film_id
	join category c on fc.category_id = c.category_id
	where c.name = category;
	film_row record;
begin
    open film_cursor;
    loop
        fetch film_cursor into film_row;
        exit when not found;
        return next film_row.title;
    end loop;
    close film_cursor;
    return;
end;
$$ language plpgsql;

select * from get_films_by_category('Action');


-- Loop through all stores and count how many distinct films are available in each store using a cursor.

create or replace function count_distinct_films()
returns setof text as $$
declare
    store_cursor cursor for select store_id from store;
    rec record;
    distinct_films int;
begin
    open store_cursor;
    loop
        fetch store_cursor into rec;
        exit when not found;

        select count(distinct film_id) into distinct_films
        from inventory where store_id = rec.store_id;

        return next 'store id: ' || rec.store_id || ', distinct films: ' || distinct_films;
    end loop;
    close store_cursor;
end;
$$ language plpgsql;

select * from count_distinct_films();





