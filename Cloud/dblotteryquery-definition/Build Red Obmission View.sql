WITH indeicedTable AS
(
	SELECT *, ROW_NUMBER() OVER (ORDER BY Issue) RowInx
	FROM dbo.Basic
)

SELECT Issue, RowInx, 
	(CASE WHEN (Red1 =   1 OR Red2 =   1 OR Red3 =   1 OR Red4 =   1 OR Red5 =  1 OR Red6 =   1) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =   1 OR m2.Red2 =   1 OR m2.Red3 =   1 OR m2.Red4 =   1 OR m2.Red5 =  1 OR m2.Red6 =   1)), 0)) END) AS '01',
	(CASE WHEN (Red1 =   2 OR Red2 =   2 OR Red3 =   2 OR Red4 =   2 OR Red5 =  2 OR Red6 =   2) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =   2 OR m2.Red2 =   2 OR m2.Red3 =   2 OR m2.Red4 =   2 OR m2.Red5 =  2 OR m2.Red6 =   2)), 0)) END) AS '02',
	(CASE WHEN (Red1 =   3 OR Red2 =   3 OR Red3 =   3 OR Red4 =   3 OR Red5 =  3 OR Red6 =   3) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =   3 OR m2.Red2 =   3 OR m2.Red3 =   3 OR m2.Red4 =   3 OR m2.Red5 =  3 OR m2.Red6 =   3)), 0)) END) AS '03',
	(CASE WHEN (Red1 =   4 OR Red2 =   4 OR Red3 =   4 OR Red4 =   4 OR Red5 =  4 OR Red6 =   4) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =   4 OR m2.Red2 =   4 OR m2.Red3 =   4 OR m2.Red4 =   4 OR m2.Red5 =  4 OR m2.Red6 =   4)), 0)) END) AS '04',
	(CASE WHEN (Red1 =   5 OR Red2 =   5 OR Red3 =   5 OR Red4 =   5 OR Red5 =  5 OR Red6 =   5) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =   5 OR m2.Red2 =   5 OR m2.Red3 =   5 OR m2.Red4 =   5 OR m2.Red5 =  5 OR m2.Red6 =   5)), 0)) END) AS '05',
	(CASE WHEN (Red1 =   6 OR Red2 =   6 OR Red3 =   6 OR Red4 =   6 OR Red5 =  6 OR Red6 =   6) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =   6 OR m2.Red2 =   6 OR m2.Red3 =   6 OR m2.Red4 =   6 OR m2.Red5 =  6 OR m2.Red6 =   6)), 0)) END) AS '06',
	(CASE WHEN (Red1 =   7 OR Red2 =   7 OR Red3 =   7 OR Red4 =   7 OR Red5 =  7 OR Red6 =   7) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =   7 OR m2.Red2 =   7 OR m2.Red3 =   7 OR m2.Red4 =   7 OR m2.Red5 =  7 OR m2.Red6 =   7)), 0)) END) AS '07',
	(CASE WHEN (Red1 =   8 OR Red2 =   8 OR Red3 =   8 OR Red4 =   8 OR Red5 =  8 OR Red6 =   8) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =   8 OR m2.Red2 =   8 OR m2.Red3 =   8 OR m2.Red4 =   8 OR m2.Red5 =  8 OR m2.Red6 =   8)), 0)) END) AS '08',
	(CASE WHEN (Red1 =   9 OR Red2 =   9 OR Red3 =   9 OR Red4 =   9 OR Red5 =  9 OR Red6 =   9) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =   9 OR m2.Red2 =   9 OR m2.Red3 =   9 OR m2.Red4 =   9 OR m2.Red5 =  9 OR m2.Red6 =   9)), 0)) END) AS '09',
	(CASE WHEN (Red1 =  10 OR Red2 =  10 OR Red3 =  10 OR Red4 =  10 OR Red5 = 10 OR Red6 =  10) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =  10 OR m2.Red2 =  10 OR m2.Red3 =  10 OR m2.Red4 =  10 OR m2.Red5 = 10 OR m2.Red6 =  10)), 0)) END) AS '10',
	(CASE WHEN (Red1 =  11 OR Red2 =  11 OR Red3 =  11 OR Red4 =  11 OR Red5 = 11 OR Red6 =  11) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =  11 OR m2.Red2 =  11 OR m2.Red3 =  11 OR m2.Red4 =  11 OR m2.Red5 = 11 OR m2.Red6 =  11)), 0)) END) AS '11',
	(CASE WHEN (Red1 =  12 OR Red2 =  12 OR Red3 =  12 OR Red4 =  12 OR Red5 = 12 OR Red6 =  12) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =  12 OR m2.Red2 =  12 OR m2.Red3 =  12 OR m2.Red4 =  12 OR m2.Red5 = 12 OR m2.Red6 =  12)), 0)) END) AS '12',
	(CASE WHEN (Red1 =  13 OR Red2 =  13 OR Red3 =  13 OR Red4 =  13 OR Red5 = 13 OR Red6 =  13) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =  13 OR m2.Red2 =  13 OR m2.Red3 =  13 OR m2.Red4 =  13 OR m2.Red5 = 13 OR m2.Red6 =  13)), 0)) END) AS '13',
	(CASE WHEN (Red1 =  14 OR Red2 =  14 OR Red3 =  14 OR Red4 =  14 OR Red5 = 14 OR Red6 =  14) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =  14 OR m2.Red2 =  14 OR m2.Red3 =  14 OR m2.Red4 =  14 OR m2.Red5 = 14 OR m2.Red6 =  14)), 0)) END) AS '14',
	(CASE WHEN (Red1 =  15 OR Red2 =  15 OR Red3 =  15 OR Red4 =  15 OR Red5 = 15 OR Red6 =  15) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =  15 OR m2.Red2 =  15 OR m2.Red3 =  15 OR m2.Red4 =  15 OR m2.Red5 = 15 OR m2.Red6 =  15)), 0)) END) AS '15',
	(CASE WHEN (Red1 =  16 OR Red2 =  16 OR Red3 =  16 OR Red4 =  16 OR Red5 = 16 OR Red6 =  16) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =  16 OR m2.Red2 =  16 OR m2.Red3 =  16 OR m2.Red4 =  16 OR m2.Red5 = 16 OR m2.Red6 =  16)), 0)) END) AS '16',
	(CASE WHEN (Red1 =  17 OR Red2 =  17 OR Red3 =  17 OR Red4 =  17 OR Red5 = 17 OR Red6 =  17) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =  17 OR m2.Red2 =  17 OR m2.Red3 =  17 OR m2.Red4 =  17 OR m2.Red5 = 17 OR m2.Red6 =  17)), 0)) END) AS '17',
	(CASE WHEN (Red1 =  18 OR Red2 =  18 OR Red3 =  18 OR Red4 =  18 OR Red5 = 18 OR Red6 =  18) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =  18 OR m2.Red2 =  18 OR m2.Red3 =  18 OR m2.Red4 =  18 OR m2.Red5 = 18 OR m2.Red6 =  18)), 0)) END) AS '18',
	(CASE WHEN (Red1 =  19 OR Red2 =  19 OR Red3 =  19 OR Red4 =  19 OR Red5 = 19 OR Red6 =  19) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =  19 OR m2.Red2 =  19 OR m2.Red3 =  19 OR m2.Red4 =  19 OR m2.Red5 = 19 OR m2.Red6 =  19)), 0)) END) AS '19',
	(CASE WHEN (Red1 =  20 OR Red2 =  20 OR Red3 =  20 OR Red4 =  20 OR Red5 = 20 OR Red6 =  20) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =  20 OR m2.Red2 =  20 OR m2.Red3 =  20 OR m2.Red4 =  20 OR m2.Red5 = 20 OR m2.Red6 =  20)), 0)) END) AS '20',
	(CASE WHEN (Red1 =  21 OR Red2 =  21 OR Red3 =  21 OR Red4 =  21 OR Red5 = 21 OR Red6 =  21) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =  21 OR m2.Red2 =  21 OR m2.Red3 =  21 OR m2.Red4 =  21 OR m2.Red5 = 21 OR m2.Red6 =  21)), 0)) END) AS '21',
	(CASE WHEN (Red1 =  22 OR Red2 =  22 OR Red3 =  22 OR Red4 =  22 OR Red5 = 22 OR Red6 =  22) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =  22 OR m2.Red2 =  22 OR m2.Red3 =  22 OR m2.Red4 =  22 OR m2.Red5 = 22 OR m2.Red6 =  22)), 0)) END) AS '22',
	(CASE WHEN (Red1 =  23 OR Red2 =  23 OR Red3 =  23 OR Red4 =  23 OR Red5 = 23 OR Red6 =  23) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =  23 OR m2.Red2 =  23 OR m2.Red3 =  23 OR m2.Red4 =  23 OR m2.Red5 = 23 OR m2.Red6 =  23)), 0)) END) AS '23',
	(CASE WHEN (Red1 =  24 OR Red2 =  24 OR Red3 =  24 OR Red4 =  24 OR Red5 = 24 OR Red6 =  24) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =  24 OR m2.Red2 =  24 OR m2.Red3 =  24 OR m2.Red4 =  24 OR m2.Red5 = 24 OR m2.Red6 =  24)), 0)) END) AS '24',
	(CASE WHEN (Red1 =  25 OR Red2 =  25 OR Red3 =  25 OR Red4 =  25 OR Red5 = 25 OR Red6 =  25) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =  25 OR m2.Red2 =  25 OR m2.Red3 =  25 OR m2.Red4 =  25 OR m2.Red5 = 25 OR m2.Red6 =  25)), 0)) END) AS '25',
	(CASE WHEN (Red1 =  26 OR Red2 =  26 OR Red3 =  26 OR Red4 =  26 OR Red5 = 26 OR Red6 =  26) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =  26 OR m2.Red2 =  26 OR m2.Red3 =  26 OR m2.Red4 =  26 OR m2.Red5 = 26 OR m2.Red6 =  26)), 0)) END) AS '26',
	(CASE WHEN (Red1 =  27 OR Red2 =  27 OR Red3 =  27 OR Red4 =  27 OR Red5 = 27 OR Red6 =  27) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =  27 OR m2.Red2 =  27 OR m2.Red3 =  27 OR m2.Red4 =  27 OR m2.Red5 = 27 OR m2.Red6 =  27)), 0)) END) AS '27',
	(CASE WHEN (Red1 =  28 OR Red2 =  28 OR Red3 =  28 OR Red4 =  28 OR Red5 = 28 OR Red6 =  28) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =  28 OR m2.Red2 =  28 OR m2.Red3 =  28 OR m2.Red4 =  28 OR m2.Red5 = 28 OR m2.Red6 =  28)), 0)) END) AS '28',
	(CASE WHEN (Red1 =  29 OR Red2 =  29 OR Red3 =  29 OR Red4 =  29 OR Red5 = 29 OR Red6 =  29) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =  29 OR m2.Red2 =  29 OR m2.Red3 =  29 OR m2.Red4 =  29 OR m2.Red5 = 29 OR m2.Red6 =  29)), 0)) END) AS '29',
	(CASE WHEN (Red1 =  30 OR Red2 =  30 OR Red3 =  30 OR Red4 =  30 OR Red5 = 30 OR Red6 =  30) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =  30 OR m2.Red2 =  30 OR m2.Red3 =  30 OR m2.Red4 =  30 OR m2.Red5 = 30 OR m2.Red6 =  30)), 0)) END) AS '30',
	(CASE WHEN (Red1 =  31 OR Red2 =  31 OR Red3 =  31 OR Red4 =  31 OR Red5 = 31 OR Red6 =  31) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =  31 OR m2.Red2 =  31 OR m2.Red3 =  31 OR m2.Red4 =  31 OR m2.Red5 = 31 OR m2.Red6 =  31)), 0)) END) AS '31',
	(CASE WHEN (Red1 =  32 OR Red2 =  32 OR Red3 =  32 OR Red4 =  32 OR Red5 = 32 OR Red6 =  32) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =  32 OR m2.Red2 =  32 OR m2.Red3 =  32 OR m2.Red4 =  32 OR m2.Red5 = 32 OR m2.Red6 =  32)), 0)) END) AS '32',
	(CASE WHEN (Red1 =  33 OR Red2 =  33 OR Red3 =  33 OR Red4 =  33 OR Red5 = 33 OR Red6 =  33) THEN 0 ELSE (RowInx - ISNULL((SELECT MAX(RowInx) FROM indeicedTable AS m2 WHERE m2.Issue < m1.Issue AND (m2.Red1 =  33 OR m2.Red2 =  33 OR m2.Red3 =  33 OR m2.Red4 =  33 OR m2.Red5 = 33 OR m2.Red6 =  33)), 0)) END) AS '33'
FROM indeicedTable AS m1