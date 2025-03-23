-- Find open Txn to Kill
SELECT pid, state, query, age(clock_timestamp(), xact_start) AS txn_duration
FROM pg_stat_activity
WHERE state IN ('active', 'idle in transaction', 'idle in transaction (aborted)');

-- Kill open Txn
SELECT pg_terminate_backend(731);
