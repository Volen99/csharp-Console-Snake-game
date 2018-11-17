using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Snake
{
    class Program
    {
        static int LevelUp(Queue<Position> snakeElements, int level)
        {
            if (snakeElements.Count == 10 || snakeElements.Count == 14 || snakeElements.Count == 18 || snakeElements.Count == 22)
            {
                level++;
                Console.SetCursorPosition(Console.WindowWidth / 2, 0);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Beep();
                Console.Write("Level {0}", level);
            }

            return level;
        }

        static Position FoodChecker(Position food, Random randomNumbersGenerator,
            List<Position> obstacles, Queue<Position> snakeElements, int level)
        {
            while (true)
            {
                food = new Position(randomNumbersGenerator.Next(0, Console.WindowHeight), //  random index for the food
                    randomNumbersGenerator.Next(0, Console.WindowWidth));

                if (snakeElements.Contains(food) == false || obstacles.Contains(food) == false) // checks if the food is inside an obstacle or snake
                {
                    break;
                }
            }

            return food;
        }

        static void PrintApple(Position food)
        {
            Console.SetCursorPosition(food.Col, food.Row);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("@");
        }

        static void Main()
        {
            Random randomNumbersGenerator = new Random();
            Console.CursorVisible = false;
            Console.Beep();


            byte right = 0;
            byte left = 1;
            byte down = 2;
            byte up = 3;
            int lastFoodTime = 0;
            int foodDissapearTime = 8000;
            int negativePoints = 0;

            Console.SetCursorPosition(Console.WindowWidth / 2, 0);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Level 1");

            Queue<Position> snakeElements = new Queue<Position>(); // For snake growing and changing form.
            for (int i = 0; i <= 5; i++)
            {
                snakeElements.Enqueue(new Position(0, i));
            }

            foreach (Position position in snakeElements) // print snake body. Col grows each iteraion
            {
                Console.SetCursorPosition(position.Col, position.Row);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("*");
            }

            Position[] directions = new Position[]  // WASD for the snake :)
            {
                new Position(0, 1), // right
                new Position(0, -1), // left
                new Position(1, 0), // down
                new Position(-1, 0), // up
            };


            double sleepTime = 100;
            int direction = right;
            Console.BufferHeight = Console.WindowHeight;
            lastFoodTime = Environment.TickCount;

            List<Position> obstacles = new List<Position>()
            {
                //new Position(10, 40),
                //new Position(12, 40),
                //new Position(7, 7),
                //new Position(19, 19),
                //new Position(6, 9),
            };

            for (int i = 0; i <= 15; i++)
            {
                obstacles.Add(new Position(randomNumbersGenerator.Next(Console.WindowHeight),
                    randomNumbersGenerator.Next(Console.WindowWidth)));
            }

            foreach (Position obstacle in obstacles)
            {

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.SetCursorPosition(obstacle.Col, obstacle.Row);
                Console.Write(".");
            }
            int level = 1;

            Position food = new Position();

            food = FoodChecker(food, randomNumbersGenerator, obstacles, snakeElements, level);

            PrintApple(food);

            int pointsCounter = 0;
            while (true) // the endless[sic] cycle!!!
            {
                negativePoints++; // if an apple wasn't eaten for given foodDissapearTime time

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo userInput = Console.ReadKey();
                    if (userInput.Key == ConsoleKey.LeftArrow)
                    {
                        if (direction != right) direction = left;
                    }
                    if (userInput.Key == ConsoleKey.RightArrow)
                    {
                        if (direction != left) direction = right;
                    }
                    if (userInput.Key == ConsoleKey.UpArrow)
                    {
                        if (direction != down) direction = up;
                    }
                    if (userInput.Key == ConsoleKey.DownArrow)
                    {
                        if (direction != up) direction = down;
                    }
                }

                Position snakeHead = snakeElements.Last(); // returns the snake head and keeps it int the variable "newHead";
                Position nextDirection = directions[direction]; // get's the element with index 'direction';

                Position snakeNewHead = new Position(snakeHead.Row + nextDirection.Row, snakeHead.Col + nextDirection.Col);

                // Go through walls
                if (snakeNewHead.Col < 0)
                {
                    snakeNewHead.Col = Console.WindowWidth - 1;
                }

                if (snakeNewHead.Row < 0)
                {
                    snakeNewHead.Row = Console.WindowHeight - 1;
                }
                if (snakeNewHead.Row >= Console.WindowHeight)
                {
                    snakeNewHead.Row = 0;
                }
                if (snakeNewHead.Col >= Console.WindowWidth)
                {
                    snakeNewHead.Col = 0;
                }

                if (snakeElements.Contains(snakeNewHead) || obstacles.Contains(snakeNewHead)) // if (snakeElements.Contains(snakeNewHead) == true)
                                                                                              // means the snake just ate her tail :D
                {
                    Console.SetCursorPosition(0, 0);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Beep(300, 400);
                    Console.WriteLine("Game over!");

                    //if (userPoints < 0) userPoints = 0;
                    pointsCounter = (snakeElements.Count - 6) * 100; // - negativePoints;
                    pointsCounter = Math.Max(pointsCounter, 0);
                    Console.WriteLine("Your points are: {0}", pointsCounter);
                    // Console.ReadLine();
                    return;
                }

                Console.SetCursorPosition(snakeHead.Col, snakeHead.Row);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("*");

                snakeElements.Enqueue(snakeNewHead); // MAKES THE SNAKE MOVE!!! (I hope so)
                Console.SetCursorPosition(snakeNewHead.Col, snakeNewHead.Row);
                Console.ForegroundColor = ConsoleColor.White;

                // Snake head :D
                if (direction == right) Console.Write(">");
                if (direction == left) Console.Write("<");
                if (direction == up) Console.Write("^");
                if (direction == down) Console.Write("v");


                if (snakeNewHead.Col == food.Col && snakeNewHead.Row == food.Row) // if this condition is true, then this means the snake just ate,
                                                                                  //and we have to teleport new apple and obstacle
                {
                    //feeding the snake

                    food = FoodChecker(food, randomNumbersGenerator, obstacles, snakeElements, level);

                    lastFoodTime = Environment.TickCount; // follows the apple time before changes place
                    PrintApple(food);
                    level = LevelUp(snakeElements, level); // ...
                    sleepTime--;

                    Position obstacle = new Position();
                    while (true)
                    {
                        obstacle = new Position(randomNumbersGenerator.Next(0, Console.WindowHeight),
                            randomNumbersGenerator.Next(0, Console.WindowWidth));

                        if (snakeElements.Contains(obstacle) == false || obstacles.Contains(obstacle) // apple won't be born within the snake or obstacle!
                            || (food.Row != obstacle.Row && food.Col != obstacle.Row) == false) // might mistake with the false up.
                        {
                            break;
                        }
                    }

                    obstacles.Add(obstacle);
                    Console.SetCursorPosition(obstacle.Col, obstacle.Row);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(".");
                }
                else
                {
                    // moving...
                    Position last = snakeElements.Dequeue();  // removes the last element and saves it 
                    Console.SetCursorPosition(last.Col, last.Row);
                    Console.Write(" ");
                }

                if (Environment.TickCount - lastFoodTime >= foodDissapearTime)
                {
                    negativePoints += 10;
                    Console.SetCursorPosition(food.Col, food.Row);
                    Console.Write(" ");

                    while (true)
                    {
                        food = new Position(randomNumbersGenerator.Next(0, Console.WindowHeight),
                            randomNumbersGenerator.Next(0, Console.WindowWidth));

                        if (snakeElements.Contains(food) == false || obstacles.Contains(food) == false)
                        {
                            break;
                        }
                    }

                    lastFoodTime = Environment.TickCount;
                }

                PrintApple(food);

                sleepTime -= 0.04; // speed during time elapsed

                Thread.Sleep((int)sleepTime); // this function defines how much time the Console stops,
                                              // so that our eyes can see the image.
            }
        }
    }
}