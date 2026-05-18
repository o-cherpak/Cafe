Cafe API Documentation


Fully functional REST API cafe ordering system with a customer kiosk flow, staff management panel, a bonus points economy, and a promotion system with two discount types. The API is live, documented, and ready to be extended with a React frontend. This project was built as a learning exercise while studying .NET backend development. The goal was to go beyond simple CRUD and build something that resembles a real-world system - with proper layering, authentication, business logic, and deployment. Over the course of the project I learned and applied: clean architecture with Repository and Unit of Work patterns, Entity Framework Core with Code First migrations and Fluent API configuration, JWT authentication with role-based authorization, Docker and Docker Compose for containerization, automated testing with xUnit covering both unit and integration scenarios, and deploying a containerized .NET application with PostgreSQL to a cloud platform.

## 📸 Screenshots

<table border="0">
  <tr>
    <td align="center" width="50%">
      <strong>Customer Create</strong><br>
      <img src="https://github.com/user-attachments/assets/367296d0-ff67-4223-9d06-6651a0544177" alt="customer_create" width="100%" />
    </td>
    <td align="center" width="50%">
      <strong>Login</strong><br>
      <img src="https://github.com/user-attachments/assets/c64a31a3-f763-4df5-8faa-6c095f83aa84" alt="login" width="100%" />
    </td>
  </tr>
  <tr>
    <td align="center" width="50%">
      <strong>Get Orders</strong><br>
      <img src="https://github.com/user-attachments/assets/6f71d0f5-c1ce-4439-bb2d-78b6a59a70f4" alt="order_get" width="100%" />
    </td>
    <td align="center" width="50%">
      <strong>Tests</strong><br>
      <img src="https://github.com/user-attachments/assets/d1ac7a78-40b4-4a08-8db8-0915245a695a" alt="tests" width="100%" />
    </td>
  </tr>
</table>

## 🌐 API Endpoints

### 🔐 Auth
| Method | Endpoint | Access | Description |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/auth/register` | Public | Register a new user |
| `POST` | `/api/auth/login` | Public | Login and receive JWT token |

### ☕ Menu
| Method | Endpoint | Access | Description |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/menuitem` | Public | Get all items (filter by `?category=`) |
| `GET` | `/api/menuitem/{id}` | Public | Get item by ID |
| `POST` | `/api/menuitem` | Admin | Create menu item |
| `PUT` | `/api/menuitem/{id}` | Admin | Update menu item |
| `DELETE` | `/api/menuitem/{id}` | Admin | Delete menu item |

### 👥 Customers
| Method | Endpoint | Access | Description |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/customer` | Admin, Barista | Get all customers |
| `GET` | `/api/customer/{id}` | Admin, Barista, Customer | Get customer by ID |
| `GET` | `/api/customer/by-email` | Admin, Barista, Customer | Get customer by email |
| `POST` | `/api/customer` | Admin, Barista | Create customer |
| `PUT` | `/api/customer/{id}` | Admin, Barista, Customer | Update customer |
| `DELETE` | `/api/customer/{id}` | Admin | Delete customer |

### 📦 Orders
| Method | Endpoint | Access | Description |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/order` | Admin, Barista | Get all orders (filter by `?customerId=`, `?status=`) |
| `GET` | `/api/order/{id}` | Admin, Barista, Customer | Get order by ID |
| `GET` | `/api/order/by-customer` | Admin, Barista, Customer | Get orders by customer ID (filter by `?customerId=`) |
| `POST` | `/api/order` | Admin, Barista, Customer | Create order |
| `PUT` | `/api/order/{id}` | Admin, Barista | Update order status |

### 🏷️ Promotions
| Method | Endpoint | Access | Description |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/promotion` | Public | Get all promotions |
| `GET` | `/api/promotion/active` | Public | Get active promotions |
| `GET` | `/api/promotion/{id}` | Public | Get promotion by ID |
| `POST` | `/api/promotion` | Admin | Create promotion |
| `PUT` | `/api/promotion/{id}` | Admin | Update promotion |

### 🎁 Customer Promotions
| Method | Endpoint | Access | Description |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/customerpromotion` | Admin, Barista | Get all customer promotions |
| `GET` | `/api/customerpromotion/{id}` | Admin, Barista, Customer | Get by ID |
| `GET` | `/api/customerpromotion/by-customer` | Admin, Barista, Customer | Get by customer ID (filter by `?customerId=`) |
| `GET` | `/api/customerpromotion/by-customer-promotion` | Admin, Barista, Customer | Get by customer and promotion ID (filter by `?customerId=`, `?promotionId=`) |
| `GET` | `/api/customerpromotion/by-order` | Admin, Barista, Customer | Get by order ID (filter by `?orderId=`) |
| `POST` | `/api/customerpromotion` | Admin, Barista, Customer | Buy a promotion with bonus points |
| `DELETE` | `/api/customerpromotion/{id}` | Admin | Delete customer promotion |

---

## 🚀 Running Locally

### Prerequisites
- **.NET 10 SDK**
- **Docker Desktop** or Postgres SQL

### With Docker Compose

1. Clone the repository:
   ```bash
   git clone https://github.com/o-cherpak/Cafe.git
   cd Cafe
2. Create a .env file in the root directory
   ```bash
    DB_USER=your_db_user
    DB_PASSWORD=your_db_password
    JWT_SECRET=your_minimum_32_characters_long_secret
3. Run Docker Compose
   ```bash
   docker compose up --build
API Access Points: <br>
API Base URL: http://localhost:5285
Scalar Docs: http://localhost:5285/scalar/v1
