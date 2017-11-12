using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public class PositionArray<T>
    {
        private List<T> data;
        private int width;
        private int height;

        public List<T> Data { get; }

        public int Width { get; }
        public int Height { get; }

        public T GetValue(int x, int y)
        {
            return data[y * width + x];
        }

        public void SetValue(int x, int y, T value)
        {
            data[y * width + x] = value;
        }

        public PositionArray(int arraySize, int width, int height)
        {
            data = new List<T>(arraySize);
            this.width = width;
            this.height = height;
        }

        public PositionArray(List<T> source, int width, int height)
        {
            data = source;
            this.width = width;
            this.height = height;
        }

    }

}