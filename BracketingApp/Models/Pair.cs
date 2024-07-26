namespace BracketingApp.Models
{
    public class Pair<T1, T2>
    {
        public T1 First { get; set; }
        public T2 Second { get; set; }

        public Pair(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }

        public override bool Equals(object obj)
        {
            if (obj is Pair<T1, T2> pair)
            {
                return EqualityComparer<T1>.Default.Equals(First, pair.First) &&
                       EqualityComparer<T2>.Default.Equals(Second, pair.Second);
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hashFirst = First == null ? 0 : First.GetHashCode();
                int hashSecond = Second == null ? 0 : Second.GetHashCode();
                return hashFirst ^ hashSecond;
        }

        public override string ToString()
        {
            return $"Pair: {First}, {Second}";
        }
    }
}

