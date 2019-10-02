using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using PowerArgs;
using TeamCitySharp.Locators;

namespace TeamCitySharp.Cli
{

    public struct TeamCityConnectionSettings
    {

        private string _buildServer;

        [ArgRequired(PromptIfMissing = true), ArgDescription("Hostname of the team city build server."), ArgPosition(1)]
        public string BuildServer {
            get => _buildServer;
            set
            {
                int position = value.IndexOf(":", StringComparison.Ordinal);
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
            _client.Authenticate();
            StringBuilder connected = new StringBuilder(Environment.NewLine).Append("Connected!");
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

        [ArgActionMethod, ArgDescription("Access the build queue information.")]
        public void BuildQueue(string locator)
        {
            if (_client == null)
            {
                throw new ArgException("Connect to a team city server first.");
            }

            int parameterIndex = locator.IndexOf(":", StringComparison.Ordinal);
            var builds = _client.BuildQueue.ByProjectLocater(ProjectLocator.WithName(locator.Substring(parameterIndex + 1)));
            foreach (var build in builds)
            {
                Console.WriteLine(build);
            }

            var expandoLocator = new ExpandoObject() as IDictionary<string, object>;
            // TODO Validate the locator format.
            int typeIndex = locator.IndexOf("/", StringComparison.Ordinal);
            expandoLocator.Add("type", locator.Substring(0, typeIndex));
            expandoLocator.Add(locator.Substring(typeIndex + 1, parameterIndex - typeIndex - 1),
                locator.Substring(parameterIndex + 1));
            builds = _client.BuildQueue.WithExpandoLocator(expandoLocator);
            foreach (var build in builds)
            {
                Console.WriteLine(build);
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
