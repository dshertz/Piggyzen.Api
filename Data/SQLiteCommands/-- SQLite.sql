-- SQLite
UPDATE Transactions
SET HasSimilar = 0;

DELETE FROM Transactions;
DELETE FROM sqlite_sequence WHERE name = 'Transactions';

SELECT DISTINCT CategoryId
FROM CategorizationHistory
WHERE CategoryId NOT IN (SELECT Id FROM Categories);

DELETE FROM CategorizationHistory
WHERE CategoryId NOT IN (SELECT Id FROM Categories);