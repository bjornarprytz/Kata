using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Kata;

public record Pay(string Cost)
{
    public string With(string resources)
    {
        var genericMana = new Regex(@"\d+").Match(Cost);

        var genericValue = int.Parse(genericMana.Value);

        var colorMana = new Regex(@"\w+$").Match(Cost).Value.ToList();

        var remainingResources = resources.ToList();
        
        var resourcesToSpend = new StringBuilder();

        foreach (var resource in colorMana.Where(resource => remainingResources.Contains(resource)))
        {
            remainingResources.Remove(resource);
            resourcesToSpend.Append(resource);
        }

        if (remainingResources.Count >= genericValue)
        {
            foreach (var c in remainingResources.Take(genericValue))
            {
                resourcesToSpend.Append(c);
            }
            
        }
        else
        {
            return "Error";
        }

        return resourcesToSpend.ToString();
    }
}