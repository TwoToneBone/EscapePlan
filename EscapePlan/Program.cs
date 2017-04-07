using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EscapePlan
{
    class Program
    {
        public List<Scenario> _scenarios { get; set; }
        public Program()
        {
            _scenarios = new List<Scenario>();
        }

        static void Main(string[] args)
        {
            Program program = new Program();

            // Collects input.
            program.CollectInputs();

            // Calculates which robots are within reach of a hole in every scenario.
            program.UnleashStorm();

            // Figure out which robot is closest to every hole.
            program.PopulateHoles();

            // Finally do the output to console.
            program.Output();

        }

        void Output()
        {
            Console.Clear();

            foreach (var scenario in _scenarios)
            {
                Console.WriteLine($"Scenario {scenario.Id}");

                foreach (var duration in scenario.Durations)
                {
                    List<Robot> survivors = duration.Robots.FindAll(r => r.IsHiding == true);

                    Console.WriteLine($"In {duration.Time} seconds {survivors.Count} robot(s) can escape");
                }

                Console.WriteLine();
            }
        }

        void PopulateHoles()
        {
            foreach (var scenario in _scenarios)
            {
                foreach (var duration in scenario.Durations)
                {
                    foreach (var hole in duration.Holes)
                    {
                        var robotsInOrder = hole.Candidates.OrderBy(a => a.Value);

                        foreach (var pair in robotsInOrder)
                        {
                            Robot robot = _scenarios
                              .Find(a => a.Id == scenario.Id)
                              .Durations.Where(b => b.Time == duration.Time)
                              .Select(b => b.Robots
                              .Find(c => c.Id == pair.Key))
                              .SingleOrDefault();

                            if (!robot.IsHiding)
                            {
                                if (!hole.IsOccupied)
                                {
                                    hole.IsOccupied = true;
                                    robot.IsHiding = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        void UnleashStorm()
        {
            foreach (var scenario in _scenarios)
            {
                foreach (var duration in scenario.Durations)
                {
                    double movementCapacity = duration.Time * 10.0f;

                    foreach (var hole in duration.Holes)
                    {
                        Dictionary<int, double> robotsInRange = new Dictionary<int, double>();

                        foreach (var robot in duration.Robots)
                        {
                            double distanceToX = robot.PosX > hole.PosX ? robot.PosX - hole.PosX : hole.PosX - robot.PosX;
                            double distanceToY = robot.PosY > hole.PosY ? robot.PosY - hole.PosY : hole.PosY - robot.PosY;

                            double distanceToHole = Math.Sqrt(Math.Pow(distanceToX, 2) + Math.Pow(distanceToY, 2));

                            if (distanceToHole <= movementCapacity)
                            {
                                robotsInRange.Add(robot.Id, distanceToHole);
                            }
                        }

                        hole.Candidates = new Dictionary<int, double>(robotsInRange);
                    }
                }
            }
        }

        void CollectInputs()
        {
            for (int i = 0; i < 10; i++)
            {
                Scenario scenario = new Scenario(i + 1);
                List<Duration> durations = new List<Duration>();

                List<Robot> robots = new List<Robot>();
                List<Hole> holes = new List<Hole>();

                int newRobots = int.Parse(Console.ReadLine());
                if (newRobots == 0)
                {
                    break;
                }

                for (int ii = 0; ii < newRobots; ii++)
                {


                    string[] inputCoordsRobot = Console.ReadLine().Split(' ');
                    Robot robot = new Robot(double.Parse(inputCoordsRobot[0], CultureInfo.InvariantCulture), double.Parse(inputCoordsRobot[1], CultureInfo.InvariantCulture), ii + 1);
                    robots.Add(robot);


                }

                int newHoles = int.Parse(Console.ReadLine());
                for (int ii = 0; ii < newHoles; ii++)
                {
                    string[] inputCoordsHole = Console.ReadLine().Split(' ');
                    Hole hole = new Hole(double.Parse(inputCoordsHole[0], CultureInfo.InvariantCulture), double.Parse(inputCoordsHole[1], CultureInfo.InvariantCulture));
                    holes.Add(hole);
                }

                for (int ii = 5; ii <= 20; ii = ii + ii)
                {

                    Duration duration = new Duration(ii);

                    duration.Robots = new List<Robot>(robots.Select(r => r.Clone() as Robot));
                    duration.Holes = new List<Hole>(holes.Select(h => h.Clone() as Hole));

                    durations.Add(duration);

                    scenario.Durations = new List<Duration>(durations.Select(d => d.Clone() as Duration));

                }
                _scenarios.Add(scenario);
            }

        }

    }

    class Robot : ICloneable
    {
        public int Id { get; private set; }
        public double PosX { get; private set; }
        public double PosY { get; private set; }
        public bool IsHiding { get; set; }

        public Robot(double posX, double posY, int id)
        {
            PosX = posX;
            PosY = PosY;
            Id = id;
        }

        public object Clone()
        {
            var robot = new Robot(this.PosX, this.PosY, this.Id)
            {
                IsHiding = this.IsHiding
            };
            return robot;
        }
    }

    class Hole : ICloneable
    {
        public Dictionary<int, double> Candidates { get; set; }
        public double PosX { get; private set; }
        public double PosY { get; private set; }
        public bool IsOccupied { get; set; }
        public Hole(double posX, double posY)
        {
            PosX = posX;
            PosY = posY;
        }

        public object Clone()
        {
            var hole = new Hole(this.PosX, this.PosY)
            {
                IsOccupied = this.IsOccupied,
                Candidates = this.Candidates

            };

            return hole;

        }
    }

    class Scenario
    {
        public int Id { get; private set; }
        public List<Duration> Durations { get; set; }

        public Scenario(int id)
        {
            Id = id;
        }
    }

    class Duration : ICloneable
    {
        public int Time { get; private set; }
        public int Survivors { get; set; }
        public List<Robot> Robots { get; set; }
        public List<Hole> Holes { get; set; }

        public Duration(int time)
        {
            Time = time;
        }

        public object Clone()
        {
            var duration = new Duration(this.Time)
            {
                Survivors = this.Survivors,
                Robots = this.Robots,
                Holes = this.Holes
            };
            return duration;
        }
    }


}