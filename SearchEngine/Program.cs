﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SearchEngine
{
    public static class Program
    {
        private static int _filesCount = 0;
        private static SearchEngine _SearchEngine;
        private static readonly string _prompt = "> ";
        private static bool debug = false;
        private static int memoryLimit = 0;
        private static bool normalize = true;
        private static bool orderFixed = false;
        private static int numberOfPermutation = 2;
        private static string pattern = "[^a-zA-Z0-9 -]";
        private static string initialSource = "";
        private static string initialExtension = "*.*";

        static int Main(string[] args)
        {
            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "--help" || args[i] == "-h")
                    {
                        Console.WriteLine($"--debug -d Show additional information");
                        Console.WriteLine($"--help -h Show help");
                        Console.WriteLine($"--memory-limit -m Max memory size. 0 for disable");
                        Console.WriteLine($"--normalize -n Pre-process every word before insert");
                        Console.WriteLine($"--pattern -p Pattern for removing unwanted characters, used for each word before insert");
                        Console.WriteLine($"--source -s Load data from specific path at start");
                        Console.WriteLine($"--extension -e Set extension for loading data at start");
                    }

                    if (args[i] == "--debug" || args[i] == "-d")
                    {
                        if (args[i + 1].IndexOf("-") != 0)
                        {
                            debug = args[i + 1] == "true";
                        }
                    }

                    if (args[i] == "--memory-limit" || args[i] == "-m")
                    {
                        if (args[i + 1].IndexOf("-") != 0)
                        {
                            memoryLimit = Convert.ToInt32(args[i + 1]);
                        }
                    }

                    if (args[i] == "--normalize" || args[i] == "-n")
                    {
                        if (args[i + 1].IndexOf("-") != 0)
                        {
                            normalize = args[i + 1] == "true";
                        }
                    }

                    if (args[i] == "--orderFixed" || args[i] == "-of")
                    {
                        if (args[i + 1].IndexOf("-") != 0)
                        {
                            orderFixed = args[i + 1] == "true";
                        }
                    }

                    if (args[i] == "--numberOfPermutation" || args[i] == "-nop")
                    {
                        if (args[i + 1].IndexOf("-") != 0)
                        {
                            numberOfPermutation = int.TryParse(args[i + 1], out var nop) ? nop : 2;
                        }
                    }

                    if (args[i] == "--pattern" || args[i] == "-p")
                    {
                        if (args[i + 1].IndexOf("-") != 0)
                        {
                            pattern = args[i + 1];
                        }
                    }

                    if (args[i] == "--source" || args[i] == "-s")
                    {
                        if (args[i + 1].IndexOf("-") != 0)
                        {
                            initialSource = args[i + 1];
                        }
                    }

                    if (args[i] == "--extension" || args[i] == "-e")
                    {
                        if (args[i + 1].IndexOf("-") != 0)
                        {
                            initialExtension = args[i + 1];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wrong arguments {Environment.NewLine} {ex.ToString()}");
                return -2;
            }

            // sample data http://mlg.ucd.ie/datasets/bbc.html
            _SearchEngine = new SearchEngine(debug, normalize, orderFixed, numberOfPermutation, pattern, memoryLimit);

            if (initialSource != "")
            {
                LoadFromSource(initialSource, initialExtension);
            }

            while (true)
            {
                Console.Write(_prompt);
                string userInput = Console.ReadLine();

                ParseInput(userInput);
            }
        }

        static void ParseInput(string userInput)
        {
            string[] command = userInput.Split(' ');

            switch (command[0])
            {
                case "source":
                case "load":
                    if (command.Length == 3) LoadFromSource(command[1], command[2]);
                    break;
                case "get":
                case "search":
                    if (command.Length == 2) Search(command[1]);
                    break;
                case "query":
                    if (command.Length == 2) Search(command[1]);
                    if (command.Length == 3 && command[2] == "?") QueryShallow(command[1]);
                    if (command.Length == 3 && command[2] == "*") QueryDeep(command[1]);
                    break;
                case "add":
                case "insert":
                    if (command.Length == 3) Insert(command[1], command[2]);
                    break;
                case "delete":
                    if (command.Length == 2) Delete(command[1]);
                    break;
                case "echo":
                    if (command.Length == 2) Echo(command[1]);
                    break;
                case "flush":
                    Flush();
                    break;
                case "info":
                    ShowInfo();
                    break;
                case "clear":
                    Console.Clear();
                    break;
                case "debug":
                    if (command.Length == 2) SetDebug(command[1] == "true");
                    break;
                case "orderfixed":
                    if (command.Length == 2) SetOrderFixed(command[1] == "true");
                    break;
                case "numberofpermutation":
                case "nop":
                    if (command.Length == 2) SetNumberOfPermutation(int.TryParse(command[1], out var nop) ? nop : 2);
                    break;
                default:
                    break;
            }
        }

        private static void LoadFromSource(string path, string extension)
        {
            foreach (string file in Directory.EnumerateFiles(path, extension, SearchOption.AllDirectories))
            {
                _filesCount++;
                foreach (string line in File.ReadLines(file))
                {
                    _SearchEngine.InsertResource(file, line);
                }
            }

            if (_SearchEngine.Debug)
            {
                Console.WriteLine($"Words inserted {_SearchEngine.Count} from {_filesCount} files with memory usage of {GC.GetTotalMemory(false)} bytes");
            }
        }

        private static void Search(string key)
        {
            List<string> results = _SearchEngine.Get(key);

            if (_SearchEngine.Debug)
            {
                Console.WriteLine($"{results.Count} results for {key}");
            }

            foreach (string el in results)
            {
                Console.WriteLine(el);
            }
        }

        private static void QueryDeep(string key)
        {
            IEnumerable<string> results = _SearchEngine.QueryDeep(key);

            if (_SearchEngine.Debug)
            {
                Console.WriteLine($"{results.Count()} results for {key}");
            }

            foreach (string el in results)
            {
                Console.WriteLine(el);
            }
        }

        private static void QueryShallow(string key)
        {
            IEnumerable<string> results = _SearchEngine.QueryShallow(key);

            if (_SearchEngine.Debug)
            {
                Console.WriteLine($"{results.Count()} results for {key}");
            }

            foreach (string el in results)
            {
                Console.WriteLine(el);
            }
        }

        private static void Insert(string resourceName, string content)
        {
            _SearchEngine.InsertResource(resourceName, content);
        }

        private static void Delete(string key)
        {
            _SearchEngine.Remove(key);
        }

        private static void Echo(string content)
        {
            Console.WriteLine(_SearchEngine.Echo(content));
        }

        private static void Flush()
        {
            _SearchEngine.Flush();
            _filesCount = 0;
        }

        private static void SetDebug(bool newStatus)
        {
            _SearchEngine.Debug = newStatus;
        }

        private static void SetOrderFixed(bool orderFixed)
        {
            _SearchEngine.OrderFixed = orderFixed;
        }

        private static void SetNumberOfPermutation(int numberOfPermutation)
        {
            _SearchEngine.NumberOfPermutation = numberOfPermutation;
        }

        private static void ShowInfo()
        {
            Console.WriteLine($"Nodes in trie: {_SearchEngine.TrieSize}");
            Console.WriteLine($"Words inserted: {_SearchEngine.Count}");
            Console.WriteLine($"Resource files: {_filesCount}");
            Console.WriteLine($"Memory usage: {GC.GetTotalMemory(false)} bytes");
        }
    }
}
