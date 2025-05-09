select * from film;
select * from customer;
select * from rental;
select * from inventory;
select * from actor;
select * from film_actor;
select * from payment;

-- List all films with their length and rental rate, sorted by length descending.
select title, length, rental_rate from film
order by length desc;

-- Find the top 5 customers who have rented the most films.
select r.customer_id, concat(first_name,' ',last_name) customer_name, count(*) no_of_rentals
from customer c join rental r on c.customer_id = r.customer_id
group by r.customer_id, first_name, last_name
order by no_of_rentals desc limit 5;

-- Display all films that have never been rented.
select title, i.inventory_id from film f
left join inventory i on f.film_id = i.film_id
left join rental r on i.inventory_id = r.inventory_id
where r.inventory_id is null;

-- List all actors who appeared in the film ‘Academy Dinosaur’.
select concat(first_name,' ',last_name) Actor_name from actor a
join film_actor fa on a.actor_id = fa.actor_id
join film f on fa.film_id = f.film_id where f.title = 'Academy Dinosaur';

-- List each customer along with the total number of rentals they made and the total amount paid.
select concat(first_name,' ',last_name) Customer_name, count(*) Total_rentals, Sum(p.amount) Total_amount from customer c
join rental r on c.customer_id = r.customer_id
join payment p on r.rental_id = p.rental_id
group by Customer_Name
order by Total_amount desc;

-- Using a CTE, show the top 3 rented movies by number of rentals.
with cte_rentals
as
(select f.title, count(*) rental_count from film f
join inventory i on f.film_id = i.film_id
join rental r on i.inventory_id = r.inventory_id
group by f.title)

select * from cte_rentals order by rental_count desc limit 3;

-- Find customers who have rented more than the average number of films.
with cte_AvgRental 
as 
(select c.customer_id, concat(first_name,' ',last_name) customer_name, count(*) as rental_count, avg(count(*)) over() avg_rental from customer c 
join rental r on c.customer_id = r.customer_id
group by c.customer_id, customer_name)

select * from cte_AvgRental where rental_count > avg_rental;

-- Write a function that returns the total number of rentals for a given customer ID.
 CREATE OR REPLACE FUNCTION get_total_rentals(cust_id INT) 
 RETURNS INT AS 
 $$ 
 DECLARE total_rentals INT; 
 BEGIN 
 	SELECT COUNT(*) INTO total_rentals FROM rental WHERE customer_id = cust_id; 
 	RETURN total_rentals; 
 END; 
 $$ 
 LANGUAGE plpgsql

 select get_total_rentals(2)

-- Write a stored procedure that updates the rental rate of a film by film ID and new rate.
create or replace procedure update_rental_rate(filmID int, new_rate NUMERIC)
language plpgsql
as $$
begin
    update film set rental_rate = new_rate where film_id = filmID;
end
$$;

call update_rental_rate(1,1.10)

-- Write a procedure to list overdue rentals (return date is NULL and rental date older than 7 days).

create or replace procedure get_overdue_rentals()
language plpgsql
as $$
begin
    create temp table overdue_rentals as
    select r.rental_id, r.customer_id, r.rental_date, r.return_date
    from rental r
    where r.return_date is null
      and r.rental_date < current_date - interval '7 days';
end;
$$;

call get_overdue_rentals();
select * from overdue_rentals;


-----



