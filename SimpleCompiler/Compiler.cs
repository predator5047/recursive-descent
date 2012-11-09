using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser;

namespace SimpleCompiler
{
    public class Compiler
    {
        private Stack<string> m_leaves = new Stack<string>();

        public void Compile(Node<string> input)
        {
            TransformToStack(input);
        }

        private void TransformToStack(Node<string> input)
        {
            FetchLeaves(input);
            m_leaves = new Stack<string>(m_leaves);
        }

        private void FetchLeaves(Node<string> input)
        {
            if (input.Children.Count != 0)
            {
                foreach (var child in input.Children)
                {
                    TransformToStack(child);
                }
            }
            else
            {
                m_leaves.Push(input.Value);
            }
        }
    }
}
