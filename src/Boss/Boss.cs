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

    [NonSerialized] private AttributeEventSubscriptions attributeSubscriptions;

    /// <summary>重写时如需自动订阅，可调用基类实现或 SubscribeAttributeEvents(player)。</summary>
    public virtual void SubscribeToPlayerEvents(Player player) => SubscribeAttributeEvents(player);

    /// <summary>重写时如需自动退订，可调用基类实现或 UnsubscribeAttributeEvents()。</summary>
    public virtual void UnsubscribeFromPlayerEvents(Player player) => UnsubscribeAttributeEvents();

    // 派生类需要替换父类的手动订阅时，可直接调用以下方法。
    protected void SubscribeAttributeEvents(Player player) =>
        (attributeSubscriptions ??= new AttributeEventSubscriptions()).Subscribe(this, player);

    protected void UnsubscribeAttributeEvents() => attributeSubscriptions?.Unsubscribe();

    /// <summary>返回初始目标分数，不开始回合，也不修改玩家状态。</summary>
    public virtual double GetStartingTarget(Player player, double basicTarget) => basicTarget;

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