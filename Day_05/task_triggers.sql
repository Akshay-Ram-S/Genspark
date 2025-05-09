------------------------- TRIGGERS -----------------------------

-- Write a trigger that logs whenever a new customer is inserted.

create table logs
(log_id serial primary key,
event_type varchar(20),
table_name varchar(20),
msg varchar(500),
event_timestamp timestamptz default current_timestamp
)

create or replace function user_created()
returns trigger 
as $$
begin
	insert into logs(event_type, table_name, msg) 
	values('Insert', 'Customer', 'New User created :' || new.customer_id || ' ' || concat(new.first_name,' ', new.last_name));
	return new;
end
$$ language plpgsql;

create trigger trg_user_create
after insert on customer 
for each row execute function user_created();

insert into customer (store_id,first_name,last_name,address_id,email,activebool,active) 
values (1,'Jack','Tiger',5,'jt@customer.org',true,1)
select * from logs;


-- Create a trigger that prevents inserting a payment of amount 0.

create or replace function check_zero()
returns trigger 
as $$
begin
    if new.amount = 0 then
        raise exception 'Payment should not be zero.';
    end if;
    return new;
end;
$$ 
language plpgsql;

create trigger trg_check_zero
before insert on payment
for each row execute function check_zero()

insert into payment(customer_id,staff_id,rental_id,amount) values (10,2,1520,0)


-- Set up a trigger to automatically set last_update on the film table before update.

create or replace function set_last_update()
returns trigger 
as 
$$
begin
    new.last_update := now();
    return new;
end;
$$ 
language plpgsql;

create trigger trg_last_update
before update on film
for each row execute function set_last_update();

update film set rental_rate = 4.99 where film_id = 133;


-- Create a trigger to log changes in the inventory table (insert/delete).

create table inventory_log (
    log_id serial primary key,
    inventory_id integer,
    operation_type text,
    changed_at timestamp default current_timestamp,
    old_data jsonb,
    new_data jsonb
);

create or replace function log_inventory_changes()
returns trigger 
as $$
begin
    if (tg_op = 'insert') then
        insert into inventory_log (inventory_id, operation_type, new_data)
        values (new.id, 'insert', to_jsonb(new));
        return new;

    elsif (tg_op = 'delete') then
        insert into inventory_log (inventory_id, operation_type, old_data)
        values (old.id, 'delete', to_jsonb(old));
        return old;

    end if;
    return null;
end;
$$ language plpgsql;

create trigger trg_inventory_log
after insert or delete on inventory
for each row execute function log_inventory_changes();


-- Write a trigger that ensures a rental can’t be made for a customer who owes more than $50.

create or replace function check_balance()
returns trigger
as $$
declare
	balance decimal;
begin
	select sum(amount) into balance from payment 
	where customer_id = new.customer_id and payment_date > current_date - interval '30 days';

	if balance > 50 then 
		raise exception 'Rental is not allowed (balance > $50)';
	end if;

	return new;
end;
$$ language plpgsql;

create trigger trg_check_balance
before insert on rental 
for each row execute function check_balance();

