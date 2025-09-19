using Aotenjo;

public abstract class CountingArtifact : Artifact, ICountable
{
    public readonly int maxCounter;
    public int currentCounter;

    public CountingArtifact(int maxCounter, string name, Rarity rarity) : base(name, rarity)
    {
        this.maxCounter = maxCounter;
        currentCounter = maxCounter - 1;
    }

    public override void ResetArtifactState()
    {
        base.ResetArtifactState();
        currentCounter = maxCounter - 1;
    }

    public override string Serialize()
    {
        return currentCounter.ToString();
    }

    public override void Deserialize(string str)
    {
        currentCounter = int.Parse(str);
    }

    public virtual int GetMaxCounter()
    {
        return maxCounter;
    }

    public virtual int GetCurrentCounter()
    {
        return currentCounter;
    }
}