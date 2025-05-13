------------ TRIGGERS ----------------

-- Create a trigger to prevent inserting payments of zero or negative amount.

create or replace function check_amount()
returns trigger 
as $$
begin
    if new.amount <= 0 then
        raise exception 'Payment should be more than zero.';
    end if;
    return new;
end;
$$ language plpgsql;

create trigger trg_check_amount
before insert on payment
for each row execute function check_amount();

insert into payment(customer_id,staff_id,rental_id,amount) values (10,2,1520,-2)


-- Set up a trigger that automatically updates last_update on the film table when the title or rental rate is changed.

create or replace function update_lastUpdate()
returns trigger
as $$
begin
	new.last_update := now();
	return new;
end;
$$ language plpgsql;

create trigger trg_update_lastUpdate
before update of title, rental_rate on film
for each row execute function update_lastUpdate();

update film set rental_rate = 5.99 where film_id = 2;


-- Write a trigger that inserts a log into rental_log whenever a film is rented more than 3 times in a week.

create table rental_log(
	log_id serial primary key,
	film_id int,
	title varchar(255),
	rental_count int,
	log_date timestamp default current_timestamp
);


create or replace function log_rental_count()
returns trigger 
as $$
declare
    rental_count int;
	f_id int;
	film_title text;
begin
    select count(*) into rental_count from rental 
	where inventory_id = new.inventory_id and rental_date >= current_date - interval '7 days';

	select f.film_id, title into f_id, film_title from film f
	join inventory i on f.film_id = i.film_id
	where f.film_id = new.inventory_id;
	
    if rental_count > 3 then
        insert into rental_log (film_id,title,rental_count)
        values (f_id,film_title, rental_count);
    end if;

    return new;
end;
$$ language plpgsql;

create trigger trg_log_rental_count
after insert on rental
for each row execute function log_rental_count();

insert into rental(rental_date, inventory_id, customer_id, staff_id)
values (now(), 100, 215, 1);

select * from rental_log;
