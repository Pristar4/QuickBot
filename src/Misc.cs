using System;

namespace QuickBot;

/// <summary>
///  SyncOut is a class that provides a thread-safe way to write to the console.
/// by using a lock object.
/// </summary>
public static class SyncOut {
    private static readonly object LockObject = new();
    public static void WriteLine(string message) {
        lock (LockObject) {
            Console.WriteLine(message);
        }
    }
}