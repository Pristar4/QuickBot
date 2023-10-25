#region

using System;

#endregion

namespace QuickBot;

internal static class Program {
    private static int Main(string[] args) {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(
                "  ___        _      _    ____        _   \n" +
                " / _ \\ _   _(_) ___| | _| __ )  ___ | |_ \n" +
                "| | | | | | | |/ __| |/ /  _ \\ / _ \\| __|\n" +
                "| |_| | |_| | | (__|   <| |_) | (_) | |_ \n" +
                @" \__\_\\__,_|_|\___|_|\_\____/ \___/ \__|");
        Console.ResetColor();
        Console.WriteLine(" v" + VersionInfo.GetProjectVersion());

        try {
            UciExtensions.Uci.Initialize();
            UciExtensions.UciLoop(args);
        }
        catch (Exception e) {
            Console.WriteLine(e.Message);
            Console.WriteLine(e); // for Debugging
        }

        return 0;
    }
}