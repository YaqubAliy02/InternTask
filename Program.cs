using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

public class Program
{
    private static readonly string SaveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Screenshots");

    static void Main()
    {
        Directory.CreateDirectory(SaveDirectory);
        Console.WriteLine("Screenshot program started...");
        Console.WriteLine($"Screenshots will be saved to: {SaveDirectory}");

            TakeScreenshot();
    }

    static void TakeScreenshot()
    {
        string filePath = Path.Combine(SaveDirectory, $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            CaptureWindowsScreenshot(filePath);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            CaptureLinuxScreenshot(filePath);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            CaptureMacScreenshot(filePath);
        }
        else
        {
            Console.WriteLine("Unsupported OS.");
        }
    }

    static void CaptureWindowsScreenshot(string filePath)
    {
        try
        {
            int screenWidth = GetSystemMetrics(78);
            int screenHeight = GetSystemMetrics(79);
            int left = GetSystemMetrics(76);
            int top = GetSystemMetrics(77);

            using Bitmap bitmap = new Bitmap(screenWidth, screenHeight);
            using Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(left, top, 0, 0, bitmap.Size);
            bitmap.Save(filePath, ImageFormat.Png);
            Console.WriteLine($"Windows screenshot saved: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error taking screenshot on Windows: {ex.Message}");
        }
    }

    static void CaptureLinuxScreenshot(string filePath)
    {
        ExecuteCommand($"import -window root {filePath}");
        Console.WriteLine($"Linux screenshot saved: {filePath}");
    }

    static void CaptureMacScreenshot(string filePath)
    {
        ExecuteCommand($"screencapture -x {filePath}");
        Console.WriteLine($"MacOS screenshot saved: {filePath}");
    }

    static void ExecuteCommand(string command)
    {
        try
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{command}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process process = Process.Start(psi);
            process?.WaitForExit();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing command: {ex.Message}");
        }
    }

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);
}