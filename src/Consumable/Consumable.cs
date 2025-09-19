namespace Aotenjo
{
    public class Consumable
    {
        public string name;
        public string description;

        public Consumable(string name, string description)
        {
            this.name = name;
            this.description = description;
        }

        public override string ToString()
        {
            return GetName();
        }

        public virtual string GetName()
        {
            return name;
        }
    }
}