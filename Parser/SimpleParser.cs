using System;
using System.Collections.Generic;

namespace Parser
{
    public class SimpleParser
    {
        private Token m_startRuleToken;
        private readonly ILexer m_lexer;
        private PQueue<Token> m_sym;
        private Stack<Token> m_consumed;
        private readonly Queue<Error> m_err;
        private Node<string> m_tree;
        private bool m_triedRollback;

        public SimpleParser(ILexer lexer)
        {
            m_startRuleToken = new Token { TokenType = TokenType.NEW_RULE };
            m_tree = new Node<string>();
            m_lexer = lexer;
            m_consumed = new Stack<Token>();
            m_err = new Queue<Error>();
        }

        public Node<string> SyntaxTree
        {
            get { return m_tree; }
        }

        public Queue<Error> Errors
        {
            get { return m_err; }
        }

        /// <summary>
        /// The main method to call when parsing
        /// </summary>
        /// <param name="input"></param>
        public void Parse(string input)
        {
            m_err.Clear();
            m_consumed.Clear();
            m_sym = m_lexer.Tokenize(input);

            // Should be stmt (top symbol) later on
            Stmt();
            //Expr()

            if (m_sym.Count > 0)
                m_err.Enqueue(new Error { Message = "Syntax Error - Unmatched tokens", Type = ErrorType.SyntaxError });

            // Temp - This should be done outside of this class
            if (m_err.Count > 0)
            {
                Console.WriteLine("Error List:");
                Console.WriteLine("-----------");

                foreach (var error in m_err)
                {
                    Console.WriteLine(error);
                }
            }
        }

        /// <summary>
        /// Consume the current input token and iterate to the next symbol in the input stream
        /// </summary>
        /// <param name="t">The terminal to accept</param>
        /// <returns></returns>
        public bool ConsumeNext(TokenType tokenType)
        {
            if (m_sym.Count > 0 && tokenType == m_sym.Peek().TokenType)
            {
                MoveNext();
                return true; // Accepted
            }

            return false; // Not accepted
        }

        /// <summary>
        /// Move to the next token in the input stream
        /// </summary>
        private void MoveNext()
        {
            var next = m_sym.Dequeue();

            var newNode = new Node<string>(next.TokenType.ToString());
            
            if (next.Value != null)
                newNode.Children.Add(new Node<string>(next.Value.ToString()));
            
            m_tree.Children.Add(newNode);
            m_consumed.Push(next);
        }

        private void AddTopNode(string nodeName)
        {
            if (m_tree.Value == null)
            {
                m_tree.Value = nodeName;
                return;
            }

            // Demote current nodes one step
            Node<string> tempNode = m_tree;
            m_tree = new Node<string>();
            m_tree.Children.Add(tempNode);
            m_tree.Value = nodeName;
        }

        /* GRAMMAR IMPLEMENTATION */

        public void Stmt()
        {
            if (RuleStartsWith(TokenType.IF).FollowedBy(TokenType.LPAR).FollowedBy(Expr).FollowedBy(TokenType.RPAR).FollowedBy(TokenType.LBRA).FollowedBy(Stmt).FollowedBy(TokenType.RBRA).NoFailureReported())
            {
                // Add to AST
                AddTopNode("Stmt");
            }
            else if (RuleStartsWith(TokenType.VAR).FollowedBy(TokenType.NAME).FollowedBy(TokenType.ASSIGN).FollowedBy(TokenType.VALUE).FollowedBy(TokenType.SEMI).NoFailureReported())
            {
                // Add to AST
                AddTopNode("Stmt");
            }
            else if (RuleStartsWith(Expr).FollowedBy(TokenType.SEMI).NoFailureReported())
            {
                // Add to AST
                AddTopNode("Stmt");
            }
            else
            {
                m_err.Enqueue(new Error { Message = "Syntax Error - Invalid Statement", Type = ErrorType.SyntaxError });
            }
        }

        public void Expr()
        {
            // FUNC LPAR VALUE RPAR SEMI
            if (RuleStartsWith(TokenType.NAME).FollowedBy(TokenType.LPAR).FollowedBy(TokenType.VALUE).FollowedBy(TokenType.RPAR).NoFailureReported())
            {
                // Add to AST
                AddTopNode("Expr");
            }
            else if (RuleStartsWith(TokenType.NAME).FollowedBy(TokenType.EQ).FollowedBy(TokenType.VALUE).NoFailureReported())
            {
                // Add to AST
                AddTopNode("Expr");
            }
            else
            {
                m_err.Enqueue(new Error { Message = "Syntax Error - Invalid Expression", Type = ErrorType.SyntaxError});
            }
        }

        private bool NoFailureReported()
        {
            var status = (m_err.Count == 0 && !m_triedRollback);
            m_triedRollback = false;
            return status;
        }

        private void TryRollBack()
        {
            m_triedRollback = true;

            while (m_consumed.Count > 0 && m_consumed.Peek() != m_startRuleToken)
            {
                m_sym.EnqueueBeginning(m_consumed.Pop());
            }

            // Pop the "new rule" token
            if (m_consumed.Count > 0)
                m_consumed.Pop();
        }

        private SimpleParser RuleStartsWith(TokenType type)
        {
            m_consumed.Push(m_startRuleToken);

            if (!ConsumeNext(type))
                TryRollBack();

            return this;
        }

        private SimpleParser RuleStartsWith(Action grammar)
        {
            grammar();
            return this;
        }

        private SimpleParser FollowedBy(TokenType type)
        {
            if (m_err.Count > 0 || m_triedRollback)
                return this;

            if (!ConsumeNext(type))
                TryRollBack();

                //m_err.Enqueue(new Error {Message = "Unexpected symbol.", Type = ErrorType.UnexpectedSymbol});

            return this;
        }

        private SimpleParser FollowedBy(Action grammar)
        {
            if (m_err.Count > 0 || m_triedRollback)
                return this;

            grammar();
            return this;
        }
    }
}