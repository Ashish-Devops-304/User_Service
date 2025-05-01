# User Microservice

## Overview

The User Microservice API offers a range of endpoints to facilitate user account management, including user creation, authentication, and the management of user addresses and payments.

## User Account Endpoints
**Description**: This group of endpoints provides functionalities to manage user accounts, including creation, authentication, retrieval, updating, and deletion. The "Verify User" endpoint allows the verification of a user's account.

### 1. Create User Account

- **POST** `/api/user`
  - **Description**: Creates a new user account.

### 2. Get All Users

- **GET** `/api/user`
  - **Description**: Retrieves a list of all user accounts.

### 3. User Login

- **POST** `/api/user/login`
  - **Description**: Authenticates a user based on provided credentials.

### 4. Manage User

- **GET** `/api/user/{id}`
  - **Description**: Retrieves detailed information about a user by ID.
  
- **PUT** `/api/user/{id}`
  - **Description**: Updates user details by ID.
  
- **DELETE** `/api/user/{id}`
  - **Description**: Deletes a user account by ID.

### 5. Verify User

- **GET** `/api/user/{userId}/verify`
  - **Description**: Verifies the user's account.

## User Address Endpoints
**Description**: This group of endpoints manages user addresses, allowing the addition, retrieval, updating, and deletion of addresses. The "Verify User Address" endpoint verifies a specific user address.

### 1. Manage User Addresses

- **POST** `/api/user/{userId}/address`
  - **Description**: Adds a new address for the specified user.

- **GET** `/api/user/{userId}/address`
  - **Description**: Retrieves all addresses associated with a user.

- **GET** `/api/user/{userId}/address/{addressId}`
  - **Description**: Retrieves detailed information about a specific user address by ID.

- **PUT** `/api/user/{userId}/address/{addressId}`
  - **Description**: Updates a user address by ID.

- **DELETE** `/api/user/{userId}/address/{addressId}`
  - **Description**: Deletes a user address by ID.

### 2. Verify User Address

- **GET** `/api/user/{userId}/address/{addressId}/verify`
  - **Description**: Verifies the specified user address.

## User Payment Endpoints
**Description**: This group of endpoints manages user payments, allowing the addition, retrieval, updating, and deletion of payment methods. The "Verify User Payment" endpoint verifies a specific user payment method.

### 1. Manage User Payments

- **POST** `/api/user/{userId}/payment`
  - **Description**: Adds a new payment method for the specified user.

- **GET** `/api/user/{userId}/payment`
  - **Description**: Retrieves all payment methods associated with a user.

- **GET** `/api/user/{userId}/payment/{paymentId}`
  - **Description**: Retrieves detailed information about a specific user payment method by ID.

- **PUT** `/api/user/{userId}/payment/{paymentId}`
  - **Description**: Updates a user payment method by ID.

- **DELETE** `/api/user/{userId}/payment/{paymentId}`
  - **Description**: Deletes a user payment method by ID.

### 2. Verify User Payment

- **GET** `/api/user/{userId}/payment/{paymentId}/verify`
  - **Description**: Verifies the specified user payment method.

## Installation

### Prerequisites

Ensure that you have the following prerequisites installed on your machine:
- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Docker](https://www.docker.com/get-started)
- [PostgreSQL](https://www.postgresql.org/)

### Setup

**1. Clone the repository:**
  ```bash
   git clone https://github.com/Ashish-Devops-304/User_Service
   cd UserService
   ```
**2. Update DB connection string:**
    Open appsettings.json and update the database connection string.

**3.Build and Run the application locally:**
  ```bash
   dotnet build
   dotnet run
   ```
   
# License

This project is licensed under the [MIT License](LICENSE).

# Usage
For detailed information on how to use the API, refer to the API documentation available at [http://localhost:5004/swagger](http://localhost:5004/swagger/index.html).
