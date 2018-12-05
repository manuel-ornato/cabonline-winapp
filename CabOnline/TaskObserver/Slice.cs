using System;
using System.Collections.Generic;

namespace CabOnline.TaskObserver
{
    internal struct Slice
    {
        private readonly int _min;
        private readonly int _max;

        public Slice(int min, int max) : this()
        {
            _min = min;
            _max = max;
        }

        public int Max => _max;

        public int Min => _min;
    }
}