using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SchemaGetter
{
    public class SchemaToFileSaver
    {
        private string BaseDirectory { get; set; }

        public SchemaToFileSaver(string baseDirectory)
        {
            BaseDirectory = baseDirectory;
        }

        public void Save(List<ModuleDefinition> moduleDefinitions)
        {
            Directory.CreateDirectory(BaseDirectory);

            foreach (var moduleDefinition in moduleDefinitions)
            {
         
                var path = Path.Combine(BaseDirectory, moduleDefinition.Db, moduleDefinition.TypeDesc,
                    moduleDefinition.FileName());
                var dir = Path.GetDirectoryName(path);
                Directory.CreateDirectory(dir);
                File.WriteAllText(path, moduleDefinition.ToString(), Encoding.UTF8);
            }
        }

        public void DeleteNotExisting(List<ModuleDefinition> existingModules)
        {
            var filesInRepo = Directory.GetFiles(BaseDirectory, searchPattern: "*.*", SearchOption.AllDirectories)
                .Where(f => !f.StartsWith(Path.Combine(BaseDirectory,".git")))
                .ToList();

            var filesInRoot = Directory.GetFiles(BaseDirectory, searchPattern: "*.*", SearchOption.TopDirectoryOnly).ToList();

            filesInRoot.ForEach(r => filesInRepo.Remove(r));

            var existingInDb = existingModules.Select(e => Path.Combine(BaseDirectory, e.PathAbdFileName()).ToString());
            var toDelete = filesInRepo.Except(existingInDb);

            foreach (var f in toDelete)
            {
                File.Delete(f);
            }

        }
    }
}