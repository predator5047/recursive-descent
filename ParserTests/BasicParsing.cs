using System.Collections.Generic;
using NUnit.Framework;
using Parser;
using Rhino.Mocks;

namespace ParserTests
{
    [TestFixture]
    public class BasicParsing
    {
        private ILexer lexerStub;
        private SimpleParser sut;
        private PQueue<Token> mockTokens;

        [SetUp]
        public void Setup()
        {
            lexerStub = MockRepository.GenerateStub<ILexer>();
            sut = new SimpleParser(lexerStub);
            mockTokens = new PQueue<Token>();
        }

        [Test]
        public void TryAssignment()
        {
            mockTokens.Enqueue(new Token { TokenType = TokenType.VAR } );
            mockTokens.Enqueue(new Token { TokenType = TokenType.NAME });
            mockTokens.Enqueue(new Token { TokenType = TokenType.ASSIGN });
            mockTokens.Enqueue(new Token { TokenType = TokenType.VALUE });
            mockTokens.Enqueue(new Token { TokenType = TokenType.SEMI });

            lexerStub.Stub(x => x.Tokenize("")).IgnoreArguments().Return(mockTokens);

            sut.Parse("");

            Assert.AreEqual(0, sut.Errors.Count);
        }

        [Test]
        public void TryComparison()
        {
            mockTokens.Enqueue(new Token { TokenType = TokenType.NAME });
            mockTokens.Enqueue(new Token { TokenType = TokenType.EQ });
            mockTokens.Enqueue(new Token { TokenType = TokenType.VALUE });
            mockTokens.Enqueue(new Token { TokenType = TokenType.SEMI });

            lexerStub.Stub(x => x.Tokenize("")).IgnoreArguments().Return(mockTokens);

            sut.Parse("");

            Assert.AreEqual(0, sut.Errors.Count);
        }
    }
}