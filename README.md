# SimpleDriveProject

Welcome to SimpleDriveProject! This is an ASP.NET Core web application that demonstrates how to upload, store, and retrieve files from different storage backends including local file systems, FTP servers, and S3-compatible services. The project integrates a robust API system for managing file storage, handling metadata in a SQL database, and using JWT for secure authentication.

# Getting Started 
To get started with this project, follow these steps:

### Prerequisites
Ensure you have the following installed before starting:

- [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0) (or latest version)
- [Visual Studio](https://visualstudio.microsoft.com/) 2022 (or any IDE of your choice)
- SQL Server (for the database connection)
- FTP Server (if you plan to use the FTP functionality)

### Installation
1. Clone the Repository or project to your local machine by running the following command in your terminal or command prompt:

        git clone https://github.com/Abdulakhtar/SimpleDriveProject.git

2. Install Required Packages by navigating to the project directory

        cd SimpleDriveProject

3. Install the required packages:
   
        dotnet restore

### Update Configuration

Open the appsettings.json file and update the following:

  - **Connection String**: Modify the DefaultConnection to point to your SQL server.
  - **S3 Credentials**: Add your S3 access key, secret key, bucket name, and endpoint.
  - Modify the **Program.cs** file if needed to reflect the configurations.
  - Update the **launchSettings.json** with the correct IP address for local testing if required.
  - **FTP Configuration**: Make sure you have configured an FTP server on your local machine. The FTP server credentials will be used in the API for file transfer operations. You can refer to this guide on FTP command line for more information.

### Run the Application:

1. Using .NET CLI run the following command to start the application:
     
        dotnet run

2. Using Visual Studio:

  - Press F5 or the green play button to run the application.
  - **Accessing the Application**: Once the application is running, you can access it at https://localhost:5001 (or the appropriate port configured). The Swagger UI will be available for testing the API endpoints.

## Using the API

### Authorization: 

  To interact with the API, first obtain a JWT token by using the GET method in the /api/auth endpoint. The token is then used for authentication when calling other endpoints.

### Testing APIs:

  - Use the POST /api/files/upload to upload files to local storage, FTP, or S3.
  - Use GET /api/files/{id} to retrieve a file based on its ID.
  - Swagger: Swagger will automatically provide an interface for testing the APIs. You can test the file upload, retrieval, and FTP operations once you’ve authorized the API.


## Conclusion
SimpleDriveProject is a versatile API-based application designed to handle file uploads and retrievals from multiple storage backends. The application combines local file storage with a database, FTP integration, and S3 compatibility to offer a flexible solution for managing your files securely. With JWT-based authentication, the project ensures that your APIs are secure and easy to test using Swagger.

This project provides an excellent starting point for developers looking to integrate multiple file storage solutions in their own applications.

## Contributing
If you'd like to contribute to this project, feel free to fork the repository and submit a pull request with your changes in a new branch. Please follow the coding guidelines and ensure that tests are added for new features.

### License

This project is licensed under the MIT License - see the LICENSE file for details.

### NOTE:

Feel free to customize this README file further to suit your project's specific requirements or add any additional information you find necessary for my _**Imporvement**_
