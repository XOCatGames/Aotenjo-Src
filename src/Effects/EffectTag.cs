namespace Aotenjo
{
    public class EffectTag
    {
        private string tag;
        
        private EffectTag(string tag)
        {
            this.tag = tag;
        }
        
        public static EffectTag Of(string tag)
        {
            return new EffectTag(tag);
        }

        public override bool Equals(object obj)
        {
            return obj is EffectTag other && other.tag.Equals(tag);
        }
        
        public override int GetHashCode()
        {
            return tag.GetHashCode() + 1;
        }

        public static readonly EffectTag CyclingSuit = new EffectTag("lunhuanhuase");
    }
}