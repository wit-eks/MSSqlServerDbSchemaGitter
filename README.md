# Simple SQL Server Db objects downloader and git pusher
It saves sql objects as sql scripts in local repo folder. Actions of downloading the schema and pushing the changes to the repo need to be scheduled. The repo should be cloned to local folder manually. Example of use presented in SchemaPullExecutor project. There are 3 projects:
- GitPusher - .NET Standard 2.0 – pushing local folder changes to the remote repo
- SchemaGetter - .NET Standard 2.0 – sql modules download and to file saver
- SchemaPullExecutor - .NET Core 2.2 – console app – an example how to utilize mentioned projects
## Schema Getter
- generated scripts have sql module create date and last modify date
- sql that gets [modules](https://docs.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-sql-modules-transact-sql?view=sql-server-2017) data:
```
select 
	[db] = db_name(),
	[schema] = OBJECT_SCHEMA_NAME(m.object_id),
	[name] = OBJECT_NAME(m.object_id) 
	,o.type, o.type_desc
	,m.uses_ansi_nulls
	,m.uses_quoted_identifier
	,o.create_date, o.modify_date
	,m.definition
from 
	sys.sql_modules m
inner join
	sys.objects o on m.object_id = o.object_id
where 
	1=1
order by 
	o.type
```
## GitPusher
-	Screenshots from GitKraken


![first-push.png](https://github.com/wit-eks/MSSqlServerDbSchemaGitter/blob/master/_assets/first-push.png)
![trigger-modified.png](https://github.com/wit-eks/MSSqlServerDbSchemaGitter/blob/master/_assets/trigger-modified.png)
![deleted-copy-of-sp.png](https://github.com/wit-eks/MSSqlServerDbSchemaGitter/blob/master/_assets/deleted-copy-of-sp.png)
