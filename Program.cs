using MiniHttpServer.shared;
using System.Net;
using System.Text;
using System.Text.Json;
using static MiniHttpServer.shared.Logger;

try
{
    string settings = File.ReadAllText("settings.json");
    SettingsModel settingsModel = JsonSerializer.Deserialize<SettingsModel>(settings)!;

    HttpListener server = new();
    // установка адресов прослушки
    server.Prefixes.Add("http://" + settingsModel.Domain + ":" + settingsModel.Port + "/");
    server.Start(); // начинаем прослушивать входящие подключения

    Log("Server is started");

    var consoleTask = Task.Run(() =>
    {
        while (true)
        {
            string? command = Console.ReadLine();
            if (command == "/stop")
            {
                Log("Server stopped by command");
                server.Stop();
            }
        }
    });

    while (true)
    {
        Log("Server is awaiting for request");

        // получаем контекст

        var context = await server.GetContextAsync();
        var response = context.Response;
        // отправляемый в ответ код html возвращает
        try
        {
            string responseText = File.ReadAllText(settingsModel.StaticDirectoryPath + "index.html");
            byte[] buffer = Encoding.UTF8.GetBytes(responseText);
            // получаем поток ответа и пишем в него ответ
            response.ContentLength64 = buffer.Length;
            using Stream output = response.OutputStream;
            // отправляем данные
            await output.WriteAsync(buffer);
            await output.FlushAsync();

            Log("Запрос обработан");
        }
        catch (DirectoryNotFoundException e)
        {
            LogError("static folder not found");
            server.Stop();
            LogWarning("Server is stopped");
            break;
        }
        catch (FileNotFoundException e)
        {
            LogError("index.html is not found in static folder");
            server.Stop();
            LogWarning("Server is stopped");
            break;
        }
        catch (Exception e)
        {
            LogError("There is an exception: " + e.Message);
            server.Stop();
            LogWarning("Server is stopped");
            break;
        }
    }

}
catch (Exception e) when (e is DirectoryNotFoundException or FileNotFoundException)
{
    LogError("settings are not found");
}
catch (JsonException e)
{
    LogError("settings.json is incorrect");
}
catch (Exception e) { LogError("There is an exception: " + e.Message); }
