Blazor has a built-in logging system that can be exploited by injecting ILogger into components.
This logging system shows the logs to the developer in the form of a console during software development.
However, after producing and publishing the application, this console no longer exists.
The method implemented in this project can be used to store logs in the database via ILogger.
