namespace MiniHttpServer.shared;

public static class Logger
{
    public static void Log(string text)
    {
        WriteToConsole(text, ConsoleColor.White);
    }

    public static void LogWarning(string text)
    {
        WriteToConsole(text, ConsoleColor.Yellow);
    }

    public static void LogError(string text)
    {
        WriteToConsole(text, ConsoleColor.Red);
    }

    private static void WriteToConsole(string text, ConsoleColor color)
    {
        string timestamp = DateTime.Now.ToString("HH-mm-ss");
        string message = $"{timestamp} - {text}";

        // Store the current console color
        ConsoleColor originalColor = Console.ForegroundColor;

        // Set the desired color and write the message
        Console.ForegroundColor = color;
        Console.WriteLine(message);

        // Restore the original console color
        Console.ForegroundColor = originalColor;
    }
}