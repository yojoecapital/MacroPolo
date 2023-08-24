using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace MacroPolo
{
    public class Node<T>
    {
        public T Value { get; set; }
        public Node<T> Prev { get; set; }
        public Node<T> Next { get; set; }
        public static Node<T> Null { get; } = new Node<T>();
        public override string ToString() => Value.ToString();
    }

    public class Buffer<T>
    {
        public int Count { private set; get; }  = 0;
        public readonly int capacity;
        private readonly Node<T> head = Node<T>.Null;
        private Node<T> tail, cursor;

        public Buffer(int capacity)
        {
            tail = head;
            cursor = head;
            this.capacity = capacity;
        }

        public void Clear()
        {
            Count = 0;
            tail = head;
            cursor = head;
            head.Next = head.Prev = null;
            tail.Next = tail.Prev = null;
            head.Value = default;
        }

        public void Add(T value)
        {
            Node<T> add;
            if (Count++ < capacity)
                add = new Node<T> { Value = value };
            else
            {
                add = tail;
                tail = tail.Next;
                tail.Prev = null;
                add.Value = value;
            }

            var prev = cursor.Prev;
            add.Next = cursor;
            add.Prev = prev;
            if (tail == head) tail = add;
            if (prev != null)
            {
                prev.Next = add;
            }
            cursor.Prev = add;
        }

        public void Remove()
        {
            if (cursor.Prev != null)
            {
                Count--;
                var remove = cursor.Prev;
                if (remove == tail) tail = cursor;
                if (remove.Prev != null)
                {
                    remove.Prev.Next = cursor;
                }
                cursor.Prev = remove.Prev;
            }
        }

        public void MoveLeft()
        {
            if (cursor.Prev != null)
                cursor = cursor.Prev;
        }

        public void MoveRight()
        {
            if (cursor.Next != null)
                cursor = cursor.Next;
        }

        public override string ToString()
        {
            var node = tail;
            var stringBuilder = new StringBuilder();

            while (node != null)
            {
                stringBuilder.Append(node.Value);
                node = node.Next;
            }

            return stringBuilder.ToString().Replace("\0", "");
        }
    }



}