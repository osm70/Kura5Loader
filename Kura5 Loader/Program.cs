using System.Diagnostics;

static void RunProcess(string fileName, string arguments = "")
{
    try
    {

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            UseShellExecute = true,
            CreateNoWindow = false
        };

        using (Process process = Process.Start(startInfo))
        {
            if (process == null)
            {
                Console.WriteLine($"Warning: Failed to start process: {fileName}");
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("Keep the Sun always in your heart!");
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }

        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\nError during process execution ({Path.GetFileName(fileName)}): {ex.Message}");
        Console.WriteLine("Ensure that the executable is present at the expected path.");
    }
}

Console.Title = "Kura5 Loader";
Console.WriteLine("**********************************************");
Console.WriteLine("                       ____  ");
Console.WriteLine("  /\\ /\\_   _ _ __ __ _| ___| ");
Console.WriteLine(" / //_/ | | | '__/ _` |___ \\ ");
Console.WriteLine("/ __ \\| |_| | | | (_| |___) |");
Console.WriteLine("\\/  \\/ \\__,_|_|  \\__,_|____/ ");
Console.WriteLine("                             ");
Console.WriteLine("**********************************************");

Console.WriteLine("Select a location for WeatherSync.");
Console.WriteLine("If WeatherSync is enabled (in the game's settings - after starting the game itself, not in the loader):");
Console.WriteLine("the selected location will determine live weather and the in-game time.");
Console.WriteLine("If WeatherSync is disabled:");
Console.WriteLine("the selected location will determine just the time and the in-game weather will be randomly generated.");
Console.WriteLine("(WeatherSync needs internet. Disable it to play offline.");
Console.WriteLine();

Console.WriteLine("0. Current real location");
Console.WriteLine("1. Berlin Germany");
Console.WriteLine("2. Sydney Australia");
Console.WriteLine("3. Tokyo Japan");
Console.WriteLine("4. Beijing China");
Console.WriteLine("5. Moscow Russia");
Console.WriteLine("6. New York USA");
Console.WriteLine("7. New Delhi India");
Console.WriteLine("8. Cairo, Egypt");
Console.WriteLine("9. Wellington, New Zealand");
Console.WriteLine("A. Fairbanks, USA");
Console.WriteLine("B. Yakutsk Siberia (Russia)");
Console.WriteLine("C. Sahara desert");
Console.WriteLine("D. Amazon rainforest");
Console.WriteLine("E. Antarctica, Arturo Parodi Station");
Console.WriteLine("F. Line Islands, Kiribati");
Console.WriteLine();

char X='0';
bool isValidChoice = false;

while (!isValidChoice)
{
    Console.Write("Enter a number (or letter) to continue: ");
    string input = Console.ReadLine()?.Trim().ToUpper() ?? "";

    if (input.Length == 1)
    {
        X = input[0];

        if ((X >= '0' && X <= '9') || (X >= 'A' && X <= 'F'))
        {
            isValidChoice = true;
        }
    }

    if (!isValidChoice)
    {
        Console.WriteLine("Invalid selection. Please enter a single character corresponding to an option (0-9, A-F).");
        Console.WriteLine();
    }
}

string baseDir = AppDomain.CurrentDomain.BaseDirectory;
string sourceDirName = X.ToString();
string sourceFile = Path.Combine(baseDir, "Data", "Kura5_Data", "Files", sourceDirName, "Assembly-CSharp.dll");
string destinationFile = Path.Combine(baseDir, "Data", "Kura5_Data", "Managed", "Assembly-CSharp.dll");
string kura5ExePath = Path.Combine(baseDir, "Data", "Kura5.exe");
string runAsDateExePath = Path.Combine(baseDir, "Data", "RunAsDate.exe");
try
{

    if (!File.Exists(sourceFile))
    {
        Console.WriteLine($"Error: Source DLL not found at {sourceFile}. Check if Data/Kura5_Data/Files/{X}/ exists.");
        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
        return;
    }

    File.Copy(sourceFile, destinationFile, true);
}
catch (Exception ex)
{
    Console.WriteLine($"\nFATAL ERROR during file copy: {ex.Message}");
    Console.WriteLine("Press any key to exit.");
    Console.ReadKey();
    return;
}

if (X != '0')
{

    string timeFilePath = Path.Combine(baseDir, "Data", "Kura5_Data", "Files", sourceDirName, "time.txt");

    if (!File.Exists(timeFilePath))
    {
        Console.WriteLine($"Error: Time configuration file not found at {timeFilePath}.");
        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
        return;
    }

    try
    {
        string timeZoneName = File.ReadAllText(timeFilePath).Trim();
        TimeSpan localOffset = TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.Now);

        TimeZoneInfo targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);
        TimeSpan targetOffset = targetTimeZone.GetUtcOffset(DateTimeOffset.Now);


        double y = (targetOffset - localOffset).TotalMinutes;
        int yInt = (int)Math.Round(y);

        Console.WriteLine($"Local Time Zone Offset: {localOffset.TotalMinutes} min.");
        Console.WriteLine($"Target Time Zone Offset ({timeZoneName}): {targetOffset.TotalMinutes} min.");
        Console.WriteLine($"Calculated time adjustment (Y): {yInt} minutes.");
        string runAsDateArguments = $"/immediate /movetime Minutes:{yInt} \"{kura5ExePath}\"";
        Console.WriteLine("Starting game...");

        RunProcess(runAsDateExePath, runAsDateArguments);
    }
    catch (TimeZoneNotFoundException ex)
    {
        Console.WriteLine($"Error: The time zone '{ex.Message}' specified in time.txt was not recognized by the system.");
        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"A critical error occurred during time setup: {ex.Message}");
        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
    }
}
else
{
    Console.WriteLine("Starting game...");
    RunProcess(kura5ExePath);
}