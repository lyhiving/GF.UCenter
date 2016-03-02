using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common.Models
{
    [Serializable]
    public class Sex
    {
        private readonly char code;
        private static readonly char[] validCodes = new char[] { 'f', 'F', 'm', 'M', 'u', 'U' };

        private Sex(char code)
        {
            if (!validCodes.Any(c => code == c))
            {
                throw new InvalidCastException("Code is invalid: {0}".FormatInvariant(code));
            }

            this.code = char.ToUpper(code);
        }

        public static Sex Female = new Sex('F');
        public static Sex Male = new Sex('M');
        public static Sex Unknown = new Sex('U');

        public override string ToString()
        {
            return this.code.ToString();
        }

        public static implicit operator Sex(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new InvalidCastException("Code cannot be empty.");
            }
            else if (code.Length > 1)
            {
                throw new InvalidCastException("Code is to long: {0}".FormatInvariant(code));
            }

            return new Sex(code.First());
        }

        public static Sex FromString(string code)
        {
            return new Sex(code.First());
        }

        public static implicit operator string(Sex sex)
        {
            return sex.code.ToString();
        }

        public static implicit operator char(Sex sex)
        {
            return sex.code;
        }

        public static implicit operator Sex(char code)
        {
            return new Sex(code);
        }

        public static bool operator ==(Sex s1, Sex s2)
        {
            return s1.code == s2.code;
        }

        public static bool operator !=(Sex s1, Sex s2)
        {
            return s1.code != s2.code;
        }

        public override bool Equals(object obj)
        {
            if (obj is Sex)
            {
                return this.code == (obj as Sex).code;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ this.code;
        }
    }
}
