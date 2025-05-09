------ Transaction -------

select * from film;
select * from rental;
select * from inventory;
select * from payment;
select * from customer;

-- Write a transaction that inserts a customer and an initial rental in one atomic operation.

begin transaction;

do $$
declare cust_id int;
begin
	insert into customer(first_name, last_name, email, address_id, active, create_date, store_id)
	values ('Red','Green','abc@cust.org',1,1,now(),1) 
	returning customer_id into cust_id;
	
	insert into rental(rental_date, inventory_id, customer_id, return_date, staff_id)
	values (now(), 3, cust_id, null, 1);
end $$;

commit;


-- Simulate a failure in a multi-step transaction (update film + insert into inventory) and roll back.


begin transaction;

savepoint before_update;

update film set release_year = 2007 where film_id = 8;
insert into inventory (film_id, store_id)
values (1, 'three'); -- invalid type for store_id  

rollback to savepoint before_update;


-- Create a transaction that transfers an inventory item from one store to another.
select * from inventory;

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


-- Demonstrate SAVEPOINT and ROLLBACK TO SAVEPOINT by updating payment amounts, then undoing one.

begin transaction;

update payment set amount = 6.99 where payment_id = 17503;
savepoint before_second_update;
update payment set amount = 10.99 where payment_id = 17504;
rollback to savepoint before_second_update;

commit;

select * from payment where payment_id in (17503, 17504);


-- Write a transaction that deletes a customer and all associated rentals and payments, ensuring atomicity.

begin transaction;

do $$
declare
    max_customer_id int;
begin
    select max(customer_id) into max_customer_id from customer;
    delete from payment where customer_id = max_customer_id;
    delete from rental where customer_id = max_customer_id;
    delete from customer where customer_id = max_customer_id;
end $$;

commit;


