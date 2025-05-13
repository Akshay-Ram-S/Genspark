------------ TRANSACTIONS ----------------------

-- Write a transaction that inserts a new customer, adds their rental, and logs the payment – all atomically.
select * from customer;
select * from rental where rental_id =-1;
select * from payment;

begin;

do $$
declare
	cust_id int;
	rent_id int;
begin
	insert into customer(first_name, last_name, email, address_id, active, create_date, store_id)
	values('Alex','Grey','alex@cust.org',1,1,now(),1) 
	returning customer_id into cust_id;
	
	insert into rental(rental_date, inventory_id, customer_id, return_date, staff_id)
	values(now(), 3, cust_id, null, 1)
	returning rental_id into rent_id;

	insert into payment(customer_id, staff_id, rental_id, amount, payment_date)
	values(cust_id, 2, rent_id, 4.99, now());

end $$;
commit;


-- Simulate a transaction where one update fails (e.g., invalid rental ID), and ensure the entire transaction rolls back.

begin;

update rental set staff_id = 2 where rental_id = 2;
update rental set staff_id = 1 where rental_id = 4; 
update rental set staff_id = 'two' where rental_id = -1; -- Error 
		
rollback;
	

-- Use SAVEPOINT to update multiple payment amounts. Roll back only one payment update using ROLLBACK TO SAVEPOINT.

begin; 

update payment set amount = 2.99 where payment_id = 17504;
savepoint payment_update1;

update payment set amount = 6.99 where payment_id = 17505;
savepoint payment_update2;

update payment set amount = 5.99 where payment_id = 17506;
savepoint payment_update3;

update payment set amount = 8.99 where payment_id = 17507;
savepoint payment_update4;

rollback to savepoint payment_update3;


-- Perform a transaction that transfers inventory from one store to another safely.


begin transaction;

do $$
declare
    current_store_id integer;
	new_store_id = 2;
begin
    select store_id into current_store_id from inventory
    where inventory_id = 4;

    if current_store_id = new_store_id then
        raise 'exception'
	end if;
	
	update inventory set store_id = 1 where inventory_id = 4;

end $$;
commit;


-- Create a transaction that deletes a customer and all associated records (rental, payment), ensuring referential integrity.

begin;

delete from payment where customer_id = 341;

delete from rental where customer_id = 341;

delete from customer where customer_id = 341;

commit;