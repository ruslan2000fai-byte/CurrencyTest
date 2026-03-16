-- Migration script for UserService
-- Creates tables: users, revoked_tokens

CREATE TABLE IF NOT EXISTS users (
    id       SERIAL PRIMARY KEY,
    name     VARCHAR(255) NOT NULL,
    password VARCHAR(255) NOT NULL,
    UNIQUE (name)
);

CREATE TABLE IF NOT EXISTS revoked_tokens (
    jti        VARCHAR(64) PRIMARY KEY,
    revoked_at TIMESTAMPTZ NOT NULL,
    expires_at TIMESTAMPTZ NULL
);
