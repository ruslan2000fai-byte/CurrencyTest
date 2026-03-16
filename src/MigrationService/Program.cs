using Microsoft.Extensions.Configuration;
using Npgsql;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var connectionString = configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

var scriptsPath = configuration["MigrationSettings:ScriptsPath"]
    ?? "scripts";

var scriptFiles = configuration.GetSection("MigrationSettings:ScriptFiles").Get<string[]>()
    ?? Array.Empty<string>();

Console.WriteLine("Starting database migration...");
Console.WriteLine($"Connection string: {connectionString.Split(';').First()}");
Console.WriteLine($"Scripts path: {scriptsPath}");

await using var conn = new NpgsqlConnection(connectionString);
await conn.OpenAsync();

foreach (var scriptFile in scriptFiles)
{
    var fullPath = Path.Combine(scriptsPath, scriptFile);
    
    if (!File.Exists(fullPath))
    {
        Console.WriteLine($"Warning: Script file '{fullPath}' not found, skipping...");
        continue;
    }
    
    Console.WriteLine($"Executing script: {scriptFile}");
    
    var sql = await File.ReadAllTextAsync(fullPath);
    
    await using var cmd = new NpgsqlCommand(sql, conn);
    await cmd.ExecuteNonQueryAsync();
    
    Console.WriteLine($"Completed: {scriptFile}");
}

Console.WriteLine("Migration completed successfully.");
