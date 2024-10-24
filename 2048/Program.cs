using System;
using System.IO;
using System.Linq;
using System.Text.Json;

class Game2048
{
    static int[,] board = new int[4, 4];
    static Random random = new Random();
    static int score = 0;

    static void Main()
    {
        LoadGame();
        Play();
    }

    static void Play()
    {
        AddRandomTile();
        AddRandomTile();
        while (true)
        {
            Console.Clear();
            PrintBoard();
            Console.WriteLine($"Score: {score}");
            Console.WriteLine("Press Arrow Keys to move, S to save, L to load or Esc to exit.");

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            if (keyInfo.Key == ConsoleKey.Escape) break;
            if (keyInfo.Key == ConsoleKey.S) { SaveGame(); continue; }
            if (keyInfo.Key == ConsoleKey.L) { LoadGame(); continue; }
            int moveMade = 0;

            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow: moveMade = MoveUp(); break;
                case ConsoleKey.DownArrow: moveMade = MoveDown(); break;
                case ConsoleKey.LeftArrow: moveMade = MoveLeft(); break;
                case ConsoleKey.RightArrow: moveMade = MoveRight(); break;
            }

            if (moveMade > 0)
            {
                AddRandomTile();
                if (IsGameOver())
                {
                    Console.Clear();
                    PrintBoard();
                    Console.WriteLine($"Game Over! Final Score: {score}");
                    break;
                }
            }
        }
    }

    static void PrintBoard()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
                Console.Write(board[i, j] == 0 ? ".\t" : board[i, j] + "\t");
            Console.WriteLine();
        }
    }

    static void AddRandomTile()
    {
        int emptyCount = 0;
        foreach (var tile in board)
            if (tile == 0) emptyCount++;

        if (emptyCount == 0) return;

        int index = random.Next(0, emptyCount);
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (board[i, j] == 0)
                {
                    if (index == 0)
                    {
                        board[i, j] = random.Next(0, 100) < 90 ? 2 : 4;
                        return;
                    }
                    index--;
                }
            }
        }
    }

    static int MoveUp()
    {
        int moveMade = 0;
        for (int col = 0; col < 4; col++)
        {
            int[] tiles = new int[4];
            for (int row = 0; row < 4; row++)
                tiles[row] = board[row, col];

            int[] newTiles = MergeTiles(tiles);
            moveMade += newTiles.Where((t, i) => t != tiles[i]).Count();

            for (int row = 0; row < 4; row++)
                board[row, col] = newTiles[row];
        }
        return moveMade;
    }

    static int MoveDown()
    {
        int moveMade = 0;
        for (int col = 0; col < 4; col++)
        {
            int[] tiles = new int[4];
            for (int row = 0; row < 4; row++)
                tiles[row] = board[3 - row, col];

            int[] newTiles = MergeTiles(tiles);
            moveMade += newTiles.Where((t, i) => t != tiles[i]).Count();

            for (int row = 0; row < 4; row++)
                board[3 - row, col] = newTiles[row];
        }
        return moveMade;
    }

    static int MoveLeft()
    {
        int moveMade = 0;
        for (int row = 0; row < 4; row++)
        {
            int[] tiles = new int[4];
            for (int col = 0; col < 4; col++)
                tiles[col] = board[row, col];

            int[] newTiles = MergeTiles(tiles);

            moveMade += newTiles.Where((t, i) => t != tiles[i]).Count();

            for (int col = 0; col < 4; col++)
                board[row, col] = newTiles[col];
        }
        return moveMade;
    }

    static int MoveRight()
    {
        int moveMade = 0;
        for (int row = 0; row < 4; row++)
        {
            int[] tiles = new int[4];
            for (int col = 0; col < 4; col++)
                tiles[col] = board[row, 3 - col];

            int[] newTiles = MergeTiles(tiles);
            moveMade += newTiles.Where((t, i) => t != tiles[i]).Count();

            for (int col = 0; col < 4; col++)
                board[row, 3 - col] = newTiles[col];
        }
        return moveMade;
    }

    static int[] MergeTiles(int[] tiles)
    {
        int[] newTiles = new int[4];
        int position = 0;

        for (int i = 0; i < 4; i++)
        {
            if (tiles[i] != 0)
            {
                if (position > 0 && newTiles[position - 1] == tiles[i])
                {
                    newTiles[position - 1] *= 2;
                    score += newTiles[position - 1];
                    position--;
                }
                else
                {
                    newTiles[position] = tiles[i];
                    position++;
                }
            }
        }

        return newTiles;
    }

    static bool IsGameOver()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (board[i, j] == 0) return false;
                if (j < 3 && board[i, j] == board[i, j + 1]) return false;
                if (i < 3 && board[i, j] == board[i + 1, j]) return false;
            }
        }
        return true;
    }

    static void SaveGame()
    {
        var saveData = new { Board = board, Score = score };
        string jsonString = JsonSerializer.Serialize(saveData);
        File.WriteAllText("savegame.json", jsonString);
        Console.WriteLine("Game saved!");
        Console.ReadKey();
    }

    static void LoadGame()
    {
        if (!File.Exists("savegame.json")) return;

        string jsonString = File.ReadAllText("savegame.json");
        var saveData = JsonSerializer.Deserialize<SaveData>(jsonString);
        board = saveData.Board;
        score = saveData.Score;
        Console.WriteLine("Game loaded!");
        Console.ReadKey();
    }

    public class SaveData
    {
        public int[,] Board { get; set; }
        public int Score { get; set; }
    }
}
