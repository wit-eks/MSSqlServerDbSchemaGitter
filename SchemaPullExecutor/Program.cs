using System;
using System.IO;
using GitPusher;
using Microsoft.Extensions.Configuration;
using SchemaGetter;


namespace SchemaPullExecutor
{
    class Program
    {
        static void Main(string[] args)
        {

            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var destFolder = config["SaveDestinationDirectory"];
            var connectionString = config["DbConnectionString"];
            var gitUserName = config["GitUserName"];
            var gitPassword = config["GitPassword"];
            var gitBranch = config["GitBranch"];
            var gitCommitterAuthorName = config["CommitterAuthorName"];
            var gitCommitterAuthorEmail = config["CommitterAuthorEmail"];


            var dbSchemaParser = new DbSchemaParser(connectionString);

            var modules = dbSchemaParser.GetSchema();

            var schemaSaver = new SchemaToFileSaver(destFolder);

            schemaSaver.Save(modules);

            schemaSaver.DeleteNotExisting(modules);

            var gitter = new Gitter(destFolder, gitUserName, gitPassword, gitBranch);
            if (!string.IsNullOrWhiteSpace(gitCommitterAuthorName)) gitter.CommitterAuthorName = gitCommitterAuthorName;
            if (!string.IsNullOrWhiteSpace(gitCommitterAuthorEmail)) gitter.CommitterAuthorEmail = gitCommitterAuthorEmail;

            var pushed = gitter.TryPush(out var pushResultMessage);

            Console.WriteLine(pushed ? "Pushed properly" : "Error occurred");
            Console.WriteLine(pushResultMessage);

        }
    }
}
