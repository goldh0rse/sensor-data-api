namespace RestApi.utils
{
    public static class EnvLoader
    {
        public static void LoadEnvironment(string path)
        {
            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);
                HashSet<string> keys = new();

                foreach (var line in lines)
                {
                    // Skip comments and empty lines
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    {
                        continue;
                    }

                    var parts = line.Split(
                        '=',
                        StringSplitOptions.RemoveEmptyEntries);

                    // Check if line format is correct
                    if (parts.Length != 2)
                    {
                        Console.WriteLine($"Skipping invalid line: {line}");
                        continue;
                    }

                    string key = parts[0].Trim();
                    string value = parts[1].Trim();

                    // Check for empty keys
                    if (string.IsNullOrEmpty(key))
                    {
                        Console.WriteLine($"Skipping invalid key in line: {line}");
                        continue;
                    }

                    // Check for duplicate keys
                    if (keys.Contains(key))
                    {
                        Console.WriteLine($"Duplicate key {key} found. Skipping...");
                        continue;
                    }
                    keys.Add(key);

                    // Set the environment variable
                    Environment.SetEnvironmentVariable(key, value);
                }
            }
            else
            {
                Console.WriteLine("No environment file found!");
                return;
            }
        }
    }
}