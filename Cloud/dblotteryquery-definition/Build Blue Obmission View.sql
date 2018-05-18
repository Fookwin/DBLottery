DROP TABLE IF EXISTS #blueNumMatrix

SELECT Issue, ROW_NUMBER() OVER (ORDER BY Issue) RowInx, Blue
INTO #blueNumMatrix 
FROM dbo.Basic
ORDER BY Issue DESC 

SELECT Issue, RowInx, Blue, 
	(CASE Blue WHEN  1 THEN 0 ELSE (RowInx - ISNULL((SELECT TOP 1 RowInx FROM #blueNumMatrix AS m2 WHERE m2.Issue < m1.Issue AND m2.Blue =  1), 0)) END) AS '01',
	(CASE Blue WHEN  2 THEN 0 ELSE (RowInx - ISNULL((SELECT TOP 1 RowInx FROM #blueNumMatrix AS m2 WHERE m2.Issue < m1.Issue AND m2.Blue =  2), 0)) END) AS '02',
	(CASE Blue WHEN  3 THEN 0 ELSE (RowInx - ISNULL((SELECT TOP 1 RowInx FROM #blueNumMatrix AS m2 WHERE m2.Issue < m1.Issue AND m2.Blue =  3), 0)) END) AS '03',
	(CASE Blue WHEN  4 THEN 0 ELSE (RowInx - ISNULL((SELECT TOP 1 RowInx FROM #blueNumMatrix AS m2 WHERE m2.Issue < m1.Issue AND m2.Blue =  4), 0)) END) AS '04',
	(CASE Blue WHEN  5 THEN 0 ELSE (RowInx - ISNULL((SELECT TOP 1 RowInx FROM #blueNumMatrix AS m2 WHERE m2.Issue < m1.Issue AND m2.Blue =  5), 0)) END) AS '05',
	(CASE Blue WHEN  6 THEN 0 ELSE (RowInx - ISNULL((SELECT TOP 1 RowInx FROM #blueNumMatrix AS m2 WHERE m2.Issue < m1.Issue AND m2.Blue =  6), 0)) END) AS '06',
	(CASE Blue WHEN  7 THEN 0 ELSE (RowInx - ISNULL((SELECT TOP 1 RowInx FROM #blueNumMatrix AS m2 WHERE m2.Issue < m1.Issue AND m2.Blue =  7), 0)) END) AS '07',
	(CASE Blue WHEN  8 THEN 0 ELSE (RowInx - ISNULL((SELECT TOP 1 RowInx FROM #blueNumMatrix AS m2 WHERE m2.Issue < m1.Issue AND m2.Blue =  8), 0)) END) AS '08',
	(CASE Blue WHEN  9 THEN 0 ELSE (RowInx - ISNULL((SELECT TOP 1 RowInx FROM #blueNumMatrix AS m2 WHERE m2.Issue < m1.Issue AND m2.Blue =  9), 0)) END) AS '09',
	(CASE Blue WHEN 10 THEN 0 ELSE (RowInx - ISNULL((SELECT TOP 1 RowInx FROM #blueNumMatrix AS m2 WHERE m2.Issue < m1.Issue AND m2.Blue = 10), 0)) END) AS '10',
	(CASE Blue WHEN 11 THEN 0 ELSE (RowInx - ISNULL((SELECT TOP 1 RowInx FROM #blueNumMatrix AS m2 WHERE m2.Issue < m1.Issue AND m2.Blue = 11), 0)) END) AS '11',
	(CASE Blue WHEN 12 THEN 0 ELSE (RowInx - ISNULL((SELECT TOP 1 RowInx FROM #blueNumMatrix AS m2 WHERE m2.Issue < m1.Issue AND m2.Blue = 12), 0)) END) AS '12',
	(CASE Blue WHEN 13 THEN 0 ELSE (RowInx - ISNULL((SELECT TOP 1 RowInx FROM #blueNumMatrix AS m2 WHERE m2.Issue < m1.Issue AND m2.Blue = 13), 0)) END) AS '13',
	(CASE Blue WHEN 14 THEN 0 ELSE (RowInx - ISNULL((SELECT TOP 1 RowInx FROM #blueNumMatrix AS m2 WHERE m2.Issue < m1.Issue AND m2.Blue = 14), 0)) END) AS '14',
	(CASE Blue WHEN 15 THEN 0 ELSE (RowInx - ISNULL((SELECT TOP 1 RowInx FROM #blueNumMatrix AS m2 WHERE m2.Issue < m1.Issue AND m2.Blue = 15), 0)) END) AS '15',
	(CASE Blue WHEN 16 THEN 0 ELSE (RowInx - ISNULL((SELECT TOP 1 RowInx FROM #blueNumMatrix AS m2 WHERE m2.Issue < m1.Issue AND m2.Blue = 16), 0)) END) AS '16'
FROM (SELECT Issue, Blue, ROW_NUMBER() OVER (ORDER BY Issue) RowInx FROM dbo.Basic) AS m1

DROP TABLE IF EXISTS #blueNumMatrix