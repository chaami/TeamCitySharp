using System;
using System.Collections.Generic;
using TeamCitySharp.Connection;
using TeamCitySharp.DomainEntities;
using TeamCitySharp.Locators;

namespace TeamCitySharp.ActionTypes
{
    public class BuildQueue : IBuildQueue
    {
        private readonly ITeamCityCaller m_caller;

        private static readonly string BuildQueueRelativePath = "/app/rest/buildQueue";

        internal BuildQueue(ITeamCityCaller caller)
        {
            m_caller = caller;
        }

        public List<Build> ByBuildTypeLocator(BuildTypeLocator locator)
        {
            var buildWrapper = m_caller.Get<BuildWrapper>($"/app/rest/buildQueue?locator=buildType:({locator})");
            return int.Parse(buildWrapper.Count) > 0 ? buildWrapper.Build : new List<Build>();
        }

        public List<Build> ByProjectLocater(ProjectLocator locator)
        {
            var buildWrapper = m_caller.Get<BuildWrapper>($"/app/rest/buildQueue?locator=project:({locator})");
            return int.Parse(buildWrapper.Count) > 0 ? buildWrapper.Build : new List<Build>();
        }

        public List<Build> WithExpandoLocator(dynamic locator)
        {
            var buildWrapper = m_caller.Get<BuildWrapper>($"{BuildQueueRelativePath}?locator={GetLocator(locator)}");
            if (buildWrapper.Build == null)
            {
                return new List<Build>();
            }
            else
            {
                return buildWrapper.Build;
            }
        }

        public string GetLocator(dynamic locator)
        {
            var locatorDictionnary = locator as IDictionary<string, object>;
            if (locatorDictionnary != null && locatorDictionnary.ContainsKey("name"))
            {
                return $"{locator.type}:(name:{locator.name})";
            }

            if (locatorDictionnary != null && locatorDictionnary.ContainsKey("id"))
            {
                return $"{locator.type}:(id:{locator.id})";
            }
            
            throw new ArgumentException($"{locator.type} Locator should have a type or name...", nameof(locator));
        }
    }
}