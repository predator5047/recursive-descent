using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Parser;
using SimpleCompiler;

namespace TreeVisualizer
{
    public partial class Form1 : Form
    {
        public List<Token> Tokens { get; set; }
        ILexer lexer = new Lexer();

        public Form1()
        {
            // Initialize empty token list
            Tokens = new List<Token>();

            // Component initialization
            InitializeComponent();
            CustomComponentInitialization();
        }

        private void CustomComponentInitialization()
        {
            var tabs = new List<int>();

            const int maxTabLevel = 10;

            for (int i = 1; i < maxTabLevel; i++)
            {
                tabs.Add(i*30);
            }

            txtInput.SelectionTabs = tabs.ToArray();

            GenerateTokens();
            UpdateTokenList();
            PopulateComboBox();
        }

        private void PopulateComboBox()
        {
            var items = Enum.GetValues(typeof (TokenType));

            foreach (var item in items)
            {
                cmbTokens.Items.Add(item);
            }

            cmbTokens.SelectedIndex = 1;
        }

        private void GenerateTokens()
        {
            /*
            Tokens.Add(new Token { TokenType = TokenType.NAME });
            Tokens.Add(new Token { TokenType = TokenType.EQ });
            Tokens.Add(new Token { TokenType = TokenType.VALUE });
            Tokens.Add(new Token { TokenType = TokenType.SEMI });
             */

            Tokens.Add(new Token { TokenType = TokenType.IF, Value = "if" });
            Tokens.Add(new Token { TokenType = TokenType.LPAR, Value = "(" });
            Tokens.Add(new Token { TokenType = TokenType.NAME, Value = "a" });
            Tokens.Add(new Token { TokenType = TokenType.EQ, Value = "==" });
            Tokens.Add(new Token { TokenType = TokenType.VALUE, Value = "5" });
            Tokens.Add(new Token { TokenType = TokenType.RPAR, Value = ")" });
            Tokens.Add(new Token { TokenType = TokenType.LBRA, Value = "{" });
            Tokens.Add(new Token { TokenType = TokenType.NAME, Value = "Print" });
            Tokens.Add(new Token { TokenType = TokenType.LPAR, Value = "(" });
            Tokens.Add(new Token { TokenType = TokenType.VALUE, Value = "\"Blaha!\"" });
            Tokens.Add(new Token { TokenType = TokenType.RPAR, Value = ")" });
            Tokens.Add(new Token { TokenType = TokenType.SEMI, Value = ";" });
            Tokens.Add(new Token { TokenType = TokenType.RBRA, Value = "}" });
        }

        private void addTokenToStreamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tokenDialog = new NewTokenDialog(Tokens);
            tokenDialog.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void btnAddToken_Click(object sender, EventArgs e)
        {
            var token = new Token
            {
                TokenType = (TokenType)Enum.Parse(typeof(TokenType), cmbTokens.SelectedItem.ToString()),
                Value = txtValue.Text
            };

            Tokens.Add(token);
            UpdateTokenList();
        }

        private void UpdateTokenList()
        {
            tokenList.DataSource = Tokens.ToArray();
        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            CallParse();
        }

        private Node<string> CallParse()
        {
            if (txtInput.Text.Length == 0)
            {
                AddTextLineToOutput("No input specified", Color.Red);
                return null;
            }

            var parser = new SimpleParser(lexer);

            try
            {
                parser.Parse(txtInput.Text);
                RenderTree(parser.SyntaxTree);
                AddTextLineToOutput("Parsing succeeded", Color.DarkGreen);
                return parser.SyntaxTree;
            }
            catch (ParserException ex)
            {
                AddTextLineToOutput(ex.Message, Color.Red);
                return null;
            }
        }

        private void CallTokenize()
        {
            Tokens.Clear();

            try
            {
                var tokens = lexer.Tokenize(txtInput.Text);
                
                foreach (var token in tokens)
                {
                    Tokens.Add(token);
                }

                AddTextLineToOutput("Lexical analysis passed", Color.DarkGreen);
                UpdateTokenList();
            }
            catch (LexerException exception)
            {
                AddTextLineToOutput(exception.Message, Color.Red);
            }
        }

        private void RenderTree(Node<string> tree)
        {
            syntaxTreeView.Nodes.Clear();
            AddNode(syntaxTreeView.Nodes.Add("Parse tree"), tree);

            syntaxTreeView.ExpandAll();
        }

        private void AddNode(TreeNode fromView, Node<string> n)
        {
            var node = fromView.Nodes.Add(n.Value);

            if (n.Children.Count > 0)
            {
                foreach (var token in n.Children)
                {
                    AddNode(node, token);
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F5))
            {
                txtOutput.Clear();
                CallParse();
            }

            if (keyData == Keys.F5)
            {
                CallParse();
            }

            if (keyData == (Keys.Control | Keys.F7))
            {
                txtOutput.Clear();
                CallTokenize();
            }

            if (keyData == Keys.F7)
            {
                CallTokenize();
            }

            if (keyData == Keys.F6)
            {
                CallCompile();
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnTokenize_Click(object sender, EventArgs e)
        {
            CallTokenize();
        }

        private void AddTextLineToOutput(string text)
        {
            AddTextLineToOutput(text, Color.Black);
        }

        private void AddTextLineToOutput(string text, Color c)
        {
            txtOutput.SelectionColor = c;
            txtOutput.AppendText(text);
            txtOutput.AppendText("\u2028");
        }

        /*private void txtInput_TextChanged(object sender, EventArgs e)
        {

        }*/

        private void btnCompile_Click(object sender, EventArgs e)
        {
            CallCompile();
        }

        private void CallCompile()
        {
            var tree = CallParse();

            if (tree == null)
                return;

            AddTextLineToOutput("Building...", Color.YellowGreen);

            var compiler = new Compiler();
            compiler.Compile(tree);
        }

        private void txtInput_TextChanged_1(object sender, EventArgs e)
        {
            
        }

        private void tokenList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
