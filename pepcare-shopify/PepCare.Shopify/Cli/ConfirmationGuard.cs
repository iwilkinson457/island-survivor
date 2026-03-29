namespace PepCare.Shopify.Cli;

/// <summary>
/// Requires explicit user confirmation before destructive/write operations.
/// </summary>
public static class ConfirmationGuard
{
    public static bool Confirm(string prompt, bool defaultNo = true)
    {
        var hint = defaultNo ? "[y/N]" : "[Y/n]";
        Console.Write($"\n⚠  {prompt} {hint}: ");
        var input = Console.ReadLine()?.Trim().ToLower();
        if (defaultNo)
            return input == "y" || input == "yes";
        return input != "n" && input != "no";
    }

    public static void RequireConfirmOrAbort(string prompt)
    {
        if (!Confirm(prompt))
        {
            Console.WriteLine("Aborted.");
            Environment.Exit(0);
        }
    }
}
