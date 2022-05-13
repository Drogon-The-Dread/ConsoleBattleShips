using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleShipConsole.Models
{
    class EVFNavyAsset //I made a new class for EVF battleships as in the email the sizes and names where diffrent from regular Battleship naming convention. 
    {
        Random random = new Random();
        private const int BATTLESHIP = 5;
        private const int DESTROYER = 4;
        private const int DESTROYER2 = 4;

        public EVFNavyAsset()
        {
            Battleship = GeneratePosistion(BATTLESHIP, AllShipsPosition);
            Destroyer = GeneratePosistion(DESTROYER, AllShipsPosition);
            Destroyer2 = GeneratePosistion(DESTROYER2, AllShipsPosition);
        }

        public int StepsTaken { get; set; } = 0;

        public List<Position> Battleship { get; set; }//5
        public List<Position> Destroyer { get; set; }//4
        public List<Position> Destroyer2 { get; set; }//4
        public List<Position> AllShipsPosition { get; set; } = new List<Position>();
        public List<Position> FirePositions { get; set; } = new List<Position>();

        public bool IsBattleshipSunk { get; set; } = false;
        public bool IsDestroyerSunk { get; set; } = false;
        public bool IsDestroyer2Sunk { get; set; } = false;
        public bool IsObliteratedAll { get; set; } = false;

        public bool CheckPBattleship { get; set; } = true;
        public bool CheckDestroyer { get; set; } = true;
        public bool CheckDestroyer2 { get; set; } = true;

        public EVFNavyAsset CheckShipStatus(List<Position> HitPositions)
        {

            IsBattleshipSunk = Battleship.Where(B => !HitPositions.Any(H => B.x == H.x && B.y == H.y)).ToList().Count == 0;
            IsDestroyerSunk = Destroyer.Where(D => !HitPositions.Any(H => D.x == H.x && D.y == H.y)).ToList().Count == 0;
            IsDestroyer2Sunk = Destroyer2.Where(D => !HitPositions.Any(H => D.x == H.x && D.y == H.y)).ToList().Count == 0;


            IsObliteratedAll = IsBattleshipSunk && IsDestroyerSunk && IsDestroyer2Sunk;
            return this;
        }

        public List<Position> GeneratePosistion(int size, List<Position> AllPosition)
        {
            List<Position> positions = new List<Position>();

            bool IsExist = false;

            do
            {
                positions = GeneratePositionRandomly(size);
                IsExist = positions.Where(AP => AllPosition.Exists(ShipPos => ShipPos.x == AP.x && ShipPos.y == AP.y)).Any();
            }
            while (IsExist);

            AllPosition.AddRange(positions);


            return positions;
        }

        public List<Position> GeneratePositionRandomly(int size)
        {
            List<Position> positions = new List<Position>();

            int direction = random.Next(1, size); //odd for horizontal and even for vertical
                                                  //pick row and column
            int row = random.Next(1, 11);
            int col = random.Next(1, 11);

            if (direction % 2 != 0)
            {
                //left first, then right
                if (row - size > 0)
                {
                    for (int i = 0; i < size; i++)
                    {
                        Position pos = new Position();
                        pos.x = row - i;
                        pos.y = col;
                        positions.Add(pos);
                    }
                }
                else // row
                {
                    for (int i = 0; i < size; i++)
                    {
                        Position pos = new Position();
                        pos.x = row + i;
                        pos.y = col;
                        positions.Add(pos);
                    }
                }
            }
            else
            {
                //top first, then bottom
                if (col - size > 0)
                {
                    for (int i = 0; i < size; i++)
                    {
                        Position pos = new Position();
                        pos.x = row;
                        pos.y = col - i;
                        positions.Add(pos);
                    }
                }
                else // row
                {
                    for (int i = 0; i < size; i++)
                    {
                        Position pos = new Position();
                        pos.x = row;
                        pos.y = col + i;
                        positions.Add(pos);
                    }
                }
            }
            return positions;
        }
        public EVFNavyAsset Fire()
        {
            Position EnemyShotPos = new Position();
            bool alreadyShot = false;
            do
            {
                EnemyShotPos.x = random.Next(1, 11);
                EnemyShotPos.y = random.Next(1, 11);
                alreadyShot = FirePositions.Any(EFP => EFP.x == EnemyShotPos.x && EFP.y == EnemyShotPos.y);
            }
            while (alreadyShot);

            FirePositions.Add(EnemyShotPos);
            return this;
        }
    }
}
