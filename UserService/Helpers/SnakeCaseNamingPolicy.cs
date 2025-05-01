using System.Text.Json;
using System.Text.RegularExpressions;

namespace User_Service.Helpers
{
    public class SnakeCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return Regex.Replace(name, "([a-z])([A-Z])", "$1_$2").ToLower();
        }
    }
}
