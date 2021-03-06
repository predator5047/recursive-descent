﻿using System;
using System.Collections.Generic;
using System.Reflection;

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

        private Func<string, Node<string>> m_rulePreHook;
        private Action<Node<string>>  m_rulePostHook;
        private bool m_unmatchedFlag = false;

        public SimpleParser(ILexer lexer)
        {
            m_startRuleToken = new Token { TokenType = TokenType.NEW_RULE };
            m_tree = new Node<string>();
            m_lexer = lexer;
            m_consumed = new Stack<Token>();
            m_err = new Queue<Error>();

            // Setup rule pre-calling conditions
            m_rulePreHook = name =>
                                {
                                    if (m_tree.Value == null)
                                    {
                                        m_tree.Value = name;
                                        return m_tree;
                                    }
                                    
                                    var tempNode = m_tree;
                                    var newTopNode = m_tree.AddChild(name);
                                    m_tree = newTopNode;
                                    return tempNode;
                                };

            // Setup rule post-calling conditions
            m_rulePostHook = node => m_tree = node;
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

            try
            {
                m_sym = m_lexer.Tokenize(input);
            }
            catch (LexerException ex)
            {
                throw new ParserException("Parsing failed (Lexer)", ex);
            }

            // Stmt == Terminating rule
            //Stmt();
            Prg();

            // If we still have symbols in the stream, parsing failed
            if (m_sym.Count > 0)
                throw new ParserException("Parsing failed (Unmatched tokens in stream)");

                //m_err.Enqueue(new Error { Message = "Syntax Error - Unmatched tokens", Type = ErrorType.SyntaxError });

            // If parsing failed, reset the tree
            if (m_err.Count > 0)
                m_tree = null;
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

        public void Prg()
        {
            var tempNode = m_rulePreHook(MethodBase.GetCurrentMethod().Name);

            if (!OneOrMore(Stmt).AndWasMatched())
            {
                throw new ParserException("Parsing failed (Prg)");
            }

            m_rulePostHook(tempNode);
        }

        public void Stmt()
        {
            // Removed/Refactored grammar:
            //RuleStartsWith(TokenType.VAR).FollowedBy(TokenType.NAME).FollowedBy(TokenType.ASSIGN).FollowedBy(TokenType.VALUE).FollowedBy(TokenType.SEMI).AndWasMatched() ||

            var tempNode = m_rulePreHook(MethodBase.GetCurrentMethod().Name);

            if (!(
                RuleStartsWith(TokenType.NAME).FollowedBy(TokenType.ASSIGN).FollowedBy(Expr).FollowedBy(TokenType.SEMI).AndWasMatched() ||
                RuleStartsWith(TokenType.IF).FollowedBy(TokenType.LPAR).FollowedBy(Expr).FollowedBy(TokenType.RPAR).FollowedBy(TokenType.LBRA).OneOrMore(Stmt).FollowedBy(TokenType.RBRA).AndWasMatched() || 
                RuleStartsWith(TokenType.VAR).FollowedBy(TokenType.NAME).FollowedBy(TokenType.ASSIGN).FollowedBy(Expr).FollowedBy(TokenType.SEMI).AndWasMatched() ||
                RuleStartsWith(Expr).FollowedBy(TokenType.SEMI).AndWasMatched()))
            {
                m_unmatchedFlag = true;
                //throw new ParserException("Parsing failed (Stmt)");
            }

            m_rulePostHook(tempNode);
        }

        public void Expr()
        {
            var tempNode = m_rulePreHook(MethodBase.GetCurrentMethod().Name);

            if (!(
                //RuleStartsWith(TokenType.NAME).FollowedBy(TokenType.OPER).FollowedBy(TokenType.NAME).AndWasMatched() ||
                RuleStartsWith(TokenType.NAME).FollowedBy(TokenType.LPAR).FollowedBy(TokenType.VALUE).FollowedBy(TokenType.RPAR).AndWasMatched() ||
                RuleStartsWith(TokenType.NAME).FollowedBy(TokenType.LPAR).FollowedBy(TokenType.NAME).FollowedBy(TokenType.RPAR).AndWasMatched() ||
                RuleStartsWith(TokenType.NAME).FollowedBy(TokenType.EQ).FollowedBy(TokenType.VALUE).AndWasMatched() ||
                RuleStartsWith(TokenType.NAME).FollowedBy(TokenType.EQ).FollowedBy(TokenType.NAME).AndWasMatched() ||
                RuleStartsWith(TokenType.NAME).FollowedBy(TokenType.OPER).FollowedBy(Expr).AndWasMatched() ||
                RuleStartsWith(TokenType.VALUE).FollowedBy(TokenType.OPER).FollowedBy(Expr).AndWasMatched() ||
                RuleStartsWith(TokenType.VALUE).AndWasMatched() ||
                RuleStartsWith(TokenType.NAME).AndWasMatched()
                ))
            {
                m_unmatchedFlag = true;
                //throw new ParserException("Parsing failed (Expr)");
            }

            m_rulePostHook(tempNode);
        }

        private bool AndHadMore()
        {
            var status = !m_unmatchedFlag;
            m_unmatchedFlag = false;
            return status;
        }

        private bool AndWasMatched()
        {
            var status = (!m_triedRollback);
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

            // Reset the AST node to NOT include anything rolled back
            m_tree.Children.Clear();
        }

        private SimpleParser OneOrMore(Action grammar)
        {
            if (m_triedRollback)
                return this;

            while (m_sym.Count > 0 && FollowedBy(grammar).AndHadMore()) {}

            //if (!ConsumeNext(type))
            //    TryRollBack();

            return this;
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
            m_consumed.Push(m_startRuleToken);

            if (m_triedRollback)
                return this;

            grammar();
            return this;
        }

        private SimpleParser FollowedBy(TokenType type)
        {
            if (m_triedRollback)
                return this;

            if (!ConsumeNext(type))
                TryRollBack();

             return this;
        }

        private SimpleParser FollowedBy(Action grammar)
        {
            if (m_triedRollback)
                return this;

            grammar();
            return this;
        }
    }
}