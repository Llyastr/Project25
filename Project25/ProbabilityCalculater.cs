using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project25
{
    class ProbabilityCalculater
    {
        int BoardWidth = 10;
        int BoardHeight = 10;
        int BoardSize
        {
            get { return BoardHeight * BoardWidth; }
        }

        public ProbabilityCalculater()
        {

        }

        public int CalculateNextShot(int[] shipSizes, List<int[]> SunkenShips, int[] state)
        {
            int[] Probabilities = CalculateProbabilityDensity(shipSizes, SunkenShips, state);
            return new List<int>(Probabilities).IndexOf(Enumerable.Max(Probabilities));
        }

        public static int PickMax(int[] density)
        {
            int MaxDensity = Enumerable.Max(density);
            List<int> MaxIndexes = new List<int>();
            for (int i = 0; i < density.Length; i++)
            {
                if (density[i] == MaxDensity)
                {
                    MaxIndexes.Add(i);
                }
            }
            Random Random = new Random();
            int RandomIndex = Random.Next(MaxIndexes.Count);
            return MaxIndexes[RandomIndex];
        }

        public int[] CalculateProbabilityDensity(int[] shipSizes, List<int[]> SunkenShips, int[] state)
        {
            bool IsOccupied(int square)
            {
                return state[square] != 0;
            }
            bool IsValidPosition(List<int> ship)
            {
                foreach (int i in ship)
                {
                    if (IsOccupied(i))
                    {
                        return false;
                    }
                }
                return true;
            }

            int[] Probabilities = new int[BoardSize];

            List<int> ShipSizes = new List<int>(shipSizes);
            List<int> SunkenShipSizes = new List<int>();
            foreach (int[] ship in SunkenShips)
            {
                SunkenShipSizes.Add(ship.Length);
            }
            foreach (int size in SunkenShipSizes)
            {
                ShipSizes.Remove(size);
            }

            foreach (int shipSize in ShipSizes)
            {
                int MaxColumn = BoardWidth - shipSize;
                int OffsetHorizontal = shipSize - 1;
                for (int i = 0; i < BoardSize; i++)
                {
                    List<int> TempInt = new List<int>();
                    for (int n = 0; n < shipSize; n++)
                    {
                        TempInt.Add(i + n);
                    }
                    if (IsValidPosition(TempInt))
                    {
                        foreach (int square in TempInt)
                        {
                            Probabilities[square]++;
                        }
                    }

                    if (i % BoardWidth == MaxColumn)
                    {
                        i += OffsetHorizontal;
                    }
                }

                int IterationsVertical = BoardHeight - shipSize + 1;
                int MaxVertical = ((BoardHeight - shipSize + 1) * BoardWidth) - 1;

                for (int i = 0; i <= MaxVertical; i++)
                {
                    List<int> TempInt = new List<int>();
                    for (int n = 0; n < shipSize; n++)
                    {
                        TempInt.Add(i + (n * BoardWidth));
                    }
                    if (IsValidPosition(TempInt))
                    {
                        foreach (int square in TempInt)
                        {
                            Probabilities[square]++;
                        }
                    }
                }
            }

            List<int> LoneHits = GetHits(state);
            foreach (int[] ship in SunkenShips)
            {
                foreach (int i in ship)
                {
                    LoneHits.Remove(i);
                }
            }
            List<int> ToSearch = new List<int>();
            foreach (int i in LoneHits)
            {
                bool IsInLoneHits(int square)
                {
                    foreach (int hit in LoneHits)
                    {
                        if (square == hit)
                        {
                            return true;
                        }
                    }
                    return false;
                }

                bool IsBorderless()
                {
                    int[] Borders = GenerateBorders();
                    foreach (int border in Borders)
                    {
                        if (IsInLoneHits(border))
                        {
                            return false;
                        }
                    }
                    return true;
                }

                int[] GenerateBorders()
                {
                    List<int> TempBorders = new List<int>();
                    void CheckBorder(Func<int, bool> boundaryCheck, int offset)
                    {
                        if (!boundaryCheck(i))
                        {
                            TempBorders.Add(i + offset);
                        }
                    }
                    CheckBorder(IsOnTop, -BoardWidth);
                    CheckBorder(IsOnBottom, BoardWidth);
                    CheckBorder(IsOnLeft, -1);
                    CheckBorder(IsOnRight, 1);
                    return TempBorders.ToArray();
                }

                void CheckDirection(Func<int, bool> boundaryCheck, int offset)
                {
                    int OffsettedSquareCheck = i - offset;
                    int OffsettedSquare = i + offset;
                    if (!boundaryCheck(i) && 
                        IsInLoneHits(OffsettedSquareCheck) && 
                        !IsInLoneHits(OffsettedSquare) &&
                        !IsOccupied(OffsettedSquare))
                    {
                        ToSearch.Add(OffsettedSquare);
                    }
                }

                int[] Borders = GenerateBorders();
                foreach (int border in Borders)
                {
                    if (!IsOccupied(border))
                    {
                        Probabilities[border] += 40;
                    }
                }

                if (IsBorderless())
                {
                    foreach (int border in Borders)
                    {
                        ToSearch.Add(border);
                    }
                }
                else
                {
                    CheckDirection(IsOnTop, -BoardWidth);
                    CheckDirection(IsOnBottom, BoardWidth);
                    CheckDirection(IsOnLeft, -1);
                    CheckDirection(IsOnRight, 1);
                }
            }
            foreach (int i in ToSearch)
            {
                Probabilities[i] = 100;
            }

            bool ContainsToSearch(List<int> ship)
            {
                foreach (int i in ship)
                {
                    foreach (int n in ToSearch)
                    {
                        if (i == n)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            if (ToSearch.Count > 0)
            {
                foreach (int shipSize in ShipSizes)
                {
                    int MaxColumn = BoardWidth - shipSize;
                    int OffsetHorizontal = shipSize - 1;
                    for (int i = 0; i < BoardSize; i++)
                    {
                        List<int> TempInt = new List<int>();
                        for (int n = 0; n < shipSize; n++)
                        {
                            TempInt.Add(i + n);
                        }
                        if (IsValidPosition(TempInt) && ContainsToSearch(TempInt))
                        {
                            foreach (int square in TempInt)
                            {
                                Probabilities[square]++;
                            }
                        }

                        if (i % BoardWidth == MaxColumn)
                        {
                            i += OffsetHorizontal;
                        }
                    }

                    int IterationsVertical = BoardHeight - shipSize + 1;
                    int MaxVertical = ((BoardHeight - shipSize + 1) * BoardWidth) - 1;

                    for (int i = 0; i <= MaxVertical; i++)
                    {
                        List<int> TempInt = new List<int>();
                        for (int n = 0; n < shipSize; n++)
                        {
                            TempInt.Add(i + (n * BoardWidth));
                        }
                        if (IsValidPosition(TempInt))
                        {
                            foreach (int square in TempInt)
                            {
                                Probabilities[square]++;
                            }
                        }
                    }
                }
            }

            return Probabilities;
        }

        private List<int> GetHits(int[] state)
        {
            List<int> TempHits = new List<int>();
            for (int i = 0; i < state.Length; i++)
            {
                if (state[i] == 2)
                {
                    TempHits.Add(i);
                }
            }
            return TempHits;
        }

        public bool IsOnTop(int square)
        {
            Tuple<int, int> RC = ToRowColumn(square);
            if (RC.Item1 == 0)
            {
                return true;
            }
            return false;
        }

        public bool IsOnBottom(int square)
        {
            Tuple<int, int> RC = ToRowColumn(square);
            if (RC.Item1 == BoardHeight - 1)
            {
                return true;
            }
            return false;
        }

        public bool IsOnLeft(int square)
        {
            Tuple<int, int> RC = ToRowColumn(square);
            if (RC.Item2 == 0)
            {
                return true;
            }
            return false;
        }

        public bool IsOnRight(int square)
        {
            Tuple<int, int> RC = ToRowColumn(square);
            if (RC.Item2 == BoardWidth - 1)
            {
                return true;
            }
            return false;
        }

        private Tuple<int, int> ToRowColumn(int square)
        {
            int Column = square % BoardWidth;
            int Row = (square - Column) / BoardWidth;

            return new Tuple<int, int>(Row, Column);
        }
    }
}
