﻿using System.Diagnostics;
using Rudzoft.ChessLib;
using Rudzoft.ChessLib.Factories;
using Rudzoft.ChessLib.Fen;
using Rudzoft.ChessLib.Protocol.UCI;
using Rudzoft.ChessLib.Types;

namespace Pristar4.ChessEngine;

public static class UciExtensions
{
    // FEN string for the initial position in standard chess
    private const string StartFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    public static readonly Uci Uci = new();

    /// <summary>
    ///     Position() is called whenever the engine receieces the "position" UCI command.
    ///     It sets up the position described in the fenstring or the initial position of the
    ///     game.("startpos")
    ///     and then makes the moves given in the following move list ("moves")
    ///     Todo: maybe use streams instead of strings so we can consume the input
    /// </summary>
    /// <param name="uci"></param>
    /// <param name="game"></param>
    /// <param name="input"></param>
    /// <param name="states"></param>
    private static void Position(this IUci uci, IGame game,
        string? input, Stack<State> states)
    {
        // Split the input string
        var tokensList = new List<string>();

        if (!string.IsNullOrEmpty(input))
        {
            tokensList = input.Split(' ').ToList();
        }

        FenData? fen = null;
        int i = 0;


        // Check if position is startpos
        if (tokensList[0] == "position" && tokensList[1] == "startpos")
        {
            fen = StartFen;
            game.Pos.Set(fen, game.Pos.ChessMode, new State());
            i = 2;
        }
        // Check if position is in fen format
        else if (tokensList[0] == "position" && tokensList[1] == "fen")
        {
            i = 2;

            while (i < tokensList.Count && tokensList[i] != "moves")
            {
                fen += tokensList[i] + " ";
                i++;
            }

            // Drop the old state and create a new one
            states = new Stack<State>();
            game.Pos.Set(fen, game.Pos.ChessMode, new State(), true);
        }

        // Process moves irrespective of "startpos" or "fen"

        List<string> movesString = new();

        if (i < tokensList.Count && tokensList[i] == "moves")
        {
            i++;

            for (; i < tokensList.Count; i++)
            {
                string uciMove = tokensList[i];
                movesString.Add(uciMove);
            }
        }

        foreach(var unused in uci.MovesFromUci(game.Pos, states, movesString))
        {
            // move is discarded, but each iteration will process the move. 
            // so the Move is being made on the game.Pos
        }
    }

    /// <summary>
    ///     Uci loop waits for an  command from the Input, parses it, and then calls the appropriate
    ///     function.
    ///     It also intercepts an end-of-file (EOF), to ensure a graceful exit. if the GUI dies
    ///     unexpectedly.
    ///     When called with some command-line arguments, the function returns immediately after the
    ///     command is executed
    ///     In addition to the UCI commands, the function also handles the following commands:
    ///     d - display board
    /// </summary>
    /// <param name="args">
    ///     Command-line arguments
    /// </param>
    public static void UciLoop(string[] args)
    {
        const string startFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        var game = GameFactory.Create(startFen);

        var states = new Stack<State>();
        string token = "", cmd = string.Join(" ", args);

        do
        {
            if (args.Length == 0)
            {
                cmd = Console.ReadLine() ?? "quit"; // Wait for an input or an end-of-file (EOF)
                if (string.IsNullOrEmpty(cmd))
                    continue; // Ignore empty strings and wait for the next input
            }

            string[] inputs = cmd.Split(' ');
            token = inputs[0];

            switch (token)
            {
                case "quit":
                    // close program
                    break;
                case "stop":
                    SyncOut.WriteLine(Uci.BestMove(Move.EmptyMove, Move.EmptyMove));
                    break;
                case "uci":
                    SyncOut.WriteLine("id name QuickBot v" + VersionInfo.GetProjectVersion(false));
                    SyncOut.WriteLine("id author Felix Jung");
                    SyncOut.WriteLine(Uci.ToString()); //TODO: print the current Best Move here
                    SyncOut.WriteLine(Uci.UciOk());
                    break;
                case "setoption":
                    // set option
                    break;
                case "go":
                    Go(Uci, game, cmd, states);
                    // start search
                    break;
                case "position":
                    Position(Uci, game, cmd, states);
                    // set position
                    break;
                case "ucinewgame":
                    // new game
                    // clear search
                    break;
                case "isready":
                    SyncOut.WriteLine(Uci.ReadyOk());
                    break;
                case "perft":
                    // create a new stopwatch
                    Stopwatch stopwatch = new Stopwatch();
                    // start stopwatch
                    stopwatch.Start();
                    // execute perft
                    Console.Write(game.Perft(6));
                    // stop stopwatch
                    stopwatch.Stop();
                    // write execution time to console
                    Console.WriteLine($"Perft execution time: {stopwatch.Elapsed.TotalMilliseconds} ms");
                    break;
                case "d":
                    // display board
                    break;
                case "--help":
                case "help":
                    // Pristar4.ChessEngine help
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(
                        "  ___        _      _    ____        _   \n" +
                        " / _ \\ _   _(_) ___| | _| __ )  ___ | |_ \n" +
                        "| | | | | | | |/ __| |/ /  _ \\ / _ \\| __|\n" +
                        "| |_| | |_| | | (__|   <| |_) | (_) | |_ \n" +
                        " \\__\\_\\\\__,_|_|\\___|_|\\_\\____/ \\___/ \\__|");
                    Console.ResetColor();
                    SyncOut.WriteLine(" v" + VersionInfo.GetProjectVersion());
                    SyncOut.WriteLine("Commands:");
                    SyncOut.WriteLine("quit - quit program");
                    SyncOut.WriteLine("uci - show uci info");
                    SyncOut.WriteLine("setoption - set uci option");
                    SyncOut.WriteLine("go - start search");
                    SyncOut.WriteLine("position - set position");
                    SyncOut.WriteLine("ucinewgame - new game");
                    SyncOut.WriteLine("isready - readyok");
                    SyncOut.WriteLine("d - display board");
                    SyncOut.WriteLine("help - show this help");
                    break;
                default:
                {
                    if (!string.IsNullOrEmpty(token) && token[0] != '#')
                    {
                        SyncOut.WriteLine(
                            $"Unknown command: '{cmd}'. Type help for more information.");
                    }

                    break;
                }
            }
        } while
            (token != "quit" &&
             args.Length == 0); // The command-line arguments are one-shot
    }

