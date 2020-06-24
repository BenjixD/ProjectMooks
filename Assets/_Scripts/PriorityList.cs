using System.Collections;
using System.Collections.Generic;
using System;


public class PriorityList<T> : IEnumerable, IEnumerator where T : IComparable<T> {
    private List<T> data;
    private int position = -1;

    public PriorityList() {
        this.data = new List<T>();
    }

    public PriorityList(PriorityList<T> priorityList) {
        this.data = priorityList.data;
    }

    public void Add(T item) {
        int index = 0;

        while (index < data.Count && item.CompareTo(data[index]) >= 0 )  {
            index++;
        }

        this.data.Insert( index, item );
    }

    public T GetAt(int i) {
        return this.data[i];
    }

    public int Count() {
        return this.data.Count;
    }

    public void Remove(T item) {
        this.data.Remove(item);
    }

    public void RemoveAt(int i) {
        this.data.RemoveAt(i);
    }

    public void Clear() {
        this.data.Clear();
    }


    // IEnumerator stuff ====
    public object Current
    {
        get { return data[position];}
    }
    public IEnumerator GetEnumerator()
    {
        return new PriorityList<T>(this);
    }

    public bool MoveNext()
    {
        position++;
        return (position < data.Count);
    }

    public void Reset()
    {
        this.position = -1;
    }
    
}

/*
// Test program
public class A : IComparable<A> {
    public int value;
    
    public A(int val) {
        this.value = val;
    }
    
    public int CompareTo(A other) {
        if (this.value < other.value) return -1;
        else if (this.value > other.value) return 1;
        else return 0;
    }
}

class Program
{
    static void Main()
    {
        PriorityList<A> pq = new PriorityList<A>();
        pq.Add(new A(2));
        pq.Add(new A(1));
        pq.Add(new A(4));
        pq.Add(new A(3));
        
        foreach (A a in pq) {
            Console.WriteLine("" + a.value);
        }

    }
}
*/