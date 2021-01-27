using System;
using System.Collections.Generic;

namespace WorkloadSimulation
{
    public class TextRandomizer
    {
        Random _random;
        List<char> _randomCharsToUse = new List<char> { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        public TextRandomizer(int seed)
        {
            _random = new Random(seed);
        }


        private string GenerateRandomString(int length)
        { 
            
            string result = "";
            for (var i = 0; i < length; ++i)
            {
                result += _randomCharsToUse[_random.Next(_randomCharsToUse.Count)];
            }
            return result;
        }


        public List<string> CreateRandomStrings(int length, int count)
        {
            var result = new List<string>();
            for(var i = 0; i < count; ++i)
            {
                result.Add(GenerateRandomString(length));
            }
            return result;
        }
    }
}
