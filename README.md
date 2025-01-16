# GoldenRaspberryManager

GoldenRaspberryManager is an API focused on managing movies nominated for and winners of the Golden Raspberry Awards, also known as the "Razzie Awards." This application allows importing, managing, and querying information related to the "worst movies" of all time, utilizing programming best practices such as DDD, SOLID, and Clean Code.

## Features

### CSV Data Import:
- Import movie information from CSV files, converting each row into domain instances.
- Perform data validations during the import process.

### CRUD Operations for Movies:
- Create, Read, Update, and Delete movie records.

### Domain Validations:
- Ensure data consistency when adding or updating information.
- Title, year, and producers are mandatory fields.

### Logs and Notifications:
- Record errors and validation messages.

### RESTful API:
- Endpoints for all CRUD operations and CSV import.

## Technologies Used
- **.NET 6/7**: Core framework for building the API.
- **Entity Framework Core**: Data persistence.
- **FluentValidation**: Robust and centralized data validation.
- **CsvHelper**: Handling and reading CSV files.
- **Dependency Injection**: For managing project dependencies.
- **BDD (Behavior-Driven Development)**: Scenarios described in Gherkin format to guide development.

## Requirements
- .NET SDK 6.0 or higher
- Database (e.g., SQLite, PostgreSQL, or SQL Server)
- Code editor (e.g., Visual Studio, Visual Studio Code)

## Installation

1. **Clone the repository:**
   ```bash
   git clone [URL](https://github.com/your-username/golden-raspberry-manager.git)
   ```

2. **Navigate to the project directory:**
   ```bash
   cd golden-raspberry-manager
   ```

3. **Restore project dependencies:**
   ```bash
   dotnet restore
   ```

4. **Apply Migrations to create the database:**
   ```bash
   dotnet ef database update
   ```

5. **Start the application:**
   ```bash
   dotnet run
   ```

## How to Use

### 1. Import CSV
**Upload a CSV file with the following columns:**

**Year**: Year of the movie.
**Title**: Movie title.
**Studios**: Studios (comma-separated if multiple).
**Producers**: Producers (comma-separated).
**IsWinner**: Indicates whether the movie was a winner (true/false).
#### Example CSV:
   ```csv
   Year,Title,Studios,Producers,IsWinner
1980,Movie A,Studio A,Producer A,true
1981,Movie B,Studio B,Producer B,false
   ```
   
##### Use the endpoint:
   ```HTTP
   POST /api/movies/import
   ```

### 2. Main Endpoints

Create a Movie
   ```HTTP
   POST /api/movies
Content-Type: application/json

{
    "year": 1980,
    "title": "Movie A",
    "studios": ["Studio A"],
    "producers": ["Producer A"],
    "isWinner": true
}
   ```
   
Retrieve all Movies
   ```HTTP
   GET /api/movies
   ```

Update a Movie
   ```HTTP
   PUT /api/movies/{id}
Content-Type: application/json

{
    "year": 1981,
    "title": "Movie B",
    "studios": ["Studio B"],
    "producers": ["Producer B"],
    "isWinner": false
}
   ```
   
Delete a Movie
   ```HTTP
   DELETE /api/movies/{id}
   ```
   
## Contribution
### Contributions are welcome! To contribute:

1. **Fork the repository.**
2. **Create a new branch with your feature:**
   ```bash
   git checkout -b feature/new-feature
   ```
3. **Commit you changes:**
   ```bash
   git commit -m "Add new feature"
   ```
4. **Push to the remote repository:**
   ```bash
   git push origin feature/new-feature
   ```
5. **Open a pull request.**

## License
Free to use