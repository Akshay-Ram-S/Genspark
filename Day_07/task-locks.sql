-- 1. Try two concurrent updates to same row → see lock in action.
-- Transaction A (Session 1)
begin;
update accounts set balance = 500 where id = 1;

-- Transaction B (Session 2)
begin;
update accounts set balance = 1000 where id = 1; -- Waits until Transaction A commits or rollback.



-- 2. Write a query using SELECT...FOR UPDATE and check how it locks row.
-- Transaction A (Session 1)
begin;
select * from accounts where id = 1 for update;

-- Transaction B (Session 2)
begin;
select * from accounts where id = 1 for update;
-- Above query is blocked until Transaction A commits or rollback.



-- 3. Intentionally create a deadlock and observe PostgreSQL cancel one transaction.
-- Transaction A (Session 1)
begin;
update accounts set balance = balance - 100 where id = 1;

-- Transaction B (Session 2)
begin;
update accounts set balance = balance + 100 where id = 2;

-- Transaction A (Session 1)
begin;
update accounts set balance = balance + 100 where id = 2;

-- Transaction B (Session 2)
begin;
update accounts set balance = balance - 100 where id = 1;



-- 4. Use pg_locks query to monitor active locks.
select pg_advisory_lock(1000);
update accounts set balance = balance - 100 where id = 1;
update accounts set balance = balance + 100 where id = 2;
select pg_advisory_unlock(1000);


-- 5. Explore about Lock Modes.

