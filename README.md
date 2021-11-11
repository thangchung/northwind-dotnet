# northwind-dotnet projects

Implementing the specification of northwind application at https://github.com/thangchung/northwind-specs

# Setup Environment Variables

Create `.env` file with content as below

```bash
POSTGRES_USER=northwind
POSTGRES_PASSWORD=<your password>
POSTGRES_DB=northwind_db

ConnectionStrings__northwind_db=Server=localhost;Port=5432;Database=northwind_db;User Id=northwind;Password=<your password>;
Kafka__BootstrapServers=localhost:9092
Kafka__SchemaRegistryUrl=http://localhost:8081
AuditorGrpcUrl=https://localhost:5006
```

And now you are ready to start it

```bash
> tye run --watch
```

# Reference stuffs
- https://minimal-apis.github.io/