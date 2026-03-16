# CurrencyTest — Решение на микросервисах

Приложение для отслеживания валют, построенное на основе микросервисной архитектуры с использованием .NET 8, Clean Architecture и CQRS.

## Архитектура

```
CurrencyTest/
├── src/
│   ├── MigrationService/          # Консольное приложение для миграции БД
│   ├── CurrencyUpdater/           # Фоновый сервис — почасовое получение XML ЦБ
│   ├── UserService/
│   │   ├── UserService.Domain/    # Сущности, интерфейсы репозиториев
│   │   ├── UserService.Application/   # CQRS команды (Register, Login, Logout)
│   │   ├── UserService.Infrastructure/ # EF Core, JWT, BCrypt
│   │   └── UserService.API/       # ASP.NET Core Web API (порт 5001)
│   ├── FinanceService/
│   │   ├── FinanceService.Domain/
│   │   ├── FinanceService.Application/ # CQRS запросы/команды
│   │   ├── FinanceService.Infrastructure/
│   │   └── FinanceService.API/    # ASP.NET Core Web API (порт 5002)
│   └── ApiGateway/                # Шлюз Ocelot (порт 5000)
└── tests/
    ├── UserService.Tests/
    └── FinanceService.Tests/
```

## Технологический стек

- **.NET 8** / **ASP.NET Core**
- **PostgreSQL** (через Npgsql + EF Core 8)
- **MediatR** — паттерн CQRS
- **Ocelot** — API Gateway
- **JWT Bearer** — аутентификация
- **BCrypt.Net-Next** — хеширование паролей
- **xUnit + Moq** — модульное тестирование
- **Docker Compose** — оркестрация контейнеров

## Схема базы данных

```sql
currency          (id, name, char_code, nominal, rate)
users             (id, name, password)
user_favorite_currencies (user_id, currency_id → currency)
revoked_tokens    (jti, revoked_at, expires_at)
```

## Скрипты миграции

MigrationService выполняет SQL-скрипты из папки `scripts/`:

| Файл | Описание |
|------|----------|
| `001_Users_RevokedToken.sql` | Таблицы `users`, `revoked_tokens` (UserService) |
| `002_Currency_UserFavorite.sql` | Таблицы `currency`, `user_favorite_currencies` (FinanceService) |

Настройка через `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=currency_db;Username=postgres;Password=postgres"
  },
  "MigrationSettings": {
    "ScriptsPath": "scripts",
    "ScriptFiles": [
      "001_Users_RevokedToken.sql",
      "002_Currency_UserFavorite.sql"
    ]
  }
}
```

## JWT Logout / Отзыв токенов

- `POST /api/auth/logout` отзывает текущий JWT, сохраняя его `jti` в таблице `revoked_tokens`.
- `UserService` и `FinanceService` проверяют входящие JWT и отклоняют отозванные токены.
- Истечение срока действия токена используется в UTC (`DateTime.UtcNow`).

## API Endpoints

### Через API Gateway (http://localhost:5000)

| Метод | Маршрут                         | Auth | Описание                       |
|-------|---------------------------------|------|--------------------------------|
| POST  | /api/auth/register              | —    | Регистрация нового пользователя |
| POST  | /api/auth/login                 | —    | Вход, возврат JWT токена       |
| POST  | /api/auth/logout                | JWT  | Отзыв текущего токена          |
| GET   | /api/finance/currencies         | JWT  | Получить избранные валюты пользователя |
| POST  | /api/finance/favorites          | JWT  | Добавить валюту в избранное    |
| DELETE| /api/finance/favorites/{id}     | JWT  | Удалить валюту из избранного   |

## Конфигурация API Gateway по окружениям

- `src/ApiGateway/ocelot.json` используется для Docker/контейнеров и маршрутизирует на имена сервисов:
  - `user_service:8080`
  - `finance_service:8080`
- `src/ApiGateway/ocelot.Development.json` переопределяет маршруты для локальной разработки:
  - `localhost:5001`
  - `localhost:5002`

### Примеры запросов / ответов

**Регистрация / Вход**
```json
POST /api/auth/register
{ "name": "alice", "password": "secret" }

Ответ: { "userId": 1, "userName": "alice", "token": "eyJ..." }
```

**Добавить в избранное**
```json
POST /api/finance/favorites
Authorization: Bearer eyJ...
{ "currencyId": 5 }
```

**Получить избранные валюты**
```
GET /api/finance/currencies
Authorization: Bearer eyJ...

Ответ: [
  { "id": 5, "name": "Доллар США", "charCode": "USD", "nominal": 1, "rate": 90.50 }
]
```

## Запуск через Docker Compose

```bash
docker-compose up --build
```

Сервисы запускаются по порядку: **PostgreSQL → MigrationService → CurrencyUpdater + UserService + FinanceService → ApiGateway**

## Запуск локально (без Docker)

1. Запустите PostgreSQL на localhost:5432 с базой данных `currency_db`
2. Выполните миграции:
   ```bash
   cd src/MigrationService
   dotnet run
   ```
3. Запустите сервисы в отдельных терминалах:
   ```bash
   cd src/CurrencyUpdater && dotnet run
   cd src/UserService/UserService.API && dotnet run      # http://localhost:5001
   cd src/FinanceService/FinanceService.API && dotnet run # http://localhost:5002
   cd src/ApiGateway && dotnet run                       # http://localhost:5000
   ```

## Запуск тестов

```bash
dotnet test
```

## Smoke Test (Вход → Выход → 401)

1. Вход и сохранение токена:
   ```bash
   curl -X POST "http://localhost:5000/api/auth/login" ^
     -H "Content-Type: application/json" ^
     -d "{\"name\":\"alice\",\"password\":\"secret\"}"
   ```
2. Доступ к защищённому эндпоинту с токеном (ожидается `200`):
   ```bash
   curl -i "http://localhost:5000/api/finance/currencies" ^
     -H "Authorization: Bearer <TOKEN>"
   ```
3. Выход:
   ```bash
   curl -X POST "http://localhost:5000/api/auth/logout" ^
     -H "Authorization: Bearer <TOKEN>"
   ```
4. Повторный запрос к защищённому эндпоинту с тем же токеном (ожидается `401`):
   ```bash
   curl -i "http://localhost:5000/api/finance/currencies" ^
     -H "Authorization: Bearer <TOKEN>"
   ```

## Сборка решения

```bash
dotnet build CurrencyTest.sln
```
