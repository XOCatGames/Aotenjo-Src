using Aotenjo;

public interface IActivable
{
    public bool IsActivating();

    public bool IsActivating(Player player)
    {
        return IsActivating();
    }
}