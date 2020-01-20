using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pathfinding.Dstar
{
    public static class Constants
    {
        public static int BoardSize = 250;
        public static double M_SQRT2 = 0.41421356237309504880;
        public static int Obsticle_Size = 20;
    }
    public class Points<T, U>
    {
        public Points()
        { }
        public Points(T first, U second)
        {
            this.X = first;
            this.Y = second;
        }
        public T X { get; set; }
        public U Y { get; set; }

    };
    public class Tile
    {
        public int X = 0,
                   Y = 0;

        public double Cost = double.MaxValue,
                      LookAhead = double.MaxValue,
                      Key1 = 0,
                      Key2 = 0,
                      H = 0;
        public bool nonPassable = false,
                    Visted = false;
        public List<Points<int, int>> Pointss = new List<Points<int, int>>();
        public Tile(int x, int y, double h, bool nonPassable = false)
        {
            X = x;
            Y = y;
            H = h;
            this.nonPassable = nonPassable;
            var xmin = X - 1 == -1 ? X : X - 1;
            var xmax = X + 1 > Constants.BoardSize ? X : X + 1;

            var ymin = Y - 1 == -1 ? Y : Y - 1;
            var ymax = Y + 1 > Constants.BoardSize ? Y : Y + 1;

            Pointss.Capacity = 9;
            Parallel.For(xmin, xmax + 1, i =>
            {
                //Parallel.For(ymin, ymax, j =>
                for (int j = ymin; j <= ymax; ++j)
                {
                    if (!(X == i && Y == j))
                        Pointss.Add(new Points<int, int>(i, j));
                    //});
                }
            });
            //});
        }
    }
    public class KeySort : IComparer<Tile>
    {
        public int Compare(Tile x, Tile y) => y.Cost.CompareTo(x.Cost) == 0 ? y.Key2.CompareTo(x.Key2) : y.Cost.CompareTo(x.Cost);
    }
    public class SortRHS : IComparer<Tile>
    {
        public int Compare(Tile x, Tile y) => x.LookAhead.CompareTo(y.LookAhead) == 0 ? x.LookAhead.CompareTo(y.LookAhead) : x.Key2.CompareTo(y.Key2);

    }
    public class Sort : IComparer<Tile>
    {
        public int Compare(Tile x, Tile y) => x.LookAhead.CompareTo(y.LookAhead);

    }

    public class Distintinc : IEqualityComparer<Tile>
    {
        public bool Equals(Tile x, Tile y) => x.X == y.X && x.Y == y.Y;
        public int GetHashCode(Tile obj) => obj.GetHashCode();

    }
    
    public class Pather
    {
        #region parameter
        public Tile[,] Map;
        public HashSet<Tile> OpenList = new HashSet<Tile>(new Distintinc());
        public HashSet<Tile> CloseList = new HashSet<Tile>(new Distintinc());
        public HashSet<Tile> BlockedList = new HashSet<Tile>(new Distintinc());
        private int BoardSize => Constants.BoardSize;
        private Tile Goal = new Tile(0, 0, 0, false);
        private Random stuff = new Random();
        #endregion
        public Pather()
        {
            Map = new Tile[BoardSize + 1, BoardSize + 1];
        }
        public void GenerateObsticle(int X,int Y)
        {
            
            var xmin = X - (Constants.Obsticle_Size/2) < 0 ? 0 : (int)(X - (Constants.Obsticle_Size / 2));
            var xmax = X + (Constants.Obsticle_Size /2) > Constants.BoardSize ? Constants.BoardSize : (int)(X + (Constants.Obsticle_Size / 2));

            int ymin = Y - (Constants.Obsticle_Size/2) < 0  ? 0 :(int) (Y - (Constants.Obsticle_Size / 2));
            var ymax = Y + (Constants.Obsticle_Size / 2) > Constants.BoardSize ? Constants.BoardSize : (int)(Y + (Constants.Obsticle_Size / 2));
            int space = (int)(Constants.Obsticle_Size/2) - 1;
            for (int i = ymin; i < Y; i++)
            {
                for (int j = xmin+space; j < xmax-space; j++)
                    Map[j, i].nonPassable = true;
                space--;
            }
            space = (int)(Constants.Obsticle_Size / 2);
            for (int i = ymax; i >= Y; i--)
            {
                for (int j = xmin + space; j < xmax- space; j++)
                    Map[j, i].nonPassable = true;
                space--;
            }
        }
        public void GenerateMap(int rovarx, int rovary)
        {
            
            Parallel.For(0, BoardSize + 1, x =>
            {
                Parallel.For(0, BoardSize + 1, y =>
                {
                   Map[x, y] = new Tile(x, y, Heuristic(x, y, rovarx, rovary), false);
                });
            });
            GenerateObsticle(100, 100);
            GenerateObsticle(225, 225);
            GenerateObsticle(225, 225 +15);
            GenerateObsticle(225, 225-15);
            GenerateObsticle(225, 225 - 30);
            
            foreach (var tile in Map)
            {
                if(tile.nonPassable)
                    BlockedList.Add(tile);
            }
        }
        private Tile RoverTile = new Tile(0, 0, 0, false);
        public void GenerateCostMap(Robot Rover, int X, int Y)
        {
            Map[X, Y].LookAhead = 0;
            Map[X, Y].Cost = 0;
            Map[X, Y].Visted = true;
            Goal = Map[X, Y];
            RoverTile = Map[Rover.X, Rover.Y];
            
            OpenList.Add(Map[X, Y]);
            while (OpenList.Count > 0)
            {
                Tile priority = OpenList.First();
                OpenList.Remove(OpenList.First());
                Map[priority.X, priority.Y].Visted = true;
                Map[priority.X, priority.Y].Cost = Goal.X == priority.X && Goal.Y == priority.Y ? 0 : CalculateCurrentCost(priority);
                CalculatePiority(priority);
                UpdatePointss(Map[priority.X, priority.Y]);
                OpenList.OrderBy(p => p.LookAhead);
                CloseList.Add(Map[priority.X, priority.Y]);

            }


            return;
        }
        public List<Tile> ShortestPath = new List<Tile>();
        public void FindClosedPath()
        {
            CloseList.OrderByDescending(p => p.Cost).ThenByDescending(p => p.Key2);
            ShortestPath.Add(RoverTile);
            for (int i = 0; i < ShortestPath.Count; ++i)
            {
   
                //                ShortestPath.Distinct(new Distintinc());
                if (ShortestPath[i].X == Goal.X && ShortestPath[i].Y == Goal.Y)
                    return;
                
                ShortestPath.Add(Lowest(ShortestPath[i]));
                ShortestPath = ShortestPath.Distinct(new Distintinc()).ToList();
                //ShortestPath.Add(Lowest(ShortestPath[i]));
            }
        }
        private Tile Lowest(Tile PathTile)
        {
            Tile LowestTile = new Tile(0, 0, 0);
            var min = double.MaxValue;
            foreach (Points<int, int> Current in PathTile.Pointss)
            {
                foreach (Tile CheckTile in CloseList)
                {
                    if (Current.X == CheckTile.X && Current.Y == CheckTile.Y && CheckTile.Cost < min)
                    {
                        LowestTile = CheckTile;
                        min = CheckTile.Cost;
                    }
                }
            }
            return LowestTile;
        }

        private void CalculatePiority(Tile currentTile)
        {
            double val = Math.Min(currentTile.Cost, currentTile.LookAhead);
            Map[currentTile.X, currentTile.Y].Key1 = currentTile.Cost + currentTile.H;
            Map[currentTile.X, currentTile.Y].Key2 = val;
        }
        private void UpdatePointss(Tile Home)
        {
            //Parallel.ForEach(Home.Pointss, dir =>
            foreach (Points<int, int> dir in Home.Pointss)
            {
                var H = !Map[dir.X, dir.Y].nonPassable ? Heuristic(Home, Map[dir.X, dir.Y]) + Map[dir.X, dir.Y].H : double.MaxValue;
                if (H < Map[dir.X, dir.Y].LookAhead)
                {
                    Map[dir.X, dir.Y].LookAhead = H;
                    OpenList.Add(Map[dir.X, dir.Y]);
                    Map[dir.X, dir.Y].Visted = false;

                }
            }//);
            return;
        }
        private double CalculateCurrentCost(Tile Current)
        {
            double min = double.MaxValue;

            foreach (Points<int, int> neighboor in Current.Pointss)
            {
                var H = Heuristic(Current, Map[neighboor.X, neighboor.Y]);
                if (Map[neighboor.X, neighboor.Y].Cost + H <= min)
                    min = Map[neighboor.X, neighboor.Y].Cost + H;
            }

            Map[Current.X, Current.Y].Visted = true;
            return min;
        }
        private double Heuristic(Tile a, Tile b)
        {
            return EightCondist(a, b);
        }
        private double Heuristic(int x1, int y1, int goalx, int goaly)
        {
            double min = (x1 - goalx) < 0 ? -(x1 - goalx) : (x1 - goalx),
                   max = (y1 - goaly) < 0 ? -(y1 - goaly) : (y1 - goaly);
            //min = Math.Abs(a.X - b.X);
            //max = Math.Abs(a.Y - b.Y);
            /*if (min > max)
            {
                temp = min > max ? max : min;
                min =  max  ;
                max = temp;
            }*/

            /*     min = min > max ? max : min;
                 max = min > max ? min : max;*/

            return Constants.M_SQRT2 * (min > max ? max : min) + (min > max ? min : max);
        }
        private double EightCondist(Tile a, Tile b)
        {
            double min = (a.X - b.X) < 0 ? -(a.X - b.X) : (a.X - b.X),
                   max = (a.Y - b.Y) < 0 ? -(a.Y - b.Y) : (a.Y - b.Y);
            //min = Math.Abs(a.X - b.X);
            //max = Math.Abs(a.Y - b.Y);
            /*if (min > max)
            {
                temp = min > max ? max : min;
                min =  max  ;
                max = temp;
            }*/

            /*     min = min > max ? max : min;
                 max = min > max ? min : max;*/

            return Constants.M_SQRT2 * (min > max ? max : min) + (min > max ? min : max);
        }

    }
}
