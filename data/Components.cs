using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ConsoleGame.Components
{
    class GameBoard
    {
        private int[,] sea;

        public GameBoard(int size)
        {
            sea = new int[size, size];
        }

        public int[,] SeaArray
        {
            get { return sea; }
            set { sea = value; }
        }
        /// <summary>
        /// Loads the Game Board (sea) with default value of 10x10, user can specify.
        /// </summary>
        /// <param name="size">Integer, allows for tuning the sea size between 10x10 and 26x26</param>
        /// <returns>A dictionary containing the integers on keys and letters as values</returns>
        /// <exception cref="ArgumentException">give a correct size (10<size<27).</exception>
        public static Dictionary<int,char> MakeGameBoard(int size=10)
        {
            if (!(size >= 10 && size <= 26))
            {
                throw new ArgumentException($"Invalid parameter as size: {size}. Please choose an integer between 10 and 26.");
            }
            Dictionary<int, char> coordinates = new Dictionary<int, char>();
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (int i = 0; i < size; i++)
            {
                coordinates[i] = alphabet[i];
            }
            return coordinates;
        }
        /// <summary>
        /// Goes over the sea and resets all the values to 0 (waves)
        /// </summary>
        public void ResetSea()
        {
            for (int i = 0; i < sea.GetLength(0); i++)
            {
                for (int j = 0; j < sea.GetLength(1); j++)
                {
                    sea[i, j] = 0; // Assuming 0 represents an empty space
                }
            }
        }
        /// <summary>
        /// Visualizes the game board (sea) depending on the user settings.
        /// </summary>
        /// <param name="sea">2D Integer array. The game board (sea) that is played on.</param>
        /// <param name="mappings">Dictionary for the sea mappings keys for integers and the values as the mapped sea terrain</param>
        /// <param name="coordinates">Dictionary with key as integers and string values containing the alphabet letters</param>
        /// <param name="view">String. What you want to visualize in the sea. Possible arguments: 
        /// waves, tracking (waves and miss), all, debug (just the integer values).</param>
        /// <exception cref="ArgumentException">Give the correct view (all, waves, tracking, or debug)</exception>
        public void ViewSea(int[,] sea, Dictionary<int, string> mappings, 
                                         Dictionary<int, char> coordinates, 
                                         string view = "all")
        {
            // Check if the view parameter is valid
            if (!(view == "all" || view == "waves" || view == "tracking" || view == "debug"))
            {
                throw new ArgumentException($"Invalid parameter as view: {view}. Please choose from: waves, tracking, all, debug.");
            }
            
            // First print the letter coordinates for the sea
            Console.Write("    ");
            for (int i = 0; i < sea.GetLength(0); i++)
            {
                Console.Write($"[{coordinates[i]}]");
            }
            Console.WriteLine();

            // Output the Sea to the command line
            for (int i = 0; i < sea.GetLength(0); i++)
            {
                Console.Write($"[{i+1,2}]");
                for (int j = 0; j < sea.GetLength(1); j++)
                {
                    if      (view == "all"){Console.Write(mappings[sea[i,j]]);}         

                    else if (view == "waves"){Console.Write(mappings[0]);}

                    else if (view == "debug"){Console.Write($"[{sea[i,j]}]");}

                    else if (view == "tracking")
                    {
                        if      (sea[i,j] == 0){Console.Write(mappings[0]);}

                        else if (sea[i,j] == 1){Console.Write(mappings[1]);}

                        else if (sea[i,j] == 2){Console.Write(mappings[2]);}

                        else {Console.Write(mappings[0]);}
                    }
                }
                Console.Write('\n');
            }
        }
        /// <summary>
        /// Will randomly place ships or have the player prepare his/her/its own sea battle formation.
        /// Rules:  
        /// 1. All ships must be placed
        /// 2. Ships must fit inside the sea.
        /// 3. Ships may not be place alongside each other and the sea border (collision).
        /// </summary>
        /// <param name="sea">2D integer Array. A sea to make ready for battle.</param>
        /// <param name="shipModels">Dictionary with ship models (string) and their size (integer) 
        /// The ship models that will be used during the game.</param>
        /// <param name="placementType">The type of placement (string), random (default) or custom.</param>
        /// <returns>2D integer Array. A sea where all ship have been placed.</returns>
        /// <exception cref="ArgumentException">Give the correct placementType (random or custom)</exception>
        public int[,] PlaceShips(int[,] sea, Dictionary<string, int> shipModels, 
                                                Dictionary<int, string> mappings, 
                                                Dictionary<int, char> coordinates,
                                                string placementType="random")
        {
            if (!(placementType == "random" || placementType == "custom"))
            {
                throw new ArgumentException($"Invalid parameter as placementType: {placementType}. Please choose from random and custom.");
            }
            
            string directions = "<>^v";
            bool success;

            if (placementType == "random")
            {
                Random rand = new Random();
                foreach (KeyValuePair<string, int> ship in shipModels)
                {
                    int[,] seaUpdate = new int[sea.GetLength(0), sea.GetLength(1)];
                    Array.Copy(sea, seaUpdate, sea.Length);
                    
                    while (true)
                    {
                        // Generate ship positions and direction
                        int row = rand.Next(0, coordinates.Count);
                        int col = rand.Next(0, coordinates.Count);
                        Tuple<int, int> position = Tuple.Create(row, col);
                        char direction = directions[rand.Next(0, 4)];
                        
                        // This will check if the ship positions are valid
                        // TODO: here you can implement ship designs such as [-][-][>] instead of [O][O][O]
                        (seaUpdate, success) = CheckAndPlaceShips(seaUpdate, position, direction, ship.Value, ship.Key);
                    
                        if (success)
                        {
                            Array.Copy(seaUpdate, sea, sea.Length); // put the ship on sea
                            break;
                        }
                        else
                        {
                            Array.Copy(sea, seaUpdate, sea.Length); // reset to sea before this update.
                        }
                    }
                }
            }
            else if (placementType == "custom")
            {
                foreach (KeyValuePair<string, int> ship in shipModels)
                {
                    int[,] seaUpdate = new int[sea.GetLength(0), sea.GetLength(1)];
                    Array.Copy(sea, seaUpdate, sea.Length);
                    
                    success = false;
                    
                    while (!success)
                    {
                        // Prompt user for ship position and direction
                        ViewSea(seaUpdate, mappings, coordinates, "all");
                        Console.WriteLine($"Please give the position the direction of your \x1b[32m{ship.Key}\x1b[0m starting from the stern.");
                        for (int i = 0; i < ship.Value; i++) {Console.Write("\x1b[32m[O]\x1b[0m");}
                        Console.WriteLine($"\nNote: you cannot place ships next to each other and on the edges. Length of {ship.Value})!");
                        Console.WriteLine("TODO: If you wish to exit the custom placement, type 'exit'.");

                        string? input = Console.ReadLine();

                        if (input?.Length == 3 && char.IsLetter(input[0]) && char.IsDigit(input[1]) &&
                            (input[2] == '^' || input[2] == 'v' || input[2] == '<' || input[2] == '>'))
                        {
                            int row = int.Parse(input[1].ToString())-1;
                            int col = char.ToUpper(input[0]) - 'A';
                            Tuple<int, int> position = Tuple.Create(row, col);
                            char direction = input[2];
                            
                            // Check if ship can be placed at the specified position and direction
                            (seaUpdate, success) = CheckAndPlaceShips(seaUpdate, position, direction, ship.Value, ship.Key);
                            
                            if (success)
                            {
                                Console.WriteLine($"Successfully placed your \x1b[32m{ship.Key}\x1b[0m at {position}!");
                                Array.Copy(seaUpdate, sea, sea.Length); // Update sea with placed ship
                                break;
                            }
                            else
                            {
                                Console.WriteLine($"Invalid placement of your \x1b[32m{ship.Key}\x1b[0m. {position} with direction {direction} is not possible.");
                                Array.Copy(sea, seaUpdate, sea.Length); // reset to sea before this update.
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid input. Please enter the position and direction in the correct format.");
                        }
                    }
                }
            }
            return sea;
        }
        /// <summary>
        /// Checks if the ship can be placed at the specified position and direction.
        /// </summary>
        /// <param name="sea">2D integer Array. The sea where the ship will be placed.</param>
        /// <param name="position">Tuple with the row and column of the ship's head.</param>
        /// <param name="direction">Char. The direction the ship will be placed in.</param>
        /// <param name="shipLength">Integer. The length of the ship that will be placed.</param>
        /// <param name="shipName">String. The name of the ship that will be placed.</param>
        /// <returns>A tuple containing the updated sea and a boolean indicating whether the ship was placed successfully.</returns>
        /// <exception cref="ArgumentException">Give the correct direction (<>^v)</exception>
        private (int[,], bool) CheckAndPlaceShips(int[,] sea, Tuple<int, int> position, char direction, int shipLength, string shipName)
        {
            if (!(direction == '^' || direction == 'v' || direction == '<' || direction == '>'))
            {
                throw new ArgumentException($"Invalid parameter as direction: {direction}. Please choose from:<>^v");
            }

            // logic on how to handle row and col if the coordinates start from the stern of the ship
            // The offset basically removes the need to reverse the for loop increment if the ship goes ^ or <
            int rowOffset = 0, colOffset = 0;
            (int row, int col) = position;
            switch (direction)
            {
                case '^': 
                    if (row-shipLength-1 < 0 || row+1 > sea.GetLength(0)-1 || col-1 < 0 || col+1 > sea.GetLength(1)-1)
                    {
                        return (sea, false);
                    }
                    rowOffset = -1; 
                    break;
                case 'v': 
                    if (row-1 < 0 || row+shipLength+1 > sea.GetLength(0)-1 || col-1 < 0 || col+1 > sea.GetLength(1)-1)
                    {
                        return (sea, false);
                    }
                    rowOffset = 1; 
                    break;
                case '<': 
                    if (row-1 < 0 || row+1 > sea.GetLength(0)-1 || col-shipLength-1 < 0 || col+1 > sea.GetLength(1)-1)
                    {
                        return (sea, false);
                    }
                    colOffset = -1; 
                    break;
                case '>': 
                    if (row-1 < 0 || row+1 > sea.GetLength(0)-1 || col-1 < 0 || col+shipLength+1 > sea.GetLength(1)-1)
                    {
                        return (sea, false);
                    }
                    colOffset = 1;
                    break;
            }
            // Go over the ship positions
            for (int i = 0; i < shipLength; i++)
            {
                int newRow = row + rowOffset * i;
                int newCol = col + colOffset * i;

                // And check if the current cell is occupied or adjacent to an occupied cell (collsion)
                for (int r = newRow - 1; r <= newRow + 1; r++)
                {
                    for (int c = newCol - 1; c <= newCol + 1; c++)
                    {
                        if (sea[r, c] != 0)
                        {
                            return (sea, false);
                        }
                    }
                }
            }
            // Placement
            for (int i = 0; i < shipLength; i++)
            {
                int newRow = row + rowOffset * i;
                int newCol = col + colOffset * i;
                
                // different ID per ship so you can keep track when they are sunk
                switch(shipName)
                {
                    case "Carrier":
                        sea[newRow, newCol] = 3;
                        break;
                    case "Battleship":
                        sea[newRow, newCol] = 4;
                        break;
                    case "Destroyer":
                        sea[newRow, newCol] = 5;
                        break;
                    case "Submarine":
                        sea[newRow, newCol] = 6;
                        break;
                    case "Patrol Boat":
                        sea[newRow, newCol] = 7;
                        break;
                    case "Cruiser":
                        sea[newRow, newCol] = 8;
                        break;
                }
            }
            return (sea, true);
        }
        /// <summary>
        /// Versatile method to count the number of times a specific value is present in the sea. (for keeping track of ship IDs)
        /// </summary>
        /// <param name="value">Integer, the value you want to count in the sea</param>
        /// <returns>Integer, the number of times the value is present in the sea</returns>
        public int CountSeaValues(int value)
        {
            int count = 0;
            for (int i = 0; i < sea.GetLength(0); i++)
            {
                for (int j = 0; j < sea.GetLength(1); j++)
                {
                    if (sea[i,j] == value)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        /// <summary>
        /// Loads the ships that you can use during the game in a dictionary. 
        /// You can play with both the 1990 and the 2002 version.
        /// </summary>
        /// <param name="gameVersion">Integer, specifies the version of Battleship. 2 options:
        /// the 1990 Milton Bradley version or the 2002 Hasbro version</param>
        /// <param name="customShips">TODO: here you should be able to edit and add ships as you wish.</param>
        /// <returns>A dictionary containing all the ships for your navy during the game. 
        /// keys the ship names (string) and values their size (integer)</returns>
        /// <exception cref="ArgumentException">Give a correct game version (1990 or 2002)</exception>
        public static Dictionary<string, int> MakeShipAssets(int gameVersion=2002, bool customShips=false)
        {
            Dictionary<string, int> ships = new Dictionary<string, int>();
            switch (gameVersion)
            {
                case 1990:
                    ships = new Dictionary<string, int>
                    {
                        {"Carrier", 5},
                        {"Battleship", 4},
                        {"Cruiser", 3},
                        {"Submarine", 3},
                        {"Destroyer", 2}
                    };
                    break;
                case 2002:
                    ships = new Dictionary<string, int>
                    {
                        {"Carrier", 5},
                        {"Battleship", 4},
                        {"Destroyer", 3},
                        {"Submarine", 3},
                        {"Patrol Boat", 2}
                    };
                    break;
                default:
                    throw new ArgumentException("Invalid game version.");
            }
            if (customShips)
            {
                Console.WriteLine("TODO: way to add and change ships");
            }
            return ships;
        }
        /// <summary>
        /// Loads the mappings that will be used to generate the sea during the game.
        /// </summary>
        /// <param name="customSeaMappings">TODO: here you should be able to edit and add sea/terrain as you wish</param>
        /// <returns>A dictionary containing the mappings from integers (keys) to the terrain type strings (values)</returns>
        public static Dictionary<int, string> MakeSeaMappings(bool customSeaMappings=false)
        {
            // different values per ship so you can keep track when they are sunk
            Dictionary<int, string> seaMappings = new Dictionary<int,string>
            {
                {0, "\x1b[34m[~]\x1b[0m"}, // Waves/unknown (blue)
                {1, "\x1b[31m[x]\x1b[0m"}, // Destroyed/hit (red)
                {2, "   "},                // Miss
                {3, "\x1b[32m[O]\x1b[0m"}, // Carrier intact (green)
                {4, "\x1b[32m[O]\x1b[0m"}, // Battleship intact (green)
                {5, "\x1b[32m[O]\x1b[0m"}, // Destroyer intact (green)
                {6, "\x1b[32m[O]\x1b[0m"}, // Submarine intact (green)
                {7, "\x1b[32m[O]\x1b[0m"}, // Patrol Boat intact (green)
                {8, "\x1b[32m[O]\x1b[0m"}  // cruiser intact (green)
            };

            if (customSeaMappings)
            {
                Console.WriteLine("TODO: way to add and change the sea/terrain and how it behaves");
            }
            return seaMappings;
        }
        /// <summary>
        /// a method to link the ship ID to the ship name
        /// </summary>
        /// <param name="customShipMappings">TODO: here you should be able to edit and add ships as you wish</param>
        /// <returns>A dictionary containing the mappings from integers (keys) to the ship names (values)</returns>
        public static Dictionary<int, string> MakeShipMappings(bool customShipMappings=false)
        {
            // different values per ship so you can keep track when they are sunk
            Dictionary<int, string> shipMappings = new Dictionary<int,string>
            {
                {3, "Carrier"},
                {4, "Battleship"},
                {5, "Destroyer"},
                {6, "Submarine"},
                {7, "Patrol Boat"},
                {8, "Cruiser"}
            };

            if (customShipMappings)
            {
                Console.WriteLine("TODO: way to add and change the sea/terrain and how it behaves");
            }
            return shipMappings;
        }
        /// <summary>
        /// Computes the maximum number of hits required to win the game. (the sum of all ship sizes)
        /// </summary>
        /// <param name="ships">A dictionary containing the ships that will be placed on the game board</param>
        /// <returns>An integer specifying the maximum number of hits required to win the game</returns>
        public static int ComputeMaxHits(Dictionary<string, int> ships)
        {
            int maxHits = 0;
            foreach (KeyValuePair<string, int> ship in ships)
            {
                maxHits += ship.Value;
            }
            return maxHits;
        }
    }
}