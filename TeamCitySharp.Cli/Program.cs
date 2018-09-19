using System;
using System.Text;
using PowerArgs;

namespace TeamCitySharp.Cli
{
    public struct TeamCityConnectionSettings
    {

        private string _buildServer;

        [ArgRequired(PromptIfMissing = true), ArgDescription("Hostname of the team city build server."), ArgPosition(1)]
        public string BuildServer {
            get { return _buildServer; }
            set
            {
                int position = value.IndexOf(":");
                if ( position != -1)
                {
                    _buildServer = value.Substring(0, position);
                    Port = value.Substring(position);
                }
                else
                {
                    _buildServer = value;
                }

            }
        }

        [ArgDescription("Port that should be used to connect to the team city build server."), DefaultValue("443"), ArgPosition(2)]
        public string Port { get; set; }

        [ArgDescription("Use ssl to connect to the team city build server"), DefaultValue(true), ArgPosition(3)]
        public bool UseSsl { get; set; }

        [ArgRequired(PromptIfMissing = true), ArgDescription("Team city build server username.")]
        public String Username { get; set; }

        [ArgDescription("Password to access the team city server")]
        public SecureStringArgument Password { get; set; }

    }

    [ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling), TabCompletion(REPL=true)]
    public class CliArgs
    {
        private static ITeamCityClient _client;

        [ArgActionMethod, ArgDescription("Connect to the Team City build server.")]
        public void Connect(TeamCityConnectionSettings connectionSettings)
        {
            _client = new TeamCityClient($"{connectionSettings.BuildServer}:{connectionSettings.Port}", connectionSettings.UseSsl);
            _client.Connect(connectionSettings.Username, connectionSettings.Password.ConvertToNonsecureString());
            StringBuilder connected = new StringBuilder(System.Environment.NewLine).Append("Connected!");
            Console.WriteLine(connected);
        }

        [ArgActionMethod, ArgDescription("List the projects of the TeamCity build server.")]
        public void Projects()
        {
            if (_client == null)
            {
                throw new ArgException("Connect to a team city server first.");
            }

            Console.WriteLine("Projects:");
            foreach (var project in _client.Projects.All())
            {
                Console.WriteLine($"  Id: {project.Id}, Name: {project.Name}, Description: {project.Description}");
            }
        }

        [ArgActionMethod, ArgDescription("List the build configurations of the TeamCity build server.")]
        public void BuildConfigs()
        {
            if (_client == null)
            {
                throw new ArgException("Connect to a team city server first.");
            }

            Console.WriteLine("Builds configurations:");
            foreach (var buildConfig in _client.BuildConfigs.All())
            {
                Console.WriteLine($"  Id: {buildConfig.Id}, Name: {buildConfig.Name}, Description: {buildConfig.Description}");
            }
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the TeamCitySharp Command line interface!");
            Args.InvokeAction<CliArgs>(args);
        }
    }
}
