using System;
using System.Collections.Generic;
using System.Linq;

namespace GroupDocs.Total.MVC.Products.Search.Service
{
    internal class TermGroup
    {
        private readonly string[] terms;

        public TermGroup(IEnumerable<string> collection)
        {
            terms = collection
                .OrderBy(t => t, StringComparer.Ordinal)
                .ToArray();
        }

        public string[] Terms => terms;

        public int Contains(string term)
        {
            for (int i = 0; i < terms.Length; i++)
            {
                if (string.CompareOrdinal(terms[i], term) == 0) return i;
            }

            return -1;
        }

        public bool Equals(TermGroup other)
        {
            if (terms.Length != other.terms.Length) return false;

            for (int i = 0; i < terms.Length; i++)
            {
                if (string.CompareOrdinal(terms[i], other.terms[i]) != 0) return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TermGroup)) return false;

            return Equals((TermGroup)obj);
        }

        public override int GetHashCode()
        {
            int hashCode = terms[0].GetHashCode();
            for (int i = 1; i < terms.Length; i++)
            {
                hashCode ^= terms[i].GetHashCode();
            }
            return hashCode;
        }
    }
}
