namespace Creational;
public class Prototype
{
    public static void Main()
    {
        Weapon basicSword = new Weapon("Sword", 10);
        Warrior prototypeWarrior = new Warrior("PrototypeWarrior", 100, basicSword);

        PrototypeRegistry registry = new PrototypeRegistry();
        registry.Register("StandardWarrior", prototypeWarrior);

        Warrior clone1 = registry.GetClone("StandardWarrior");
        clone1.Name = "CloneOne";
        clone1.Equipment.Type = "Axe";

        Warrior clone2 = registry.GetClone("StandardWarrior");
        clone2.Name = "CloneTwo";
        clone2.Equipment.Type = "Bow";

        Console.WriteLine(prototypeWarrior);
        Console.WriteLine(clone1);
        Console.WriteLine(clone2);
    }

    interface IPrototype<T>
    {
        T Clone();
    }

    class Warrior : IPrototype<Warrior>
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public Weapon Equipment { get; set; }
        public Warrior(string name, int level, Weapon equipment)
        {
            Name = name;
            Level = level;
            Equipment = equipment;
        }
        public Warrior Clone()
        {
            var clone = (Warrior)this.MemberwiseClone();

            clone.Equipment = this.Equipment.Clone();

            return clone;
        }
    }

    class Weapon : IPrototype<Weapon>
    {
        public string Type { get; set; }
        public int Damage { get; set; }

        public Weapon(string type, int damage)
        {
            Type = type;
            Damage = damage;
        }
        public Weapon Clone()
        {
            return (Weapon)this.MemberwiseClone();
        }
    }

    class PrototypeRegistry
    {
        private readonly Dictionary<string, IPrototype<Warrior>> _prototypes = new();
        public void Register(string key, IPrototype<Warrior> prototype)
        {
            _prototypes[key] = prototype;
        }
        public Warrior GetClone(string key)
        {
            if (_prototypes.TryGetValue(key, out IPrototype<Warrior>? value))
            {
                return value.Clone();
            }
            throw new ArgumentException("Prototype not found");
        }
    }
}
