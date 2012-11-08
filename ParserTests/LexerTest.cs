using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Parser;

namespace ParserTests
{
    [TestFixture]
    class LexerTest
    {
        ILexer sut;

        [SetUp]
        public void Setup()
        {
            sut = new Lexer();
        }

        [Test]
        public void CanCreateSingleLexem()
        {
            var input = "if";
            var expected = new PQueue<Token>();
            expected.Enqueue(new Token { TokenType = TokenType.IF, Value = "if" });

            var actual = sut.Tokenize(input);
            Assert.AreEqual(expected.First().TokenType, actual.First().TokenType);
            Assert.AreEqual(expected.First().Value, actual.First().Value);
            Assert.AreEqual(expected.Count, actual.Count);
        }

        [Test]
        public void CanCreateTwoLexemes()
        {
            var input = "if(";
            var expected = new PQueue<Token>();
            expected.Enqueue(new Token { TokenType = TokenType.IF, Value = "if" });
            expected.Enqueue(new Token { TokenType = TokenType.LPAR, Value = "(" });

            var actual = sut.Tokenize(input);

            Assert.AreEqual(expected.Count, actual.Count);
            Assert.AreEqual(expected.First().TokenType, actual.First().TokenType);
            Assert.AreEqual(expected.First().Value, actual.First().Value);
            Assert.AreEqual(expected.Last().TokenType, actual.Last().TokenType);
            Assert.AreEqual(expected.Last().Value, actual.Last().Value);
        }

        [Test]
        public void CanIdentifyRegexLexem()
        {
            var input = "banan";
            var expected = new PQueue<Token>();
            expected.Enqueue(new Token { TokenType = TokenType.NAME, Value = "banan" });

            var actual = sut.Tokenize(input);
            Assert.AreEqual(expected.First().TokenType, actual.First().TokenType);
            Assert.AreEqual(expected.First().Value, actual.First().Value);
            Assert.AreEqual(expected.Count, actual.Count);
        }

        [Test]
        public void IgnoresWhiteSpaceInExpressions()
        {
            var input = "if (";
            var expected = new PQueue<Token>();
            expected.Enqueue(new Token { TokenType = TokenType.IF, Value = "if" });
            expected.Enqueue(new Token { TokenType = TokenType.LPAR, Value = "(" });

            var actual = sut.Tokenize(input);

            Assert.AreEqual(expected.Count, actual.Count);
            Assert.AreEqual(expected.First().TokenType, actual.First().TokenType);
            Assert.AreEqual(expected.First().Value, actual.First().Value);
            Assert.AreEqual(expected.Last().TokenType, actual.Last().TokenType);
            Assert.AreEqual(expected.Last().Value, actual.Last().Value);
        }
    }
}
