# GameShoppingMvc

GameShoppingMvc is a modern ASP.NET Core MVC application designed for purchasing video games. It features a complete shopping experience with a user-friendly interface, administrative controls, and AI-powered recommendations.

## Features

- **Storefront**: Browse a catalog of games with search and genre filtering.
- **Shopping Cart**: Add games to cart, update quantities, and proceed to checkout.
- **User Accounts**: Secure registration and login using ASP.NET Core Identity.
- **Order Management**: View past orders and track order status.
- **Admin Dashboard**: secure area for administrators to Create, Read, Update, and Delete (CRUD) games.
- **AI Recommendations**: Get personalized game recommendations powered by Gemini AI.
- **Modern UI**: A responsive and visually appealing interface built with Bootstrap and custom styling.

## Technology Stack

- **Framework**: .NET 10.0 (ASP.NET Core MVC)
- **Database**: SQL Server
- **ORM**: Entity Framework Core 10.0
- **Authentication**: ASP.NET Core Identity
- **AI Integration**: Gemini API

## Getting Started

### Prerequisites

- .NET 10.0 SDK
- SQL Server

### Installation to run locally

1.  **Clone the repository**
    ```bash
    git clone https://github.com/yourusername/GameShoppingMvc.git
    cd GameShoppingMvc
    ```

2.  **Configure the Database**
    Update the `DefaultConnection` string in `appsettings.json` or use User Secrets to point to your SQL Server instance.

3.  **Apply Migrations**
    Run the following command to create the database and apply schema changes:
    ```bash
    dotnet ef database update
    ```

4.  **Seed Data** (Optional)
    The application includes a seeder to populate default data and create an admin user.

5.  **Run the Application**
    ```bash
    dotnet run --project GameShoppingMvcUI
    ```

6.  **Access the App**
    Open your browser and navigate to `https://localhost:7193` (or the port specified in the launch logs).

## Usage

- **User**: Register a new account to start shopping.
- **Admin**: Log in with administrative credentials to access the Admin panel for managing inventory.
