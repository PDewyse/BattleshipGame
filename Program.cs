using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Transactions;
using ConsoleGame.Setup;

using ConsoleGame.Components;

namespace ConsoleGame
{
    // Class for executing the program
    class Program
    {
        static void Main(string[] args) 
        {
            BattleshipGame game = new BattleshipGame();
            game.Run();
        }
    }
    class BattleshipGame
    {
        static int SEA_SIZE = 10; // for the GameBoard class
        static int GAME_VERSION = 2002; // for the GameBoard class
        static int MAX_TURNS = 100;

        /// <summary>
        /// The main method that runs the game.
        /// </summary>
        public void Run()
        {
            MainMenu();
        }     
        /// <summary>
        /// Displays the splash screen for the game BattleShip.
        /// </summary>
        private void DisplaySplashScreen()
        {
            Console.WriteLine(@"                                      
 _           _   _   _           _     _       
| |         | | | | | |         | |   (_)      
| |__   __ _| |_| |_| | ___  ___| |__  _ _ __  
| '_ \ / _` | __| __| |/ _ \/ __| '_ \| | '_ \ 
| |_) | (_| | |_| |_| |  __/\__ \ | | | | |_) |   |__
|_.__/ \__,_|\__|\__|_|\___||___/_| |_|_| .__/    |\/
                                        | |       ---
                                        | |      / | [
                                          !      | |||
                                        _/|     _/|-++'
                                    +  +--|    |--|--|_ |-
                                { /|__|  |/\__|  |--- |||__/
                                +---------------___[}-_===_.'____                 /\
                            ____`-' ||___-{]_| _[}-  |     |_[___\==--            \/   _
            __..._____--==/___]_|__|_____________________________[___\==--____,------' .7
           \    BB-61                                                                  /
            \_________________________________________________________________________|");
            Console.WriteLine("\x1b[34m~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\x1b[0m");
            Console.WriteLine("\x1b[34m~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~Toekitoeki 2024~~~~~~~~\x1b[0m");
            
        }
        /// <summary>
        /// Gives the Main menu options and allows the user to choose. will bring the user to the correct part of the game.
        /// </summary>
        private void MainMenu()
        {
            DisplaySplashScreen();
            Console.WriteLine(@"
            1. START
            2. QUIT");

            string? input = Console.ReadLine();
            
            switch (input?.ToLower()) 
            {
                case "1":
                case "start":
                    StartGame();
                    break;
                case "2":
                case "quit":
                    QuitGame();
                    break;
                default:
                    Console.WriteLine("Please choose from the three options!");
                    MainMenu();
                    break;
            }
        }
        /// <summary>
        /// create the necessary instances and start the specific parts of the game.
        /// </summary>
        private void StartGame()
        {
            Console.WriteLine("Starting BattleShip!");

            GameBoard playerGameBoard = new GameBoard(SEA_SIZE);
            GameBoard enemyGameBoard = new GameBoard(SEA_SIZE);

            // Load the game components
            Dictionary<string, int> ships = GameBoard.MakeShipAssets(GAME_VERSION);
            Dictionary<int, string> seaMappings = GameBoard.MakeSeaMappings();
            Dictionary<int, char> coordinates = GameBoard.MakeGameBoard(SEA_SIZE);

            // Game process
            ShipPlacementMenu(playerGameBoard, enemyGameBoard, ships, seaMappings, coordinates);

            GameLoopMenu(playerGameBoard, enemyGameBoard, ships, seaMappings, coordinates);
            GameLoop(playerGameBoard, enemyGameBoard, ships, seaMappings, coordinates);
            
            QuitGame();
        }
        /// <summary>
        /// A simple method allowing the program to terminate where it is called.
        /// </summary>
        private void QuitGame()
        {
            Console.WriteLine("Thank you for playing BattleShip by Toekitoeki!");
            Environment.Exit(0);
        }
        /// <summary>
        /// Displays the Ship placements menu options and brings the user to the correct part of the game.
        /// </summary>
        private void ShipPlacementMenu(GameBoard playerGameBoard, GameBoard enemyGameBoard,
                                       Dictionary<string, int> ships, 
                                       Dictionary<int, string> seaMappings, 
                                       Dictionary<int, char> coordinates)
        {
            Console.WriteLine(@"How do you wish to place your ships?
            1. CUSTOM
            2. RANDOM 
            3. BACK");

            string? input = Console.ReadLine();

            switch (input?.ToLower())
            {
                case "1":
                case "1.":
                case "custom":
                    ShipPlacement(playerGameBoard, enemyGameBoard, ships, seaMappings, coordinates, "custom"); 
                    break;
                case "2":
                case "2.":
                case "random":
                    ShipPlacement(playerGameBoard, enemyGameBoard, ships, seaMappings, coordinates, "random"); 
                    break;
                case "3":
                case "3.":
                case "back":
                    MainMenu();
                    break;
                default:
                    Console.WriteLine("Please choose from the three options!");
                    ShipPlacementMenu(playerGameBoard, enemyGameBoard, ships, seaMappings, coordinates);
                    break;
            }
        }
        /// <summary>
        /// Main method for placing the ships on the game board for both the player and the enemy.
        /// </summary>
        /// <param name="playerGameBoard">An instance of the GameBoard class for the player</param>
        /// <param name="enemyGameBoard">An instance of the GameBoard class for the enemy</param>
        /// <param name="ships">A dictionary containing the ships that will be placed on the game board</param>
        /// <param name="seaMappings">A dictionary containing the mappings for the sea terrain</param>
        /// <param name="coordinates">A dictionary containing the mappings for the game board coordinates</param>
        /// <param name="placementType">A string specifying the type of placement, either random or custom</param>
        private void ShipPlacement(GameBoard playerGameBoard, GameBoard enemyGameBoard,
                                   Dictionary<string, int> ships, 
                                   Dictionary<int, string> seaMappings, 
                                   Dictionary<int, char> coordinates,
                                   string placementType="random")
        {
            // Reset the seas for both the player and the enemy (for re-doing the placement)
            enemyGameBoard.ResetSea();
            playerGameBoard.ResetSea();

            enemyGameBoard.SeaArray = enemyGameBoard.PlaceShips(enemyGameBoard.SeaArray,ships, seaMappings,coordinates,"random");
            playerGameBoard.SeaArray = playerGameBoard.PlaceShips(playerGameBoard.SeaArray, ships, seaMappings,coordinates, placementType);
            
            ShipConfirmMenu(playerGameBoard, enemyGameBoard, ships, seaMappings, coordinates);
        }
        /// <summary>
        /// Displays the Ship confirmation menu options and brings the user to the correct part of the game.
        /// </summary>
        /// <param name="playerGameBoard">An instance of the GameBoard class for the player</param>
        /// <param name="enemyGameBoard">An instance of the GameBoard class for the enemy</param>
        /// <param name="ships">A dictionary containing the ships that will be placed on the game board</param>
        /// <param name="seaMappings">A dictionary containing the mappings for the sea terrain</param>
        /// <param name="coordinates">A dictionary containing the mappings for the game board coordinates</param>
        /// <param name="view">A string specifying the type of view, either all, waves, tracking, or debug</param>
        private void ShipConfirmMenu(GameBoard playerGameBoard, GameBoard enemyGameBoard,
                                       Dictionary<string, int> ships, 
                                       Dictionary<int, string> seaMappings, 
                                       Dictionary<int, char> coordinates,
                                       string view = "all")
        {
            playerGameBoard.ViewSea(playerGameBoard.SeaArray, seaMappings, coordinates, "all");
            Console.WriteLine(@"Are you happy with this placement for your navy?
            1. YES
            2. NO");

            string? input = Console.ReadLine();

            switch (input?.ToLower())
            {
                case "1":
                case "1.":
                case "yes":
                    break;
                case "2":
                case "2.":
                case "no":
                    ShipPlacementMenu(playerGameBoard, enemyGameBoard, ships, seaMappings, coordinates);
                    break;
                default:
                    Console.WriteLine("Please choose from the two options!");
                    ShipConfirmMenu(playerGameBoard, enemyGameBoard, ships, seaMappings, coordinates, view);
                    break;
            }
        }
        /// <summary>
        /// Displays the Game loop menu options and brings the user to the correct part of the game.
        /// </summary>
        /// <param name="playerGameBoard">An instance of the GameBoard class for the player</param>
        /// <param name="enemyGameBoard">An instance of the GameBoard class for the enemy</param>
        /// <param name="ships">A dictionary containing the ships that will be placed on the game board</param>
        /// <param name="seaMappings">A dictionary containing the mappings for the sea terrain</param>
        /// <param name="coordinates">A dictionary containing the mappings for the game board coordinates</param>
        private void GameLoopMenu(GameBoard playerGameBoard, GameBoard enemyGameBoard,
                                  Dictionary<string, int> ships, 
                                  Dictionary<int, string> seaMappings, 
                                  Dictionary<int, char> coordinates)
        {
            Console.WriteLine(@"Congratulations for choosing your navy! Let the battle begin!
            1. START BATTLE
            2. REDO SHIP PLACEMENT
            3. QUIT");

            string? input = Console.ReadLine();

            switch (input?.ToLower())
            {
                case "1":
                case "1.":
                case "start":
                    break; // Continue under start game
                case "2":
                case "2.":
                case "redo":
                    ShipPlacementMenu(playerGameBoard, enemyGameBoard, ships, seaMappings, coordinates);
                    GameLoopMenu(playerGameBoard, enemyGameBoard, ships, seaMappings, coordinates);
                    break;
                case "3":
                case "3.":
                case "quit":
                    QuitGame();
                    break;
                default:
                    Console.WriteLine("Please choose from the four options!");
                    GameLoopMenu(playerGameBoard, enemyGameBoard, ships, seaMappings, coordinates);
                    break;
            }
        }
        /// <summary>
        /// The main game loop where the player and the enemy take turns to shoot at each other.
        /// </summary>
        /// <param name="playerGameBoard">An instance of the GameBoard class for the player</param>
        /// <param name="enemyGameBoard">An instance of the GameBoard class for the enemy</param>
        /// <param name="ships">A dictionary containing the ships that will be placed on the game board</param>
        /// <param name="seaMappings">A dictionary containing the mappings for the sea terrain</param>
        /// <param name="coordinates">A dictionary containing the mappings for the game board coordinates</param>
        private void GameLoop(GameBoard playerGameBoard, GameBoard enemyGameBoard,
                              Dictionary<string, int> ships, 
                              Dictionary<int, string> seaMappings, 
                              Dictionary<int, char> coordinates)
        {
            // initialize the necessary variables for the game loop
            Tuple<int, int> targetCoordinate;
            Random rand = new Random();
            int row;
            int col;
            List<int> rowIndices = new List<int>();
            List<int> colIndices = new List<int>();

            int playerHits = 0;
            int enemyHits = 0;
            int maxHits = GameBoard.ComputeMaxHits(ships);
            bool success;
            int speed = 1000; // in milliseconds

            for (int i = 0; i < MAX_TURNS; i++)
            {
                Console.WriteLine($"Starting turn {i+1}");
                
                // Player turn
                targetCoordinate = TargetSelectionMenu(playerGameBoard, enemyGameBoard, seaMappings, coordinates);
                PrintCountDown(speed);
                success = EvaluateTarget(enemyGameBoard, targetCoordinate);
                playerHits += success ? 1 : 0;

                // Enemy turn
                Console.WriteLine("The Enemy will now make its move");

                // no placing back the same coordinates
                do
                {
                    row = rand.Next(0, coordinates.Count);
                } while (rowIndices.Contains(row));
                rowIndices.Add(row);
                
                do
                {
                    col = rand.Next(0, coordinates.Count);
                } while (colIndices.Contains(col));
                colIndices.Add(col);

                targetCoordinate = new Tuple<int, int>(row, col);
                PrintCountDown(speed);
                success = EvaluateTarget(playerGameBoard, targetCoordinate);
                enemyHits += success ? 1 : 0;

                if (EndGame(playerHits, enemyHits, maxHits)) 
                {
                    break;
                }
            }
        }
        /// <summary>
        /// Displays the Target selection menu options and returns the target coordinate.
        /// </summary>
        /// <param name="playerGameBoard">An instance of the GameBoard class for the player</param>
        /// <param name="enemyGameBoard">An instance of the GameBoard class for the enemy</param>
        /// <param name="seaMappings">A dictionary containing the mappings for the sea terrain</param>
        /// <param name="coordinates">A dictionary containing the mappings for the game board coordinates</param>
        /// <returns>A tuple containing the row and column of the target coordinate</returns>
        private Tuple<int, int> TargetSelectionMenu(GameBoard playerGameBoard, GameBoard enemyGameBoard,
                              Dictionary<int, string> seaMappings, 
                              Dictionary<int, char> coordinates)
        {
            while (true)
            {
                Console.WriteLine("ENEMY SEA");
                enemyGameBoard.ViewSea(enemyGameBoard.SeaArray, seaMappings, coordinates, "tracking");
                Console.WriteLine();
                Console.WriteLine("YOUR SEA");
                playerGameBoard.ViewSea(playerGameBoard.SeaArray, seaMappings, coordinates, "all");
                
                Tuple<int, int> targetCoordinate;

                Console.WriteLine("Select the coordinates of the enemy sea you wish to target. (eg. c4)\nNote: You can exit the game by typing exit");
                string? input = Console.ReadLine();

                if (input?.Length == 2 && char.IsLetter(input[0]) && char.IsDigit(input[1]))
                {
                    int row = int.Parse(input[1].ToString())-1;
                    int col = char.ToUpper(input[0]) - 'A';

                    if (row > 0 && col > 0 && row < coordinates.Count() && col < coordinates.Count())
                    {
                        targetCoordinate = new Tuple<int, int>(row, col);
                        return targetCoordinate;
                    }
                    else
                    {
                        Console.WriteLine("Invalid target coordinates, try something in the format: letter+number, eg b5.");
                        continue;
                    }
                }
                else if (input == "exit")
                {
                    QuitGame();
                }
                else
                {
                    Console.WriteLine("Invalid target coordinates, try something in the format: letter+number, eg b5.");
                    continue;
                }
            }
        }
        /// <summary>
        /// Evaluates the target coordinate and updates the game board accordingly.
        /// </summary>
        /// <param name="board">An instance of the GameBoard class</param>
        /// <param name="targetCoordinate">A tuple containing the row and column of the target coordinate</param>
        /// <returns>A boolean indicating whether the target was a hit or a miss</returns>
        private bool EvaluateTarget(GameBoard board, Tuple<int, int> targetCoordinate)
        {
            (int row, int col) = targetCoordinate;
            int targetValue = board.SeaArray[row, col];
            Dictionary<int, string> shipMappings = GameBoard.MakeShipMappings();
            switch (targetValue)
            {
                case 0:
                    Console.WriteLine("Miss!\n");
                    board.SeaArray[row, col] = 2;
                    return false;
                case 1:
                    Console.WriteLine("Miss! A ship was already hit here.\n");
                    return false;
                case 2:
                    Console.WriteLine("Miss! This sector was already hit.\n");
                    return false;
                case >=3:
                    Console.WriteLine("\x1b[31mHit!\x1b[0m\n");
                    board.SeaArray[row, col] = 1;
                    // Every ship has it's own unique value
                    if (board.CountSeaValues(targetValue) == 0)
                    {
                        Console.WriteLine($"The {shipMappings[targetValue]} has been sunk!\n");
                    }
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Prints a countdown before the shot is fired.
        /// </summary>
        /// <param name="speed">An integer specifying the speed of the countdown</param>
        private void PrintCountDown(int speed=1000)
        {
            Dictionary<int, string> stringNumbers = new Dictionary<int, string>{{1,"One"}, {2, "Two"}, {3, "Three"}};

            Console.WriteLine($"Target acquired, firing in:");

            for (int i = stringNumbers.Count; i >= 1; i--)
            {
                Console.WriteLine($"{stringNumbers[i]}");
                Thread.Sleep(speed);
            }
            Console.WriteLine($"Shot has been fired!");
            Thread.Sleep(speed);
        }
        /// <summary>
        /// Checks if the game has ended and displays the game state.
        /// </summary>
        /// <param name="playerHitCount">An integer specifying the number of hits the player has scored</param>
        /// <param name="enemyHitCount">An integer specifying the number of hits the enemy has scored</param>
        /// <param name="maxHits">An integer specifying the maximum number of hits required to win the game</param>
        /// <returns>A boolean indicating whether the game has ended</returns>
        private bool EndGame(int playerHitCount, int enemyHitCount, int maxHits)
        {
            if (playerHitCount >= maxHits)
            {
                Console.WriteLine("You have won!");
                return true;
            }
            else if (enemyHitCount >= maxHits)
            {
                Console.WriteLine("You have lost, better luck next time.");
                return true;
            }
            else
            {
                Console.WriteLine($@"Current state of the game:
                >You have scored {playerHitCount} hits.
                >The enemy has hit you {enemyHitCount} times!");
                Console.WriteLine("");
                return false;
            }
        }
    }
}