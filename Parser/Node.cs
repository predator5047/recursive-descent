using System.Collections.Generic;

namespace Parser
{
    public class Node<T>
    {
        public Node()
        {
        }

        public Node(T value)
        {
            Value = value;
        }

        public Node<T> AddChild(T value)
        {
            var node = new Node<T>(value);
            m_children.Add(node);
            return node;
        }

        private List<Node<T>> m_children = new List<Node<T>>();
 
        public T Value { get; set; }
        public List<Node<T>> Children { get { return m_children; } }
    }
}