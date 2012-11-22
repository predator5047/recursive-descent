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
            mockTokens.Enqueue(new Token { TokenType = TokenType.VAR, Value = "string"} );
            mockTokens.Enqueue(new Token { TokenType = TokenType.NAME, Value = "abc" });
            mockTokens.Enqueue(new Token { TokenType = TokenType.ASSIGN, Value = "=" });
            mockTokens.Enqueue(new Token { TokenType = TokenType.VALUE, Value = "5" });
            mockTokens.Enqueue(new Token { TokenType = TokenType.SEMI, Value = ";" });

            lexerStub.Stub(x => x.Tokenize("")).IgnoreArguments().Return(mockTokens);

            sut.Parse("");

            Assert.DoesNotThrow(() => sut.Parse(""));
        }

        //[Test]
        //public void TrySeveralStatementsInsideIf()
        //{
        //    mockTokens.Enqueue(new Token { TokenType = TokenType.IF });
        //    mockTokens.Enqueue(new Token { TokenType = TokenType.LPAR });

        //    mockTokens.Enqueue(new Token { TokenType = TokenType.NAME });
        //    mockTokens.Enqueue(new Token { TokenType = TokenType.EQ });
        //    mockTokens.Enqueue(new Token { TokenType = TokenType.VALUE });
        //    mockTokens.Enqueue(new Token { TokenType = TokenType.RPAR });
        //    mockTokens.Enqueue(new Token { TokenType = TokenType.LBRA });
        //    mockTokens.Enqueue(new Token { TokenType = TokenType.NAME });
        //    mockTokens.Enqueue(new Token { TokenType = TokenType.LPAR });
        //    mockTokens.Enqueue(new Token { TokenType = TokenType.VALUE });
        //    mockTokens.Enqueue(new Token { TokenType = TokenType.RPAR });
        //    mockTokens.Enqueue(new Token { TokenType = TokenType.SEMI });
        //    mockTokens.Enqueue(new Token { TokenType = TokenType.VAR, Value = "string" });
        //    mockTokens.Enqueue(new Token { TokenType = TokenType.NAME, Value = "abc" });
        //    mockTokens.Enqueue(new Token { TokenType = TokenType.ASSIGN, Value = "=" });
        //    mockTokens.Enqueue(new Token { TokenType = TokenType.VALUE, Value = "5" });
        //    mockTokens.Enqueue(new Token { TokenType = TokenType.SEMI, Value = ";" });
        //    mockTokens.Enqueue(new Token { TokenType = TokenType.RBRA });


        //    lexerStub.Stub(x => x.Tokenize("")).IgnoreArguments().Return(mockTokens);

        //    sut.Parse("");

        //    Assert.AreEqual(0, sut.Errors.Count);
        //}

        [Test]
        public void TryComparison()
        {
            mockTokens.Enqueue(new Token { TokenType = TokenType.NAME });
            mockTokens.Enqueue(new Token { TokenType = TokenType.EQ });
            mockTokens.Enqueue(new Token { TokenType = TokenType.VALUE });
            mockTokens.Enqueue(new Token { TokenType = TokenType.SEMI });

            lexerStub.Stub(x => x.Tokenize("")).IgnoreArguments().Return(mockTokens);

            Assert.DoesNotThrow(() => sut.Parse(""));
        }

        [Test]
        public void SingleStatementInsideIf()
        {
            // Start Prg "if (expr) { stmt* }"
            // Start Stmt: "if ("
            mockTokens.Enqueue(new Token { TokenType = TokenType.IF });
            mockTokens.Enqueue(new Token { TokenType = TokenType.LPAR });

            // Start Expression: "name == 2"
            mockTokens.Enqueue(new Token { TokenType = TokenType.NAME });
            mockTokens.Enqueue(new Token { TokenType = TokenType.EQ });
            mockTokens.Enqueue(new Token { TokenType = TokenType.VALUE });
            // End Expression

            mockTokens.Enqueue(new Token { TokenType = TokenType.RPAR });
            mockTokens.Enqueue(new Token { TokenType = TokenType.LBRA });

            // Start Stmt: "expr;"
            // Start Expr: "print(2)"
            mockTokens.Enqueue(new Token { TokenType = TokenType.NAME });
            mockTokens.Enqueue(new Token { TokenType = TokenType.LPAR });
            mockTokens.Enqueue(new Token { TokenType = TokenType.VALUE });
            mockTokens.Enqueue(new Token { TokenType = TokenType.RPAR });
            // End Stmt

            mockTokens.Enqueue(new Token { TokenType = TokenType.SEMI });
            // End expr

            mockTokens.Enqueue(new Token { TokenType = TokenType.RBRA });
            // End Stmt
            // End Prg

            lexerStub.Stub(x => x.Tokenize("")).IgnoreArguments().Return(mockTokens);

            Assert.DoesNotThrow(() => sut.Parse(""));
        }
    }
}