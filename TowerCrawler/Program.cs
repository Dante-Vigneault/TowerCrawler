class Player
{
    public int Health { get; private set; }
    public int Armor { get; private set; }

    public Player(int health, int armor)
    {
        Health = health;
        Armor = armor;
    }

    public void TakeDamage(int amount)
    {
        int damageTaken = Math.Max(amount - Armor, 0);
        Health -= damageTaken;
        Console.WriteLine($"You took {damageTaken} damage. Health is now {Health}.");
    }

    public bool IsDead()
    {
        return Health <= 0;
    }

    public void Heal(int amount)
    {
        Health += amount;
        if (Health > 100) Health = 100;
        Console.WriteLine($"You heal for {amount}. Health is now {Health}.");
    }
}

class Monster
{
    public int Health { get; private set; }

    public Monster(int health)
    {
        Health = health;
    }

    public void TakeDamage(int amount)
    {
        Health -= amount;
        Console.WriteLine($"Monster took {amount} damage. Health is now {Health}.");
    }

    public bool IsDead()
    {
        return Health <= 0;
    }
}

class TowerCrawler
{
    static string[] towerFloors = new string[]
    {
        "Dusty Entrance Hall",
        "Trap Room", //Takes damage
        "Empty Corridor",
        "Healing Fountain", //Heals player
        "Monster Lair" //Extra monster damage
    };

    static int playerFloor = 0;
    static int monsterFloor = 3;

    static Player player = new Player(100, 20);
    static Monster monster = new Monster(100);

    static bool gameRunning = true;

    static void Main()
    {
        Console.WriteLine("Welcome to Tower Crawler!");
        Console.WriteLine("Commands: 'up'/'u', 'down'/'d', 'search'/'s', 'quit'");

        while (gameRunning)
        {
            ShowStatus();

            Console.Write("\nWhat do you want to do? ");
            string input = Console.ReadLine().ToLower();

            switch (input)
            {
                case "u":
                case "up":
                    if (playerFloor < towerFloors.Length - 1)
                    {
                        playerFloor++;
                        Console.WriteLine("You move up a floor.");
                        TriggerFloorEvent();
                    }
                    else Console.WriteLine("You can't go higher.");
                    break;

                case "d":
                case "down":
                    if (playerFloor > 0)
                    {
                        playerFloor--;
                        Console.WriteLine("You move down a floor.");
                        TriggerFloorEvent();
                    }
                    else Console.WriteLine("You can't go lower.");
                    break;

                case "s":
                case "search":
                    if (playerFloor == monsterFloor)
                    {
                        Console.WriteLine("The monster is here! You attack!");
                        monster.TakeDamage(25);

                        if (monster.IsDead())
                        {
                            Console.WriteLine("You defeated the monster! Victory!");
                            gameRunning = false;
                            break;
                        }

                        Console.WriteLine("The monster fights back!");
                        player.TakeDamage(monsterFloor == 4 ? 25 : 15); // Extra damage in monster lair

                        if (player.IsDead())
                        {
                            Console.WriteLine("You were slain by the monster... Game Over.");
                            gameRunning = false;
                        }
                    }
                    else
                    {
                        Console.WriteLine("The monster is not on this floor.");
                    }
                    break;

                case "quit":
                    Console.WriteLine("You have exited the tower.");
                    gameRunning = false;
                    break;

                default:
                    Console.WriteLine("Command not understood. Try 'up', 'down', 'search', or 'quit'.");
                    break;
            }

            MonsterTurn();
        }
    }

    static void ShowStatus()
    {
        Console.WriteLine($"\n--- Status ---");
        Console.WriteLine($"Player: Floor {playerFloor} - {towerFloors[playerFloor]}");
        Console.WriteLine($"  Health: {player.Health} | Armor: {player.Armor}");
        Console.WriteLine($"Monster: Floor {monsterFloor} - {towerFloors[monsterFloor]}");
        Console.WriteLine($"  Health: {monster.Health}");
    }

    static void MonsterTurn()
    {
        if (!gameRunning || monster.IsDead()) return;

        if (monsterFloor < playerFloor)
        {
            monsterFloor++;
            Console.WriteLine("The monster moves up a floor.");
        }
        else if (monsterFloor > playerFloor)
        {
            monsterFloor--;
            Console.WriteLine("The monster moves down a floor.");
        }
    }

    static void TriggerFloorEvent()
    {
        Console.WriteLine($"You arrive at: {towerFloors[playerFloor]}");

        switch (playerFloor)
        {
            case 1: // Trap Room
                player.TakeDamage(10);
                Console.WriteLine("A trap triggers!");
                break;

            case 3: // Healing Fountain
                player.Heal(15);
                break;

            case 4: // Monster Lair
                Console.WriteLine("The air is thick with danger...");
                break;
        }
    }
}