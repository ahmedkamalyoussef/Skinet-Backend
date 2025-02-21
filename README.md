# Skinet E-Commerce Platform

A modern e-commerce platform built with ASP.NET Core and Redis, featuring a robust shopping cart system and product management.

## Features

- Payment (using stripe)
- Product catalog with filtering and pagination
- Redis-powered shopping cart system
- RESTful API architecture
- Docker support for SQL Server and Redis
- Clean architecture implementation

## Technologies

- ASP.NET Core
- Entity Framework Core
- SQL Server
- Redis
- Docker

## Prerequisites

- .NET 9.0 SDK
- Docker Desktop
- Visual Studio 2022 or VS Code

## Installation

1. Clone the repository:
```bash
git clone https://github.com/yourusername/Skinet.git
cd Skinet
```

2. Start the Docker containers:
```bash
docker-compose up -d
```

3. Update the database:
```bash
dotnet ef database update --project Skinet.Infrastructure --startup-project Skinet.API
```

4. Run the application:
```bash
cd Skinet.API
dotnet run
```

## Project Structure

- **Skinet.API**: Web API layer and controllers
- **Skinet.Core**: Domain models and interfaces
- **Skinet.Infrastructure**: Data access and external services

## API Endpoints

### Products
- `GET /api/products`: Get all products
- `GET /api/products/{id}`: Get product by ID

### Shopping Cart
- `GET /api/cart/{id}`: Get cart by ID
- `POST /api/cart`: Update cart
- `DELETE /api/cart/{id}`: Delete cart

## Example Requests

### Update Cart
```json
POST /api/cart
{
    "id": "cart1",
    "items": [
        {
            "productId": 1,
            "productName": "Product Name",
            "price": 100,
            "quantity": 1,
            "brand": "Brand Name",
            "type": "Product Type",
            "pictureUrl": "image-url"
        }
    ]
}
```

## Configuration

The application uses two main connection strings in `appsettings.json`:

1. SQL Server:
```json
"DefaultConnection": "Server=localhost,1433;Database=skinet;User Id=SA;Password=Password@1;TrustServerCertificate=True"
```

2. Redis:
```json
"Redis": "localhost:6379"
```

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

