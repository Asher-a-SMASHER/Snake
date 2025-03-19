//TODO restart feature
//TODO Apple shouldn't spawn within 3 char of edge of map
//TODO Scores
//TODO Mode with random speeds within a range
//TODO High scores

Exception? exception = null;
int speedInput;
string restart = "yes";
string prompt = $"Select speed from 1 - 10:  ";
string? input;
while (restart is "yes" or "Yes")
{
    while (IsUserInputValid() == false)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            speedInput = 5;
            break;
        }
        else
        {
            Console.WriteLine("Invalid Input. Try Again...");
            Console.Clear();
        }
    }
    int[] velocities = [250, 210, 175, 150, 125, 100, 75, 65, 57, 50];
    int velocity = velocities[speedInput - 1];
    char[] DirectionChars = ['^', 'v', '<', '>'];
    Console.BackgroundColor = ConsoleColor.White;
    int width = Console.WindowWidth;
    int height = Console.WindowHeight;
    Tile[,] board = new Tile[width, height];
    Direction? direction = null;
    Queue<(int X, int Y)> snake = new();
    (int X, int Y) = (width / 2, height / 2);
    bool CloseRequested = false;

    try
    {
        Console.CursorVisible = false;
        Console.Clear();
        snake.Enqueue((X, Y));
        board[X, Y] = Tile.Snake;
        (int FoodX, int FoodY) = PositionFood(width, height, board);
        board[FoodX, FoodY] = Tile.Food;
        Console.SetCursorPosition(X, Y);
        Console.Write('S');
        while (direction is null && !CloseRequested)
        {
            (direction, CloseRequested) = GetDirection(direction, CloseRequested);
        }
        while (!CloseRequested)
        {
            if (Console.WindowWidth != width || Console.WindowHeight != height)
            {
                Console.Clear();
                Console.Write("Console was resized. Snake game has ended.");
                return;
            }
            switch (direction)
            {
                case Direction.Up:
                    Y--;
                    break;
                case Direction.Down:
                    Y++;
                    break;
                case Direction.Left:
                    X--;
                    break;
                case Direction.Right:
                    X++;
                    break;
            }
            if (X < 0 || X >= width ||
                Y < 0 || Y >= height ||
                board[X, Y] is Tile.Snake)
            {
                Console.Clear();
                Console.Write("Game Over. Score: " + (snake.Count - 1) + ".");
                return;
            }
            Console.SetCursorPosition(X, Y);

            int IntDirection = (int)direction!;
            char DirectionChar = DirectionChars[IntDirection];

            Console.ForegroundColor = ChooseRandomForegroundColour();
            Console.Write(DirectionChar);
            snake.Enqueue((X, Y));
            if (board[X, Y] is Tile.Food)
            {
                (FoodX, FoodY) = PositionFood(width, height, board);
                board[FoodX, FoodY] = Tile.Food;
            }
            else
            {
                (int x, int y) = snake.Dequeue();
                board[x, y] = Tile.Open;
                Console.SetCursorPosition(x, y);
                Console.Write(' ');
            }
            board[X, Y] = Tile.Snake;
            if (Console.KeyAvailable)
            {
                (direction, CloseRequested) = GetDirection(direction, CloseRequested);
            }
            Thread.Sleep(velocity - speedInput * snake.Count);

        }
    }
    catch (Exception e)
    {
        exception = e;
        throw;
    }
    finally
    {
        Console.Clear();
        Console.WriteLine(exception?.ToString() ?? "Snake was closed.");
    }
}
bool IsUserInputValid()
{
    Console.Write(prompt);
    input = Console.ReadLine();
    var IsInt = int.TryParse(input, out speedInput);

    if (IsInt == false || speedInput is < 1 or > 10)
    { return false; }
    else { return true; }
}
(Direction?, bool) GetDirection(Direction? direction, bool closeRequested)
{
    switch (Console.ReadKey(true).Key)
    {
        case ConsoleKey.UpArrow:
            direction = Direction.Up;
            break;
        case ConsoleKey.DownArrow:
            direction = Direction.Down;
            break;
        case ConsoleKey.LeftArrow:
            direction = Direction.Left;
            break;
        case ConsoleKey.RightArrow:
            direction = Direction.Right;
            break;
        case ConsoleKey.Escape:
            closeRequested = true;
            break;
    }
    return (direction, closeRequested);
}

(int, int) PositionFood(int width, int height, Tile[,] board)
{
    List<(int X, int Y)> possibleCoordinates = [];
    for (int i = 0; i < width; i++)
    {
        for (int j = 0; j < height; j++)
        {
            if (board[i, j] is Tile.Open)
            {
                possibleCoordinates.Add((i, j));
            }
        }
    }
    int index = Random.Shared.Next(possibleCoordinates.Count);
    (int X, int Y) = possibleCoordinates[index];
    Console.SetCursorPosition(X, Y);
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Write('+');
    Console.ForegroundColor = ConsoleColor.DarkGreen;
    return (X, Y);
}

ConsoleColor ChooseRandomForegroundColour()
{
    int RandomNum = Random.Shared.Next(15);
    return (ConsoleColor)RandomNum;
}

enum Direction
{
    Up = 0,
    Down = 1,
    Left = 2,
    Right = 3,
}

enum Tile
{
    Open,
    Snake,
    Food
}