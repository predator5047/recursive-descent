using System.Collections.Generic;

namespace TreeVisualizer
{
    public static class StringExtensions
    {
        public static IEnumerable<int> AllIndicesOf(this string input, string search)
        {
            int position, offset = 0;
            
            while ((position = input.IndexOf(search)) >= 0) 
            {
                input = input.Substring(position + search.Length);
                offset += position;

                yield return offset;
            }
        }
    }
}