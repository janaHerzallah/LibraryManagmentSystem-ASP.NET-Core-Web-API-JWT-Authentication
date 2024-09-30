# Library Management System

This project implements software to manage libraries with multiple branches, allowing members to borrow books. The project includes APIs for managing users (admins and members), books, authors, categories, library branches, as well as borrowing and returning operations.

All APIs are authorized based on user roles using the JWT Bearer Authentication scheme. The APIs are directly connected to the library management system running on PostgreSQL through pgAdmin.

## Packages Used
Below are the packages required for setting up the system:

![image](https://github.com/user-attachments/assets/c78bc6da-32b0-4a01-9f7e-bee7f59c8cdc)


## Features
- User management: Admins and members registering their accounts and logging in 
- Book management: Get All , GetById , search by title ,filter by author ID ,Add, update, delete, and soft-delete books
- Author management: Get All , GetById ,Add, update, delete, and soft-delete authors
- Categories management : Get All , GetById ,Add, update, delete, and soft-delete categories
- Branches managment :Get All , GetById, Add, update, delete, and soft-delete branches
- Borrowing and Returning Books By members within rules
- Member management : Get All , GetById ,Add, update, delete, and soft-delete memebrs
- Book borrowing and returning operations
- Multiple branches support
- JWT-based authentication and role-based authorization
- PostgreSQL database integration
- Excel Integration for data import/export using EEPlus

## Testing
The application can be tested via Swagger, which supports:
- **Add**, **Update**, **Delete**, **Soft-Delete** functions for all entities
- **Get Active**, **Get All**, **Get by ID** for all entities
- **Borrow a Book**, **Return a Book**
- **Search a Book By Title or AuthorName**
- **Filter Books By Availability**

## Installation

### Prerequisites
- [.NET 6 SDK or higher](https://dotnet.microsoft.com/download/dotnet/6.0)
- PostgreSQL
- pgAdmin

### Setup
1. Clone the repository:
    ```bash
    git clone https://github.com/your-username/library-management-system.git
    ```

2. Navigate to the project directory:
    ```bash
    cd library-management-system
    ```

3. Install the necessary packages:
    ```bash
    dotnet restore
    ```

4. Configure the PostgreSQL connection string in `appsettings.json`.

5. Run database migrations:
    ```bash
    dotnet ef database update
    ```

6. Start the application:
    ```bash
    dotnet run
    ```

7. Access the Swagger UI for API testing at:
    ```plaintext
    http://localhost:5000/swagger
    ```

## Technologies
- **.NET 8** – Backend framework
- **PostgreSQL** – Database
- **Entity Framework Core** – ORM for database interactions
- **JWT Bearer Authentication** – Secure authentication
- **Swagger** – API testing and documentation interface

## License
This project is licensed under the MIT License.
