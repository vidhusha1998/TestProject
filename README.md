Running API Tests with RestSharp and Xunit

Prerequisites
•	.NET SDK 6.0 or higher
•	Visual Studio, Rider, or a text editor (VS Code with C# support)
•	NuGet packages:
o	RestSharp
o	Xunit
o	Xunit.Runner.VisualStudio
o	Newtonsoft.Json

Steps to Run Tests
•	Clone the Repository
o	git clone https://github.com/your-repo/api-tests.git
o	cd api-tests

•	Run Tests
o	In IDE: Use Test Explorer to run tests in Visual Studio
	dotnet test

Tests Covered
•	GET all objects
•	POST new object
•	GET object by ID
•	PUT (update) object
•	DELETE object

Conclusion
•	This testing suite ensures that your API is functioning as expected for typical CRUD operations. Each test validates the interaction with the API, ensuring that objects are created, read, updated, and deleted correctly.

