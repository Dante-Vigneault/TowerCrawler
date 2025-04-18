using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

class Player
{
    public string ClassName { get; private set; }
    public int Health { get; private set; }
    public int Armor { get; private set; }
    public int AttackBonus { get; private set; }

    public Player(string className, int health, int armor, int attackBonus)
    {
        ClassName = className;
        Health = health;
        Armor = armor;
        AttackBonus = attackBonus;
    }

    public void TakeDamage(int amount)
    {
        int reduced = Math.Max(amount - Armor, 0);
        Health -= reduced;

        if (Armor > 0)
            Console.WriteLine($"Your armor absorbs part of the blow! (-{Armor} damage)");

        Console.WriteLine($"{ClassName} takes {reduced} damage. Health is now {Health}.");
    }

    public void Heal(int amount)
    {
        Health = Math.Min(Health + amount, 100);
        Console.WriteLine($"{ClassName} heals for {amount}. Health is now {Health}.");
    }

    public void AddArmor(int amount, int maxArmor)
    {
        int previous = Armor;
        Armor = Math.Min(Armor + amount, maxArmor);
        int gained = Armor - previous;

        if (gained > 0)
            Console.WriteLine($"You found armor scraps! Armor +{gained} (Now: {Armor}/{maxArmor})");
        else
            Console.WriteLine($"You're already at max armor ({maxArmor}).");
    }

    public void ModifyArmor(int amount, int maxArmor)
    {
        Armor = Math.Clamp(Armor + amount, 0, maxArmor);
        Console.WriteLine($"Armor modified by {amount}. New value: {Armor}/{maxArmor}.");
    }

    public void ModifyAttack(int amount)
    {
        AttackBonus += amount;
        Console.WriteLine($"Attack modified by {amount}. New bonus: {AttackBonus}.");
    }

    public bool IsDead() => Health <= 0;
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

    public bool IsDead() => Health <= 0;
}

class TowerCrawler
{
    static Random rng = new Random();

    static void Main()
    {
        // Difficulty selection
        string difficulty = "";
        int totalFloors = 0, numberOfMonsters = 0, maxArmor = 0;
        int monsterMinDamage = 0, monsterMaxDamage = 0;

        Console.WriteLine("Choose your difficulty:");
        Console.WriteLine("1. Easy\n2. Medium\n3. Hard");

        while (true)
        {
            Console.Write("Enter 1, 2, or 3: ");
            string input = Console.ReadLine();

            if (input == "1") { difficulty = "easy"; totalFloors = 5; numberOfMonsters = 1; maxArmor = 6; monsterMinDamage = 5; monsterMaxDamage = 10; break; }
            else if (input == "2") { difficulty = "medium"; totalFloors = 7; numberOfMonsters = 2; maxArmor = 8; monsterMinDamage = 10; monsterMaxDamage = 15; break; }
            else if (input == "3") { difficulty = "hard"; totalFloors = 10; numberOfMonsters = 3; maxArmor = 10; monsterMinDamage = 15; monsterMaxDamage = 20; break; }
            else Console.WriteLine("Invalid input.");
        }

        // Path selection
        string playerPath = "";
        int baseHealth = 100, baseArmor = 0, attackBonus = 0;

        Console.WriteLine("Choose your path:\n1. Warrior\n2. Berserker\n3. Paladin");
        while (true)
        {
            Console.Write("Enter 1, 2, or 3: ");
            string choice = Console.ReadLine();

            if (choice == "1") { playerPath = "Warrior"; baseArmor = 2; attackBonus = 2; break; }
            else if (choice == "2") { playerPath = "Berserker"; baseArmor = 0; attackBonus = 5; break; }
            else if (choice == "3") { playerPath = "Paladin"; baseArmor = 4; attackBonus = 0; break; }
            else Console.WriteLine("Invalid choice.");
        }

        Player player = new Player(playerPath, baseHealth, baseArmor, attackBonus);
        Monster monster = new Monster(100);

        List<string> towerFloors = new List<string>();
        Dictionary<int, string> floorEvents = new Dictionary<int, string>();

        for (int i = 0; i < totalFloors; i++)
        {
            towerFloors.Add($"Floor {i}");
            floorEvents[i] = GetRandomEvent();
        }

        int playerFloor = 0;
        int monsterFloor = rng.Next(1, totalFloors);
        floorEvents[monsterFloor] = "monster_lair";
        bool monsterRevealed = false;
        bool gameRunning = true;

        Console.WriteLine("\nGame Start!");

        while (gameRunning)
        {
            ShowStatus(player, monster, playerFloor, monsterFloor, monsterRevealed);
            Console.Write("\nYour action: ");
            string action = Console.ReadLine().ToLower();

            HandlePlayerInput(action, ref playerFloor, ref monsterRevealed, player, maxArmor, monster, monsterFloor, floorEvents);
            HandleMonsterTurn(ref monsterFloor, playerFloor, ref monsterRevealed, player, monster, monsterMinDamage, monsterMaxDamage);
            CheckProximityWarning(playerFloor, monsterFloor);

            if (player.IsDead()) { Console.WriteLine($"\n{player.ClassName} was slain by the monster. Game Over."); gameRunning = false; }
            else if (monster.IsDead()) { Console.WriteLine($"\n{player.ClassName} defeated the monster! Victory!"); gameRunning = false; }
        }
    }

    static string GetRandomEvent()
    {
        string[] events = { "trap", "heal", "armor_scrap", "cursed_item", "health_shop", "flavor_text", "nothing" };
        return events[rng.Next(events.Length)];
    }

