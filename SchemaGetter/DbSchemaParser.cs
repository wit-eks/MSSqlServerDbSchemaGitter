using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SchemaGetter
{
    public class DbSchemaParser
    {
        public DbSchemaParser(string connectionString)
        {
            ConnectionString = connectionString;
        }

        private string ConnectionString { get; }
        private static string _getSchemaQuery = @"
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
where 1=1
	
order by 
	o.type
";

        public List<ModuleDefinition> GetSchema()
        {
            var res = new List<ModuleDefinition>();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand(_getSchemaQuery, connection);
                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var e = new ModuleDefinition
                        {
                            Db = reader.GetString(0),
                            Schema = reader.GetString(1),
                            Name = reader.GetString(2),
                            Type = reader.GetString(3).Trim(),
                            TypeDesc = reader.GetString(4).Trim(),
                            UsesAnsiNulls = reader.GetBoolean(5),
                            UsesQuotedIdentifier = reader.GetBoolean(6),
                            CreateDate = reader.GetDateTime(7),
                            ModifyDate = reader.GetDateTime(8),
                            Definition = reader.GetString(9),
                        };
                        res.Add(e);
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }
                reader.Close();
            }

            return res;
        }
    }
}
