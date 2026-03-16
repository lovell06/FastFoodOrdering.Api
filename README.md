FastFoodOrderingApi/
│
├── Controllers/
│ ├── AuthController.cs
│ ├── ProductsController.cs
│ └── OrdersController.cs
│
├── Models/
│ ├── User.cs
│ ├── Product.cs
│ ├── Order.cs
│ └── OrderDetail.cs
│
├── Enums/
│ ├── OrderStatus.cs
│ ├── PaymentMethod.cs
│ └── PaymentStatus.cs
│
├── DTOs/
│ ├── Auth/
│ │ ├── RegisterRequest.cs
│ │ ├── LoginRequest.cs
│ │ └── AuthResponse.cs
│ │
│ ├── Product/
│ │ └── ProductResponse.cs
│ │
│ └── Order/
│ ├── CreateOrderRequest.cs
│ ├── CreateOrderItemRequest.cs
│ ├── ConfirmOrderRequest.cs
│ ├── PayOrderRequest.cs
│ ├── OrderResponse.cs
│ └── OrderDetailResponse.cs
│
├── Data/
│ ├── ApplicationDbContext.cs
│ └── SeedData.cs
│
├── Services/
│ ├── Interfaces/
│ │ ├── IAuthService.cs
│ │ ├── IProductService.cs
│ │ └── IOrderService.cs
│ │
│ └── Implementations/
│ ├── AuthService.cs
│ ├── ProductService.cs
│ └── OrderService.cs
│
├── Mappings/
│ └── MappingProfile.cs
│
├── Helpers/
│ └── ApiResponse.cs
│
├── Migrations/
│
├── Properties/
│
├── appsettings.json
├── appsettings.Development.json
├── Program.cs
└── FastFoodOrderingApi.csproj

orderdetail

- ProductId
- OrderId
