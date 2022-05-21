namespace Kata.TransitiveDependencies;

public class DependencyGraph
{
    private readonly Dictionary<char, IReadOnlySet<char>> _dependencies = new();

    public void Add(char thing, params char[] dependencies)
    {
        if (!_dependencies.ContainsKey(thing))
        {
            _dependencies.Add(thing, new HashSet<char>(dependencies));
        }
        else
        {
            _dependencies[thing] = _dependencies[thing].Concat(dependencies).ToHashSet();
        }
    }

    public IEnumerable<char> DependenciesFor(char thing)
    {
        if (!_dependencies.ContainsKey(thing))
            return Enumerable.Empty<char>();

        var dependencies = new HashSet<char>();
        var subDependencies = new Queue<char>(_dependencies[thing]);

        while (subDependencies.Any())
        {
            var dependency = subDependencies.Dequeue();
            
            if (!dependencies.Add(dependency) || !_dependencies.ContainsKey(dependency))
                continue;

            foreach (var transitive in _dependencies[dependency].Where(t => t != thing))
            {
                subDependencies.Enqueue(transitive);
            }
        }

        return dependencies;
    }

}