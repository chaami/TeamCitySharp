using System;

namespace TeamCitySharp.Locators
{
    public class ProjectLocator
    {
        public static ProjectLocator WithId(string id)
        {
            return new ProjectLocator {Id = id};
        }

        public static ProjectLocator WithName(string name)
        {
            return new ProjectLocator {Name = name};
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Id))
                return "id:" + Id;

            if (!string.IsNullOrEmpty(Name))
                return "name:" + Name;

            throw new ArgumentException("Project locator should have an id or name set.");
        }
    }
}