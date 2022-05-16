using System.Net.Security;

namespace Kata.BackToTheCheckout;

public class CheckOut
{
    private readonly PricingRules _pricingRules;
    private readonly List<Item> _items = new ();
    
    public int Total { get; private set; }

    public CheckOut(PricingRules pricingRules)
    {
        _pricingRules = pricingRules;
    }
    
    public int Scan(Item item)
    {
        _items.Add(item);

        Total = _pricingRules.Price(_items);

        return Total;
    }

    public int Remove(Item item)
    {
        _items.Remove(item);
        
        Total = _pricingRules.Price(_items);

        return Total;
    }
}

public record Item(string Id)
{
    public static IEnumerable<Item> Create(string items) => items.Select(c => new Item(c.ToString()));
}

public class PricingRules
{
    private readonly List<PricingRule> _pricingRules = new();

    public void AddRule(PricingRule pricingRule)
    {
        _pricingRules.Add(pricingRule);
    }
    
    public int Price(IEnumerable<Item> items)
    {
        var pattern = string.Join("", items.Select(i => i.Id));

        var rules = new Queue<PricingRule>(_pricingRules);

        var price = 0;
        
        while (pattern.Any() && rules.Any() && rules.Dequeue() is { } rule)
        {
            price += rule.Process(ref pattern);
        }

        return price;
    }
}

public record PricingRule(string Pattern, int Cost);

public static class PricingRulesExtensions
{

    public static PricingRules AddStandardRules(this PricingRules pricingRules)
    {
        pricingRules.AddRule(new PricingRule("AAA", 130));
        pricingRules.AddRule(new PricingRule("BB", 45));
        pricingRules.AddRule(new PricingRule("A", 50));
        pricingRules.AddRule(new PricingRule("B", 30));
        pricingRules.AddRule(new PricingRule("C", 20));
        pricingRules.AddRule(new PricingRule("D", 15));

        return pricingRules;
    }

    public static int Process(this PricingRule rule, ref string pattern)
    {
        var remaining = pattern;
        var value = 0;
        var foundPattern = true;

        while (foundPattern)
        {
            foundPattern = true;
            
            foreach (var item in rule.Pattern)
            {
                if (!remaining.Contains(item))
                {
                    foundPattern = false;
                    break;
                }
                remaining = TakeOutOne(remaining, item);
            }

            if (!foundPattern) break;
            
            pattern = remaining;
            value += rule.Cost;
        }


        return value;

        string TakeOutOne(string p, char c)
        {
            var i = p.IndexOf(c);

            if (i == -1)
                return p;

            var skipC = i+1;

            return p[..i] + p[skipC..];
        }
    }
}