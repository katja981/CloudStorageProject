# flyimager

## Quick start

**Start database**

```sh
docker compose up
```

**Start server**

```sh
dotnet run --project server/Api
```

**Start client**

```sh
npm run dev --prefix client
```

**Generate API client**

Make sure your backend is running.
Then from repository root, run:

```sh
npx swagger-typescript-api generate -p http://localhost:5000/openapi/v1.json -o ./client -n src/generated-client.ts
```

Instantiate it like this in your React components:

```ts
new Api({ baseUrl: '/api' })
```
