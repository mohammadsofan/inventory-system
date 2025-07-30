# Inventory System

The Inventory System is a lightweight C# console application designed to manage product inventories through a user-friendly command-line interface. It supports core CRUD operations—create, read, update, and delete on product records, with data persisted using JSON file-based storage.

The project is built following clean architecture principles, ensuring a clear separation of concerns across models, services, interfaces, and validation layers. This modular design makes the system easy to maintain, extend, and test.

Additionally, the system incorporates structured logging using Serilog, configured via appsettings.json. It supports multiple logging sinks, including rolling log files in both .txt and .json formats, as well as integration with Seq for centralized log management and monitoring. This enhances observability and facilitates easier debugging and diagnostics.

## Features
- Add new products
- Update existing products
- Delete products
- List products
- JSON-based file storage
- Logging with serilog
  
## Technologies Used
- C# (.NET)
- File I/O
- Serilog logging with txt files, json files, and Seq
- JSON Serialization
- Console UI

## Folder Structure
- `Models/` – Product models
- `Services/` – Business logic
- `Interfaces/` – Abstractions for services
- `Validators/` – Input validation logic
- `Dtos/` – Result classes for responses
- `Utils/` - Helper classes
- `Enums/` - Enums
## Getting Started
1. Clone the repo
2. Open in Visual Studio / VS Code
3. Build and run

