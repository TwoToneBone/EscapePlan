using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapePlan
{
    class Program
    {
        public static List<Scenario> _scenarios = new List<Scenario>();

        static void Main(string[] args)
        {
            // Collects input.
            CollectInputs();

            // Calculates which robots are within reach of a hole in every scenario.
            UnleashStorm();

            // Figure out which robot is closest to every hole.
            PopulateHoles();

            // Finally do the output to console.
            Output();

        }

        private static void Output()
        {
            Console.Clear();

            foreach (var scenario in _scenarios)
            {
                Console.WriteLine($"Scenario {scenario.Id}");

                foreach (var duration in scenario.Durations)
                {
                    List<Robot> survivors = duration.Robots.FindAll(r => r.isHiding == true);

                    Console.WriteLine($"In {duration.Time} seconds {survivors.Count} robot(s) can escape");
                }

                Console.WriteLine();
            }
        }

        private static void PopulateHoles()
        {
            foreach (var scenario in _scenarios)
            {

                foreach (var duration in scenario.Durations)
                {
                    foreach (var hole in duration.Holes)
                    {
                        int RobotId = hole.RobotsInRange.OrderBy(a=> a.Value).FirstOrDefault().Key;

                        IEnumerable<Robot> robot = _scenarios.Find(a => a.Id == scenario.Id)
                            .Durations.Where(b => b.Time == duration.Time)
                            .Select(b => b.Robots
                            .Find(c => c.Id == RobotId));
                        

                    }
                }
            }

        }

        private static void UnleashStorm()
        {
            foreach (var scenario in _scenarios)
            {
                foreach (var duration in scenario.Durations)
                {
                    double movementCapacity = duration.Time * 10.0f;

                    foreach (var hole in duration.Holes)
                    {
                        hole.RobotsInRange = new Dictionary<int, double>();

                        foreach (var robot in duration.Robots)
                        {
                            double distanceToX = robot.PosX > hole.PosX ? robot.PosX - hole.PosX : hole.PosX - robot.PosX;
                            double distanceToY = robot.PosY > hole.PosY ? robot.PosY - hole.PosY : hole.PosY - robot.PosY;

                            double distanceToHole = Math.Sqrt(Math.Pow(distanceToX, 2) + Math.Pow(distanceToY, 2));

                            if (distanceToHole <= movementCapacity)
                            {
                                hole.RobotsInRange.Add(robot.Id, distanceToHole);
                            }

                            distanceToX = 0;
                            distanceToY = 0;
                            distanceToHole = 0;
                        }

                        
                    }
                    movementCapacity = 0;
                }
            }
        }

        private static void CollectInputs()
        {
            for (int i = 0; i < 10; i++)
            {
                int newRobots = int.Parse(Console.ReadLine());

                if (newRobots == 0)
                {
                    break;
                }

                List<Robot> robots = new List<Robot>();
                List<Hole> holes = new List<Hole>();

                for (int ii = 0; ii < newRobots; ii++)
                {
                    string[] inputCoords = Console.ReadLine().Split(' ');

                    Robot robot = new Robot(double.Parse(inputCoords[0], CultureInfo.InvariantCulture), double.Parse(inputCoords[1], CultureInfo.InvariantCulture), ii + 1);
                    robots.Add(robot);
                }

                int newHoles = int.Parse(Console.ReadLine());

                for (int ii = 0; ii < newHoles; ii++)
                {
                    string[] inputCoords = Console.ReadLine().Split(' ');
                    Hole hole = new Hole(double.Parse(inputCoords[0], CultureInfo.InvariantCulture), double.Parse(inputCoords[1], CultureInfo.InvariantCulture), ii + 1);
                    holes.Add(hole);
                }

                Scenario scenario = new Scenario();

                for (int ii = 5; ii <= 20; ii = ii + ii)
                {
                    Duration duration = new Duration(ii);

                    duration.Robots = robots;
                    duration.Holes = holes;

                    scenario.Durations.Add(duration);
                }

                _scenarios.Add(scenario);

            }

        }
    }

    internal class Robot
    {
        public int Id { get; set; }
        public double PosX { get; set; }
        public double PosY { get; set; }
        public bool isHiding { get; set; }

        public Robot(double posX, double posY, int id)
        {
            PosX = posX;
            PosY = PosY;
            Id = id;
        }
    }

    internal class Hole
    {
        public Dictionary<int, double> RobotsInRange { get; set; }
        public int Id { get; set; }
        public double PosX { get; set; }
        public double PosY { get; set; }
        public bool isOccupied { get; set; }
        public Hole(double posX, double posY, int id)
        {
            PosX = posX;
            PosY = posY;
            Id = id;
           
        }
    }

    internal class Scenario
    {
        public int Id { get; set; }
        public List<Duration> Durations { get; set; }

        public Scenario()
        {
            Durations = new List<Duration>();
        }
    }

    internal class Duration
    {
        public int Time { get; set; }
        public int Survivors { get; set; }
        public List<Robot> Robots { get; set; }
        public List<Hole> Holes { get; set; }

        public Duration(int time)
        {
            Time = time;
        }

    }
}