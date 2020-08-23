using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TicTacToe
{
    class Array2D<T> : IEnumerable<T>
    {
        readonly T[] data;
        public readonly uint Width;
        public readonly uint Height;

        public Array2D(uint width, uint height, T filler = default)
        {
            data = new T[width * height];
            Array.Fill(data, filler);
            Width = width;
            Height = height;
        }

        public T this[int x, int y]
        {
            get => data[x + y * Width];
            set => data[x + y * Width] = value;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T obj in data) yield return obj;
        }

        IEnumerator IEnumerable.GetEnumerator() => data.GetEnumerator();
    }
}
