/*
1) In a transaction, if I perform multiple updates and an error happens in the third statement, but I have not used SAVEPOINT, what will happen if I issue a ROLLBACK?
   Will my first two updates persist?

-> No, the first two updates will not persist. 
   If we issue a ROLLBACK it will go to last commit state (Before the start of transaction) as we didn't use SAVEPOINT.
*/
begin;
update accounts set balance = balance - 500 where id = 1;
update accounts set balance = balance + 500 where id = 2;
update account set balance = balance + 10 where id = 1; -- error statement
rollback; -- all the above changes are undone


/*
2) Suppose Transaction A updates Alice’s balance but does not commit. Can Transaction B read the new balance if the isolation level is set to READ COMMITTED?

-> In the READ COMMITTED isolation level, Transaction B will not be able to read the new balance set by Transaction A. 
   This is because Transaction A has not committed yet.
*/
-- Transaction A
begin;
update accounts set balance = 100 where id = 1;

-- Transaction B
set transaction isolation level read committed;
select balance from account where id = 1; -- shows the previous commit balance value



/*
3) What will happen if two concurrent transactions both execute:
   UPDATE tbl_bank_accounts SET balance = balance - 100 WHERE account_name = 'Alice';
   at the same time? Will one overwrite the other?

-> No, they will not overwrite each other. 
   Since default isolation level is "Read Committed", one of the transaction will wait for the other transaction to complete. 
*/


/* 
4) If I issue ROLLBACK TO SAVEPOINT after_alice;, will it only undo changes made after the savepoint or everything?

-> It will undo the changes made after the savepoint not everything in the transaction.
*/
begin;
update accounts set balance = balance - 100 where name = 'Alice';
savepoint after_alice;
update account set balance = balance - 100 where name = 'Bob';
rollback to savepoint after_alice; -- undo changes after the savepoint in the transaction


/*
5) Which isolation level in PostgreSQL prevents phantom reads?

-> In PostgreSQL, serializable isolation level prevents phantom read.
*/


/*
6) Can Postgres perform a dirty read (reading uncommitted data from another transaction)?

-> No, PostgreSQL cannot perform a dirty read because "Read Uncommitted" isolation level is not present in PostgreSQL.
   The default isolation level is "Read Committed".
*/


/*
7) If autocommit is ON (default in Postgres), and I execute an UPDATE, is it safe to assume the change is immediately committed?

-> Yes, the changes made from an update statement is immediately committed if the update statement executes successfully.
*/
update accounts set balance = balance + 1000 where id = 1; -- update is committed


/*
8) If I do this:

   BEGIN;
   UPDATE accounts SET balance = balance - 500 WHERE id = 1;
   -- (No COMMIT yet)
   And from another session, I run:

   SELECT balance FROM accounts WHERE id = 1;
   Will the second session see the deducted balance?

-> No, the second session will not see the deducted balance due to transaction isolation (Read Committ by default).
   It will only see the last committed value of balance.
*/