    /// <summary>
    ///     Go() is called whenever the engine receives the "go" UCI command.
    ///     It sets the thinking time and other parameters from the input string.
    ///     and then starts the search.
    /// </summary>
    /// <param name="uci"></param>
    /// <param name="game"></param>
    /// <param name="input"></param>
    /// <param name="states"></param>
    public static void Go(this IUci uci, IGame game,
        string? input, Stack<State> states)
    {
        // Split the input string
        var tokensList = new List<string>();

        if (!string.IsNullOrEmpty(input))
        {
            tokensList = input.Split(' ').ToList();
        }


        for (int i = 0; i < tokensList.Count; i++)
        {
            string token = tokensList[i];

            // TODO: check if this is the correct way to parse the token value
            ulong tokenValue = ulong.TryParse(token, out tokenValue) ? tokenValue : 0;

            switch (token)
            {
                case "searchmoves":
                {
                    // Needs to be the last command on the line
                    i++; // Move past the "searchmoves" token

                    while (i < tokensList.Count)
                    {
                        // check if it is a legal move
                        var move = uci.MoveFromUci(game.Pos, tokensList[i]);
                        game.SearchParameters.SearchMoves.Add(move);
                        i++;
                    }

                    i--; // Move the index back to the starting token of the next command
                    break;
                }
                case "wtime":
                    game.SearchParameters.WhiteTimeMilliseconds = tokenValue;
                    break;
                case "btime":
                    game.SearchParameters.BlackTimeMilliseconds = tokenValue;
                    break;
                case "winc":
                    game.SearchParameters.WhiteIncrementTimeMilliseconds = tokenValue;
                    break;
                case "binc":
                    game.SearchParameters.BlackIncrementTimeMilliseconds = tokenValue;
                    break;
                case "movestogo":
                    game.SearchParameters.MovesToGo = tokenValue;
                    break;
                case "depth":
                    game.SearchParameters.Depth = tokenValue;
                    break;
                case "nodes":
                    game.SearchParameters.Nodes = tokenValue;
                    break;
                case "movetime":
                    game.SearchParameters.MoveTime = tokenValue;
                    break;
                case "mate":
                    game.SearchParameters.Mate = tokenValue;
                    break;
                case "perft":
                    //TODO: check perft command input root? depth?
                    game.Perft(6);
                    break;
                case "infinite":
                    game.SearchParameters.Infinite = true;
                    break;
                case "ponder":
                    game.SearchParameters.Ponder = true;
                    break;
            }
        }

        var search = new Search();

        var bestMove = search.Think(game);
        string actual = uci.MoveToUci(game.Pos, bestMove);
        uci.BestMove(bestMove, Move.EmptyMove);
        Console.WriteLine($"bestmove {actual}");
    }
}