    static void HandlePlayerInput(string input, ref int playerFloor, ref bool monsterRevealed, Player player, int maxArmor, Monster monster, int monsterFloor, Dictionary<int, string> floorEvents)
    {
        switch (input)
        {
            case "u":
            case "up":
                playerFloor = Math.Min(playerFloor + 1, floorEvents.Count - 1);
                monsterRevealed = false;
                Console.WriteLine("You move up a floor.");
                TriggerFloorEvent(playerFloor, player, maxArmor, floorEvents);
                break;
            case "d":
            case "down":
                playerFloor = Math.Max(playerFloor - 1, 0);
                monsterRevealed = false;
                Console.WriteLine("You move down a floor.");
                TriggerFloorEvent(playerFloor, player, maxArmor, floorEvents);
                break;
            case "s":
            case "search":
                if (playerFloor == monsterFloor) { monsterRevealed = true; Console.WriteLine("The monster is here!"); }
                else Console.WriteLine("Nothing found.");
                break;
            case "a":
            case "attack":
                if (playerFloor == monsterFloor && monsterRevealed)
                {
                    if (rng.Next(100) < 70)
                    {
                        int damage = rng.Next(10, 21) + player.AttackBonus;
                        Console.WriteLine($"{player.ClassName} attacks for {damage}!");
                        monster.TakeDamage(damage);
                    }
                    else Console.WriteLine($"{player.ClassName} missed!");
                }
                else Console.WriteLine($"{player.ClassName} can't attack right now.");
                break;
            case "quit":
                Console.WriteLine($"{player.ClassName} exits the tower.");
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Unknown command.");
                break;
        }
    }

    static void HandleMonsterTurn(ref int monsterFloor, int playerFloor, ref bool monsterRevealed, Player player, Monster monster, int minDmg, int maxDmg)
    {
        if (monster.IsDead()) return;

        bool sameFloor = monsterFloor == playerFloor;
        int roll = rng.Next(100);

        if (sameFloor && monsterRevealed && roll < 30)
        {
            Console.WriteLine("The monster fades into the shadows.");
            monsterRevealed = false;
        }
        else if (sameFloor && roll < 70)
        {
            if (rng.Next(100) < 60)
            {
                int damage = rng.Next(minDmg, maxDmg + 1);
                Console.WriteLine("The monster attacks!");
                player.TakeDamage(damage);
            }
            else Console.WriteLine("The monster misses!");
        }
        else if (roll < 90)
        {
            int direction = rng.Next(2) == 0 ? -1 : 1;
            monsterFloor = Math.Clamp(monsterFloor + direction, 0, 9);
            monsterRevealed = false;
            Console.WriteLine("You hear footsteps echo elsewhere...");
        }
        else Console.WriteLine("You sense the monster waiting...");
    }

    static void TriggerFloorEvent(int playerFloor, Player player, int maxArmor, Dictionary<int, string> floorEvents)
    {
        string eventType = floorEvents[playerFloor];
        Console.WriteLine($"Event: { eventType}");

        switch (eventType)
        {
            case "trap": player.TakeDamage(10);
                break;
            case "heal": player.Heal(15);
                break;
            case "armor_scrap": player.AddArmor(1, maxArmor);
                break;
            case "cursed_item":
                Console.WriteLine("You find two cursed items. Choose one:");
                Console.WriteLine("1. +3 Armor / -3 Damage");
                Console.WriteLine("2. + 3 Damage / -3 Armor");
                while (true)
                {
                    string choice = Console.ReadLine();
                    if (choice == "1") { player.ModifyArmor(3, maxArmor); player.ModifyAttack(-3); break; }
                    else if (choice == "2") { player.ModifyAttack(3); player.ModifyArmor(-3, maxArmor); break; }
                    else Console.WriteLine("Choose 1 or 2.");
                }
                break;
            case "health_shop":
                Console.WriteLine("A figure offers a deal: 10 HP for a stat boost. Accept? (y/n)");
                string deal = Console.ReadLine();
                if (deal == "y" && player.Health > 10)
                {
                    player.TakeDamage(10);
                    if (rng.Next(2) == 0) player.ModifyArmor(1, maxArmor);
                    else player.ModifyAttack(2);
                }
                break;
            case "flavor_text":
                Console.WriteLine("The floor glows faintly with ancient runes.");
                break;
            case "monster_lair":
                Console.WriteLine("You feel the monster's power in this lair...");
                break;
            default:
                Console.WriteLine("Nothing happens.");
                break;
        }
    }

    static void CheckProximityWarning(int playerFloor, int monsterFloor)
    {
        if (Math.Abs(playerFloor - monsterFloor) == 1)
        {
            Console.WriteLine("You feel a chill... the monster is near.");
        }
    }

    static void ShowStatus(Player player, Monster monster, int playerFloor, int monsterFloor, bool monsterRevealed)
    {
        Console.WriteLine($"---{ player.ClassName} Status-- - ");
        Console.WriteLine($"Floor: {playerFloor} | Health: {player.Health} | Armor: {player.Armor} | Bonus DMG: {player.AttackBonus}");
        Console.WriteLine($"Monster: {(monsterRevealed ? $"Floor {monsterFloor}" : "???")} | Health: {(monsterRevealed ? monster.Health.ToString() : "??")}");
        Console.WriteLine("Controls: [u]p, [d]own, [s]earch, [a]ttack, [quit]");
    }
}
