select * from customer order by customer_id desc;

create extension if not exists pgcrypto;

-- 1. Create a stored procedure to encrypt a given text
-- Task: Write a stored procedure sp_encrypt_text that takes a plain text input (e.g., email or mobile number) and returns an encrypted version using PostgreSQL's pgcrypto extension.

create or replace procedure sp_encrypt_text(
    plain_text text,       
    encryption_key text,   
    out encrypted_text bytea 
)
language plpgsql as
$$
begin
    encrypted_text := pgp_sym_encrypt(plain_text, encryption_key); 
end;
$$;

do $$
declare
    result bytea;
begin
    call sp_encrypt_text('sample', '102938', result);
    raise notice 'encrypted text: %', result;
end;
$$;


-- 2. Create a stored procedure to compare two encrypted texts
-- Task: Write a procedure sp_compare_encrypted that takes two encrypted values and checks if they decrypt to the same plain text.

create or replace procedure sp_compare_encrypted(
    in encrypt_text1 bytea,
    in encrypt_text2 bytea,
    in secret_key text,
    out is_equal boolean
)
language plpgsql
as $$
declare
    decrypt_text1 text;
    decrypt_text2 text;
begin
    decrypt_text1 := pgp_sym_decrypt(encrypt_text1, secret_key);
    decrypt_text2 := pgp_sym_decrypt(encrypt_text2, secret_key);
    is_equal := (decrypt_text1 = decrypt_text2);
end;
$$;

do $$ 
declare 
    encrypted_text1 bytea;
    encrypted_text2 bytea;
    secret_key text := 'your_secret_key';
    comparison_result boolean;
begin
    encrypted_text1 := pgp_sym_encrypt('Sample', secret_key);
    encrypted_text2 := pgp_sym_encrypt('Sample', secret_key);
    
    call sp_compare_encrypted(encrypted_text1, encrypted_text2, secret_key, comparison_result);
    
    raise notice 'Comparison result: %', comparison_result;
end $$;


-- 3. Create a function to partially mask a given text
-- Task: Write a procedure sp_mask_text that shows only the first 2 and last 2 characters of the input string
-- Masks the rest with *

create or replace procedure sp_mask_text(
    in input_text text,
    out masked_text text
)
language plpgsql
as $$
declare
    length_of_text integer;
    visible_part text;
    masked_part text;
begin
    length_of_text := char_length(input_text);

    if length_of_text <= 4 then
        masked_text := input_text;
    else
        visible_part := concat(substr(input_text, 1, 2), substr(input_text, length_of_text - 1, 2));
        masked_part := repeat('*', length_of_text - 4);
        masked_text := concat(substr(input_text, 1, 2), masked_part, substr(input_text, length_of_text - 1, 2));
    end if;
end;
$$;


do $$ 
declare
    masked_text text;
begin
    call sp_mask_text('masked-text', masked_text);
    raise notice 'masked text: %', masked_text;
end $$;



-- 4) Create a procedure to insert into customer with encrypted email and masked name
-- Task: Call sp_encrypt_text for email, Call fn_mask_text for first_name
-- Insert masked and encrypted values into the customer table
drop procedure proc_insert_customer;

create or replace procedure proc_insert_customer(
	p_store_id int,
	p_first_name text,
	p_last_name text,
	p_email text,
	p_address_id int,
	p_activebool boolean,
	p_create_date date,
	p_active int,
	secret_key text
)
language plpgsql
as $$
declare 
	encrypted_email bytea;
	masked_name text;
begin
	call sp_encrypt_text(p_email,secret_key,encrypted_email);
	call sp_mask_text(p_first_name, masked_name);
	insert into customer (store_id, first_name, last_name, email, address_id, activebool, create_date, active)
	values (p_store_id, masked_name, p_last_name, encrypted_email, p_address_id, p_activebool, p_create_date, p_active);
end $$;

call proc_insert_customer(1,'customer1','customer','123456@cust.org',500,true,current_date,1,'102938');

select * from customer where first_name like '%**%';


-- 5) Create a procedure to fetch and display masked first_name and decrypted email for all customers
-- Task: Write sp_read_customer_masked() that:
-- Loops through all rows, Decrypts email, 
-- Displays customer_id, masked first name, and decrypted email

create or replace procedure sp_read_customer(
    secret_key text
)
language plpgsql
as $$
declare
    rec record;
    v_email_decrypted text;
    cust_cursor cursor for select customer_id, first_name, email from customer order by customer_id desc;
begin
    open cust_cursor;

    loop
        fetch cust_cursor into rec;
        exit when not found;

        begin
            v_email_decrypted := pgp_sym_decrypt(decode(rec.email, 'hex'), secret_key);
			raise notice 'Email %', rec.email;
        exception when others then
            v_email_decrypted := 'Decryption failed';
        end;

        raise notice 'id: %, name: %, email: %', rec.customer_id, rec.first_name, v_email_decrypted;
    end loop;

    close cust_cursor;
end;
$$;

call sp_read_customer('102938');


