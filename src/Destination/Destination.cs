using System;
using Aotenjo;

public abstract class Destination
{
    public string name;
    public bool onSale;
    public Player player;
    public string eventName;


    public Destination(string name, bool onSale, Player player, string eventName = "")
    {
        this.name = name;
        this.onSale = onSale;
        this.player = player;
        this.eventName = eventName;
    }

    public virtual string GetName(Func<string, string> localize)
    {
        return localize($"destination_{name}_name");
    }

    public virtual string GetDescription(Func<string, string> localize)
    {
        return localize($"destination_{name}_description");
    }

    public virtual string GetSaleDescription(Func<string, string> localize)
    {
        if (eventName != "")
        {
            return localize($"event_{name}_{eventName}_description");
        }

        return localize($"destination_{name}_sale_description");
    }

    public virtual string GetEventName()
    {
        return "none";
    }

    public virtual bool IsOnEvent()
    {
        return onSale || eventName != "";
    }

    public virtual void SetOnSale()
    {
        onSale = true;
    }

    public abstract Destination GetRandomRedEventVariant(Player player);

    public enum DestinationEventType
    {
        COMMON,
        YELLOW,
        RED
    }
}