namespace Aotenjo
{
    public class ReducableContainer<T>
    {
        public bool reduced;
        public T target;

        public ReducableContainer(bool reduced, T target)
        {
            this.reduced = reduced;
            this.target = target;
        }
    }
}