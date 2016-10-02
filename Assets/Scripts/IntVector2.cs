namespace Assets.Scripts
{
    public class IntVector2
    {

        public int x;
        public int y;

        public IntVector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;                
            }

            if (obj is IntVector2 == false)
            {
                return false;                
            }

            // Compare
            var compared = obj as IntVector2;

            return x == compared.x && y == compared.y;
        }

        public static IntVector2 operator +(IntVector2 iv1, IntVector2 iv2)
        {
            return new IntVector2(iv1.x + iv2.x, iv1.y + iv2.y);
        }

        public static IntVector2 operator -(IntVector2 iv1, IntVector2 iv2)
        {
            return new IntVector2(iv1.x - iv2.x, iv1.y - iv2.y);
        }

        public static IntVector2 operator /(IntVector2 iv1, int iv)
        {
            return new IntVector2(iv1.x / iv, iv1.y / iv);
        }

        public override int GetHashCode()
        {
            return x ^ y;
        }

        public override string ToString()
        {
            return "[" + x + "," + y + "]";
        }
    }
}
