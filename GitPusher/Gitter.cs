using System;
using System.Linq;
using LibGit2Sharp;

namespace GitPusher
{
    public class Gitter
    {
        private readonly string _repositoryFolder;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _branchName;
        public string CommitterAuthorName = "GitterAutoCommitter";
        public string CommitterAuthorEmail = "gitter@autocommitter";

        public bool IsRepoValid { get; }

        public Gitter(string repositoryFolder, string userName, string password, string branchName = "master")
        {
            _repositoryFolder = repositoryFolder;
            _userName = userName;
            _password = password;
            _branchName = branchName;
            IsRepoValid = Repository.IsValid(_repositoryFolder);
        }

        public bool TryPush(out string pushResultMessage)
        {
            
            if (!IsRepoValid)
            {
                pushResultMessage = $"This is not proper GIT repository: {_repositoryFolder}";
                return false;
            }
            try
            {
                using (var repo = new Repository(_repositoryFolder))
                {

                    var state = repo.RetrieveStatus(new StatusOptions()).GroupBy(i => i.State)
                        .ToDictionary(g => g.Key, g => g.Count());
                    pushResultMessage = string.Join(Environment.NewLine, state.Select(s => $"{s.Key} - {s.Value}"));
     
                    var changesCount = state.Sum(s => s.Value);

                    if (changesCount > 0)
                    {
                        Commands.Stage(repo, "*");

                        var author = new Signature(CommitterAuthorName, CommitterAuthorEmail, DateTime.Now);

                        var message = $"Auto commit - ({changesCount}) files";
                        var commit = repo.Commit(message, author, author);

                        pushResultMessage += $"{Environment.NewLine}Commit: {commit.Sha}";
                    }

                    var options = new PushOptions
                    {
                        CredentialsProvider = (url, usernameFromUrl, types) =>
                            new UsernamePasswordCredentials {Username = _userName, Password = _password}
                    };
                    repo.Network.Push(repo.Branches[_branchName], options);
                }

                return true;
            }
            catch (Exception e)
            {
                pushResultMessage = $"Exception: {e.Message}";
                return false;
            }
        }
    }
}
