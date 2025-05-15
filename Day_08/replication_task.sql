/*
Objective:
Create a stored procedure that inserts rental data on the primary server, and verify that changes replicate to the standby server. 
Add a logging mechanism to track each operation.
*/

/*
COMMANDS

initdb -D "C:\Primary"

pg_ctl -D C:\Primary -o "-p 5432" -l C:\Primary\logfile start

psql -p 5432 -d postgres -c "CREATE ROLE replicator with REPLICATION LOGIN PASSWORD 'repl_pass';"

pg_basebackup -D C:\Standby -Fp -Xs -P -R -h 127.0.0.1 -U replicator -p 5432

pg_ctl -D C:\Standby -o "-p 5433" -l C:\Standby\logfile start

psql -p 5432 -d postgres 

(In another cmd - Standby server)

psql -p 5433 -d postgres

*/

-- primary server :

CREATE TABLE rental_log (
    log_id SERIAL PRIMARY KEY,
    rental_time TIMESTAMP,
    customer_id INT,
    film_id INT,
    amount NUMERIC,
    logged_on TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);


CREATE OR REPLACE PROCEDURE sp_add_rental_log(
    p_customer_id INT,
    p_film_id INT,
    p_amount NUMERIC
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO rental_log (rental_time, customer_id, film_id, amount)
    VALUES (CURRENT_TIMESTAMP, p_customer_id, p_film_id, p_amount);
EXCEPTION WHEN OTHERS THEN
    RAISE NOTICE 'Error occurred: %', SQLERRM;
END;
$$;

CALL sp_add_rental_log(1, 100, 4.99);


-- Standby server : 

SELECT * FROM rental_log ORDER BY log_id DESC LIMIT 1;


----------- Add a trigger to log any UPDATE to rental_log ----------------

-- primary server
create table log_rental_log(
	log_rental_id serial primary key,
    operation text,
    old_data jsonb,
    new_data jsonb,
    change_time timestamp default now()
);

create or replace function log_RentalLog()
returns trigger as $$
begin
    if tg_op = 'insert' then
        insert into log_rental_log (operation, old_data, new_data)
        values ('INSERT', null, to_jsonb(new));
        return new;
    elsif tg_op = 'UPDATE' then
        insert into log_rental_log (operation, old_data, new_data)
        values ('update', to_jsonb(old), to_jsonb(new));
        return new;
    elsif tg_op = 'DELETE' then
        insert into log_rental_log (operation, old_data, new_data)
        values ('delete', to_jsonb(old), null);
        return old;
    end if;
    return null;
end;
$$ language plpgsql;

create trigger trg_log_RentalLog
after insert or update or delete on rental_log
for each row execute function log_RentalLog();


CALL sp_add_rental_log(2, 10, 4.99);


-- Standby server (Checking log_rental_log)

select * from log_rental_log;	
------------------------------------------------------------------------------------------










