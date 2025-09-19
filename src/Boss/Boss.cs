using System;
using Aotenjo;

[Serializable]
public abstract class Boss
{
    public string name;

    public Boss(string name)
    {
        this.name = name;
    }

    public abstract void SubscribeToPlayerEvents(Player player);
    public abstract void UnsubscribeFromPlayerEvents(Player player);

    public virtual string GetName(Player player, Func<string, string> loc)
    {
        var type = GetType();
        string locName = loc("boss_" + name + "_name");
        if (type.GetCustomAttributes(typeof(HarderBossAttribute), true).Length > 0)
        {
            return string.Format(loc("boss_harder_name_format"), locName);
        }

        return locName;
    }
    
    public virtual string GetHarderName(Player player, Func<string, string> loc)
    {
        return loc("boss_" + name + "_harder_name");
    }

    public virtual string GetDescription(Player player, Func<string, string> loc)
    {
        var type = GetType();
        if (type.GetCustomAttributes(typeof(HarderBossAttribute), true).Length > 0)
        {
            return GetHarderDescription(player, loc);
        }
        return loc("boss_" + name + "_description");
    }
    
    public virtual string GetHarderDescription(Player player, Func<string, string> loc)
    {
        return loc("boss_" + name + "_harder_description");
    }

    public abstract Artifact GetReversedArtifact(Artifact baseArtifact);

    public virtual Boss GetHarderBoss()
    {
        return this;
    }
}