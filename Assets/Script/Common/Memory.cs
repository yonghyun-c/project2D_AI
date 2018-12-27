using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class Memory<T>
{
    private readonly int SIZE;
    private readonly T[] memories;

    private int start;
    private int idx;

    public static Memory<T> boom(int size, T defaultValue)
    {
        Memory<T> memory = new Memory<T>(size);

        for (int i=0; i<size; i++)
        {
            memory.Memorize(defaultValue);
        }

        return memory;
    }

    private Memory(int size)
    {
        start = -1;
        this.SIZE = size;
        memories = new T[SIZE];
        idx = 0;
    }

    private void IncreaseIdx()
    {
        if (start == idx)
        {
            start = Next(start);
        }

        if (start == -1)
        {
            start = 0;
        }

        idx = Next(idx);
    }

    private int Next(int item)
    {
        return (item + 1) % SIZE;
    }

    private int Prev(int item)
    {
        return (item + SIZE - 1) % SIZE;
    }

    public void Memorize(T item)
    {
        memories[idx] = item;
        IncreaseIdx();
    }

    public bool HasMemory()
    {
        return start != -1 && start < idx;
    }

    public T Remember()
    {
        return memories[Prev(idx)];
    }

    public T Remember(int before)
    {
        if ((idx - start + SIZE) % SIZE > before)
        {
            throw new Exception("NOT EXIST MEMORY ACCESS");
        }

        int targetIdx = (idx + SIZE - before) % SIZE;
        return memories[targetIdx];
    }

    public void Erase()
    {
        idx = Prev(idx);
    }
}