namespace Qanat.EFModels.Entities;

public class TopologicalSort
{
    public static IList<T> Sort<T, Key>(IEnumerable<T> source, Func<T, IEnumerable<Key>> getDependencies, Func<T, Key> getKey, Func<T, Key> getLabel)
    {
        return Sort<T, Key>(source, RemapDependencies(source, getDependencies, getKey), getLabel);
    }

    public static IList<T> Sort<T, Key>(IEnumerable<T> source, Func<T, IEnumerable<Key>> getDependencies, Func<T, Key> getKey, bool throwOnMissingDependency = true)
    {
        return Sort<T, Key>(source, RemapDependencies(source, getDependencies, getKey, throwOnMissingDependency), getKey);
    }

    public static IList<T> Sort<T, Key>(IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies, Func<T, Key> getLabel, IEqualityComparer<T> comparer = null)
    {
        var sorted = new List<T>();
        var visited = new Dictionary<T, bool>(comparer);

        foreach (var item in source)
        {
            Visit(item, getDependencies, sorted, visited, getLabel);
        }

        return sorted;
    }

    public static void Visit<T, Key>(T item, Func<T, IEnumerable<T>> getDependencies, List<T> sorted, Dictionary<T, bool> visited, Func<T, Key> getLabel)
    {
        if (item == null)
        {
            return;
        }

        var alreadyVisited = visited.TryGetValue(item, out var inProcess);
        if (alreadyVisited)
        {
            if (inProcess)
            {
                var label = getLabel(item);
                throw new ArgumentException($"Detected a circular dependency starting at {label}, please double check the dependencies.");
            }
        }
        else
        {
            visited[item] = true;
            var dependencies = getDependencies(item);
            if (dependencies != null)
            {
                foreach (var dependency in dependencies)
                {
                    Visit(dependency, getDependencies, sorted, visited, getLabel);
                }
            }

            visited[item] = false;
            sorted.Add(item);
        }
    }

    private static Func<T, IEnumerable<T>> RemapDependencies<T, Key>(IEnumerable<T> source, Func<T, IEnumerable<Key>> getDependencies, Func<T, Key> getKey, bool throwOnMissingDependency = true)
    {
        var map = source.ToDictionary(getKey);
        return item =>
        {
            var dependencies = getDependencies(item);
            return dependencies?.Select(key => {
                if (!map.TryGetValue(key, out var value))
                {
                    if(throwOnMissingDependency)
                    {
                        throw new KeyNotFoundException($"{item.ToString()}: The dependency {key.ToString()} does not exist.");
                    }
                }
                return value;
            });
        };
    }

}