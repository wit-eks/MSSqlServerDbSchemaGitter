using System;
using System.IO;
using System.Text;

namespace SchemaGetter
{
    public class ModuleDefinition
    {
        public string Db { get; set; }
        public string Schema { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string TypeDesc { get; set; }
        public bool UsesAnsiNulls { get; set; }
        public bool UsesQuotedIdentifier { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string Definition { get; set; }

        public string FileName() => $"[{Schema}].[{Name}].sql";
        public string PathAbdFileName() => Path.Combine(Db,TypeDesc,FileName());
        
        public override string ToString()
        {

            var sb = new StringBuilder($"USE [{Db}]");
            sb.AppendLine();
            sb.AppendLine("GO");
            sb.AppendLine("--====================================================");
            sb.AppendLine( "--  AUTO GENERATED CODE");
            sb.AppendLine($"--  Module Create Date: {CreateDate:s}");
            sb.AppendLine($"--  Module Modify Date: {ModifyDate:s}");
            sb.AppendLine($"--  Module Type: {Type} - {TypeDesc}");
            sb.AppendLine("--====================================================");
            sb.AppendLine($"SET ANSI_NULLS {(UsesAnsiNulls ? "ON" : "OFF")}");
            sb.AppendLine("GO");
            sb.AppendLine($"SET QUOTED_IDENTIFIER {(UsesQuotedIdentifier ? "ON" : "OFF")}");
            sb.AppendLine("GO");
            sb.AppendLine();
            sb.AppendLine(Definition);

            return sb.ToString();
        }
    }
}