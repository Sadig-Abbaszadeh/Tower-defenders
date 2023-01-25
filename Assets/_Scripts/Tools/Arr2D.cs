using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Arr2D<T> : IEnumerable<T>
{
    private T[] _arr;

    public readonly int Width, Height;

    // ctor
    public Arr2D(int width, int height)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentOutOfRangeException("Array width and height must be positive integers");

        this._arr = new T[width * height];
        this.Width = width;
        this.Height = height;
    }

    // indexer
    public T this[int x, int y]
    {
        get => _arr[y * Width + x];
        set => _arr[y * Width + x] = value;
    }

    public bool IsValidCoordinate(int x, int y) => x >= 0 && x < Width
        && y >= 0 && y < Height;

    #region Enumerability Wrapper

    public IEnumerator<T> GetEnumerator() => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private class Enumerator : IEnumerator<T>
    {
        private Arr2D<T> array;
        private int x = -1,
            y = 0;

        public T Current => array[x, y];

        object IEnumerator.Current => array[x, y];

        // ctor
        public Enumerator(Arr2D<T> array)
        {
            this.array = array;
        }

        public bool MoveNext()
        {
            x++;

            if(x >= array.Width)
            {
                x = 0;
                y++;
            }

            return y < array.Height;
        }

        public void Reset()
        {
            x = -1;
            y = 0;
        }

        public void Dispose()
        {
            // dispose of the reference
            array = null;
        }
    }

    #endregion
}