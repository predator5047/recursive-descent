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
        }
    }
}
