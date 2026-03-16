-- Migration script for FinanceService
-- Creates tables: currency, user_favorite_currencies

CREATE TABLE IF NOT EXISTS currency (
    id       SERIAL PRIMARY KEY,
    name     VARCHAR(255) NOT NULL,
    char_code VARCHAR(10) NOT NULL,
    nominal  INTEGER NOT NULL DEFAULT 1,
    rate     DECIMAL(18, 4) NOT NULL,
    UNIQUE (char_code)
);

CREATE TABLE IF NOT EXISTS user_favorite_currencies (
    user_id     INTEGER NOT NULL,
    currency_id INTEGER NOT NULL REFERENCES currency(id) ON DELETE CASCADE,
    PRIMARY KEY (user_id, currency_id)
);
