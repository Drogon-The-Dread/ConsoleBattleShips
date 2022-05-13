using BattleShipConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace BattleShipConsole
{
    class Program //Created for Everflow with Github Copilot for speed
    {
        static void Main(string[] args)
        {
            Run();
        }

        static void Run()
        {
            #region Intro Text
            WriteLine("Welcome to Battleship!");
            WriteLine("The blue colour [~] represents water thats unexplored\n" + "The red colour[*] represents a ship has been hit \n"
            + "The green colour[O] represents your ships.\n");
            WriteLine("Left hand side is your firing Map, Right hand side is your Boats and enemies shots.");
            #endregion Intro Text

            #region Name Input
            WriteLine("Please enter your name: ");
            string name = ReadLine();
            if (name.Length == 0 || name == " ")
            {
                WriteLine("Nothing Entered, Press Enter to restart");
                ReadLine();
                Clear();
                Run();
            }
            else if (name.Length > 1)
                name = name.Substring(0, 1).ToUpper() + name.Substring(1);
            WriteLine($"Welcome {name}.");
            #endregion Name Input

            #region Board Creation
            bool ShowShips = false;//show enemy ships position
            if (name == "Iamacheater")
            {
                ShowShips = true;
            }
            Dictionary<char, int> Coordinates = PopulateDictionary();
            PrintHeader();
            for (int h = 0; h < 19; h++)
            {
                Write(" ");
            }
            #endregion Board Creation

            #region Game
            EVFNavyAsset EVFNavyAsset = new EVFNavyAsset();
            EVFNavyAsset EVFEnemyNavyAsset = new EVFNavyAsset();

            PrintMap(EVFNavyAsset.FirePositions, EVFNavyAsset, EVFEnemyNavyAsset, ShowShips);

            int Game;
            for (Game = 1; Game < 101; Game++)
            {
                EVFNavyAsset.StepsTaken++;

                Position position = new Position();

                ForegroundColor = ConsoleColor.White;
                WriteLine("Enter firing Coord's (e.g. A3).");
                string input = ReadLine();
                position = AnalyzeInput(input, Coordinates);

                if (position.x == -1 || position.y == -1)
                {
                    WriteLine("Invalid coordinates!");
                    Game--;
                    continue;
                }

                if (EVFNavyAsset.FirePositions.Any(EFP => EFP.x == position.x && EFP.y == position.y))
                {
                    WriteLine("This coordinate already being shot.");
                    Game--;
                    continue;
                }


                EVFEnemyNavyAsset.Fire();


                var index = EVFNavyAsset.FirePositions.FindIndex(p => p.x == position.x && p.y == position.y);

                if (index == -1)
                    EVFNavyAsset.FirePositions.Add(position);

                Clear();



                EVFNavyAsset.AllShipsPosition.OrderBy(o => o.x).ThenBy(n => n.y).ToList();
                EVFNavyAsset.CheckShipStatus(EVFEnemyNavyAsset.FirePositions);

                EVFEnemyNavyAsset.AllShipsPosition.OrderBy(o => o.x).ThenBy(n => n.y).ToList();
                EVFEnemyNavyAsset.CheckShipStatus(EVFNavyAsset.FirePositions);

                PrintHeader();
                for (int h = 0; h < 19; h++)
                {
                    Write(" ");
                }



                PrintMap(EVFNavyAsset.FirePositions, EVFNavyAsset, EVFEnemyNavyAsset, ShowShips);

                Commentator(EVFNavyAsset, true);
                Commentator(EVFEnemyNavyAsset, false);
                if (EVFEnemyNavyAsset.IsObliteratedAll || EVFNavyAsset.IsObliteratedAll) { break; }


            }

            ForegroundColor = ConsoleColor.White;

            if (EVFEnemyNavyAsset.IsObliteratedAll && !EVFNavyAsset.IsObliteratedAll)
            {
                WriteLine($"Game Ended, you win. {name}");
            }
            else if (!EVFEnemyNavyAsset.IsObliteratedAll && EVFNavyAsset.IsObliteratedAll)
            {
                WriteLine($"Game Ended, you lose. {name}, The only winning move is not to play."); //Cheeky War games reference
            }
            else
            {
                WriteLine("Game Ended, draw.");
            }

            WriteLine("Total steps taken:{0} ", Game);
            ReadLine();


        }

        static void PrintStatistic(int x, int y, EVFNavyAsset navyAsset)
        {
            if (x == 1 && y == 10)
            {
                ForegroundColor = ConsoleColor.White;
                Write("Indicator:    ");
            }

            if (x == 2 && y == 10)
            {
                if (navyAsset.IsBattleshipSunk)
                {
                    ForegroundColor = ConsoleColor.Red;
                    Write("Battleship [5]");
                }
                else
                {
                    ForegroundColor = ConsoleColor.DarkGreen;
                    Write("Battleship [5]");
                }
            }

            if (x == 3 && y == 10)
            {

                if (navyAsset.IsDestroyerSunk)
                {
                    ForegroundColor = ConsoleColor.Red;
                    Write("Destroyer [4] ");
                }
                else
                {
                    ForegroundColor = ConsoleColor.DarkGreen;
                    Write("Destroyer [4] ");
                }
            }

            if (x == 4 && y == 10)
            {

                if (navyAsset.IsDestroyer2Sunk)
                {
                    ForegroundColor = ConsoleColor.Red;
                    Write("Destroyer [4] ");
                }
                else
                {
                    ForegroundColor = ConsoleColor.DarkGreen;
                    Write("Destroyer [4] ");
                }
            }

            if (x > 4 && y == 10)
            {
                for (int i = 0; i < 14; i++)
                {
                    Write(" ");
                }
            }

        }

        static void PrintMap(List<Position> positions, EVFNavyAsset MyNavyAsset, EVFNavyAsset EnemyMyNavyAsset, bool showEnemyShips)
        {
            PrintHeader();
            WriteLine();
            if (!showEnemyShips)
                showEnemyShips = MyNavyAsset.IsObliteratedAll;

            List<Position> SortedLFirePositions = positions.OrderBy(o => o.x).ThenBy(n => n.y).ToList();
            List<Position> SortedShipsPositions = EnemyMyNavyAsset.AllShipsPosition.OrderBy(o => o.x).ThenBy(n => n.y).ToList();

            SortedShipsPositions = SortedShipsPositions.Where(FP => !SortedLFirePositions.Exists(ShipPos => ShipPos.x == FP.x && ShipPos.y == FP.y)).ToList();


            int hitCounter = 0;
            int EnemyshipCounter = 0;
            int myShipCounter = 0;
            int enemyHitCounter = 0;

            char row = 'A';
            try
            {
                for (int x = 1; x < 11; x++)
                {
                    for (int y = 1; y < 11; y++)
                    {
                        bool keepGoing = true;

                        #region row indicator
                        if (y == 1)
                        {
                            ForegroundColor = ConsoleColor.DarkYellow;
                            Write("[" + row + "]");
                            row++;
                        }
                        #endregion


                        if (SortedLFirePositions.Count != 0 && SortedLFirePositions[hitCounter].x == x && SortedLFirePositions[hitCounter].y == y)
                        {

                            if (SortedLFirePositions.Count - 1 > hitCounter)
                                hitCounter++;

                            if (EnemyMyNavyAsset.AllShipsPosition.Exists(ShipPos => ShipPos.x == x && ShipPos.y == y))
                            {

                                ForegroundColor = ConsoleColor.Red;
                                Write("[*]");

                                keepGoing = false;

                            }
                            else
                            {
                                ForegroundColor = ConsoleColor.Black;
                                Write("[X]");

                                keepGoing = false;

                            }

                        }

                        if (keepGoing && showEnemyShips && SortedShipsPositions.Count != 0 && SortedShipsPositions[EnemyshipCounter].x == x && SortedShipsPositions[EnemyshipCounter].y == y)

                        {

                            if (SortedShipsPositions.Count - 1 > EnemyshipCounter)
                                EnemyshipCounter++;

                            ForegroundColor = ConsoleColor.DarkGreen;
                            Write("[O]");
                            keepGoing = false;
                        }

                        if (keepGoing)
                        {
                            ForegroundColor = ConsoleColor.Blue;
                            Write("[~]");
                        }


                        PrintStatistic(x, y, MyNavyAsset);


                        if (y == 10)
                        {
                            Write("      ");

                            PrintMapOfEnemy(x, row, MyNavyAsset, EnemyMyNavyAsset, ref myShipCounter, ref enemyHitCounter);
                        }
                    }

                    WriteLine();
                }

            }
            catch (Exception e)
            {
                string error = e.Message.ToString();
            }
        }

        static void PrintMapOfEnemy(int x, char row, EVFNavyAsset MyNavyAsset, EVFNavyAsset EnemyNavyAsset, ref int MyshipCounter, ref int EnemyHitCounter)
        {
            List<Position> EnemyFirePositions = new List<Position>();
            row--;
            Random random = new Random();
            List<Position> SortedLFirePositions = EnemyNavyAsset.FirePositions.OrderBy(o => o.x).ThenBy(n => n.y).ToList();
            List<Position> SortedLShipsPositions = MyNavyAsset.AllShipsPosition.OrderBy(o => o.x).ThenBy(n => n.y).ToList();

            SortedLShipsPositions = SortedLShipsPositions.Where(FP => !SortedLFirePositions.Exists(ShipPos => ShipPos.x == FP.x && ShipPos.y == FP.y)).ToList();


            try
            {

                for (int y = 1; y < 11; y++)
                {
                    bool keepGoing = true;

                    #region row indicator
                    if (y == 1)
                    {
                        ForegroundColor = ConsoleColor.DarkYellow;
                        Write("[" + row + "]");
                        row++;
                    }
                    #endregion


                    if (SortedLFirePositions.Count != 0 && SortedLFirePositions[EnemyHitCounter].x == x && SortedLFirePositions[EnemyHitCounter].y == y)
                    {

                        if (SortedLFirePositions.Count - 1 > EnemyHitCounter)
                            EnemyHitCounter++;

                        if (MyNavyAsset.AllShipsPosition.Exists(ShipPos => ShipPos.x == x && ShipPos.y == y))
                        {

                            ForegroundColor = ConsoleColor.Red;
                            Write("[*]");

                            keepGoing = false;

                        }
                        else
                        {
                            ForegroundColor = ConsoleColor.Black;
                            Write("[X]");

                            keepGoing = false;

                        }

                    }

                    if (keepGoing && SortedLShipsPositions.Count != 0 && SortedLShipsPositions[MyshipCounter].x == x && SortedLShipsPositions[MyshipCounter].y == y)

                    {

                        if (SortedLShipsPositions.Count - 1 > MyshipCounter)
                            MyshipCounter++;

                        ForegroundColor = ConsoleColor.DarkGreen;
                        Write("[O]");

                        keepGoing = false;

                    }

                    if (keepGoing)
                    {
                        ForegroundColor = ConsoleColor.Blue;
                        Write("[~]");
                    }


                    PrintStatistic(x, y, EnemyNavyAsset);

                }


            }
            catch (Exception e)
            {
                string error = e.Message.ToString();
            }
        }

        static Position AnalyzeInput(string input, Dictionary<char, int> Coordinates)
        {
            Position pos = new Position();

            char[] inputSplit = input.ToUpper().ToCharArray();


            if (inputSplit.Length < 2 || inputSplit.Length > 4)
            {
                return pos;
            }




            if (Coordinates.TryGetValue(inputSplit[0], out int value))
            {
                pos.x = value;
            }
            else
            {
                return pos;
            }


            if (inputSplit.Length == 3)
            {

                if (inputSplit[1] == '1' && inputSplit[2] == '0')
                {
                    pos.y = 10;
                    return pos;
                }
                else
                {
                    return pos;
                }

            }


            if (inputSplit[1] - '0' > 9)
            {
                return pos;
            }
            else
            {
                pos.y = inputSplit[1] - '0';
            }

            return pos;
        }

        static void PrintHeader()
        {
            ForegroundColor = ConsoleColor.DarkYellow;
            Write("[ ]");
            for (int i = 1; i < 11; i++)
                Write("[" + i + "]");
        }


        static Dictionary<char, int> PopulateDictionary()
        {
            Dictionary<char, int> Coordinate =
                     new Dictionary<char, int>
                     {
                         { 'A', 1 },
                         { 'B', 2 },
                         { 'C', 3 },
                         { 'D', 4 },
                         { 'E', 5 },
                         { 'F', 6 },
                         { 'G', 7 },
                         { 'H', 8 },
                         { 'I', 9 },
                         { 'J', 10 }
                     };

            return Coordinate;
        }

        static void Commentator(EVFNavyAsset navyAsset, bool isMyShip)
        {

            string title = isMyShip ? "Your" : "Enemy";

            if (navyAsset.CheckPBattleship && navyAsset.IsBattleshipSunk)
            {
                ForegroundColor = ConsoleColor.DarkRed;
                WriteLine("{0} {1} has sunk", title, nameof(navyAsset.Battleship));
                navyAsset.CheckPBattleship = false;
            }

            if (navyAsset.CheckDestroyer && navyAsset.IsDestroyerSunk)
            {
                ForegroundColor = ConsoleColor.DarkRed;
                WriteLine("{0} {1} has sunk", title, nameof(navyAsset.Destroyer));
                navyAsset.CheckDestroyer = false;
            }

            if (navyAsset.CheckDestroyer2 && navyAsset.IsDestroyer2Sunk)
            {
                ForegroundColor = ConsoleColor.DarkRed;
                WriteLine("{0} {1} has sunk", title, nameof(navyAsset.Destroyer2));
                navyAsset.CheckDestroyer2 = false;
            }
            #endregion Game

        }
    }
}