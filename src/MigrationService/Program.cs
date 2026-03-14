using Npgsql;

var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
    ?? "Host=localhost;Port=5432;Database=currency_db;Username=postgres;Password=postgres";

Console.WriteLine("Starting database migration...");

await using var conn = new NpgsqlConnection(connectionString);
await conn.OpenAsync();

const string sql = """
    CREATE TABLE IF NOT EXISTS currency (
        id       SERIAL PRIMARY KEY,
        name     VARCHAR(255) NOT NULL,
        char_code VARCHAR(10) NOT NULL,
        nominal  INTEGER NOT NULL DEFAULT 1,
        rate     DECIMAL(18, 4) NOT NULL DEFAULT 0,
        UNIQUE (char_code)
    );

    CREATE TABLE IF NOT EXISTS users (
        id       SERIAL PRIMARY KEY,
        name     VARCHAR(255) NOT NULL,
        password VARCHAR(255) NOT NULL,
        UNIQUE (name)
    );

    CREATE TABLE IF NOT EXISTS user_favorite_currencies (
        user_id     INTEGER NOT NULL REFERENCES users(id)    ON DELETE CASCADE,
        currency_id INTEGER NOT NULL REFERENCES currency(id) ON DELETE CASCADE,
        PRIMARY KEY (user_id, currency_id)
    );

    CREATE TABLE IF NOT EXISTS revoked_tokens (
        jti        VARCHAR(64) PRIMARY KEY,
        revoked_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
        expires_at TIMESTAMPTZ
    );
    """;

await using var cmd = new NpgsqlCommand(sql, conn);
await cmd.ExecuteNonQueryAsync();

Console.WriteLine("Migration completed successfully.");
