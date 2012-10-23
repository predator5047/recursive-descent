using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Parser;

namespace TreeVisualizer
{
    public partial class Form1 : Form
    {
        public List<Token> Tokens { get; set; }

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
            Tokens.Add(new Token { TokenType = TokenType.VALUE, Value = "\"Hello world!\"" });
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

        private void CallParse()
        {
            var lexer = new FakeLexer();
            lexer.LexerOutput = Tokens;

            var parser = new SimpleParser(lexer);

            parser.Parse("");

            SetOutput(parser.Errors);

            if (parser.Errors.Count == 0)
                RenderTree(parser.SyntaxTree);
        }

        private void RenderTree(Node<string> tree)
        {
            syntaxTreeView.Nodes.Clear();

            AddNode(syntaxTreeView.Nodes.Add("BASE"), tree);
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

        private void SetOutput(Queue<Error> errors)
        {
            txtOutput.Text = string.Empty;

            if (errors.Count > 0)
            {
                txtOutput.ForeColor = Color.DarkRed;
                foreach (var error in errors)
                {
                    txtOutput.Text += error.ToString() + '\r' + '\n';
                }
            }
            else
            {
                txtOutput.ForeColor = Color.DarkGreen;
                txtOutput.Text = "Parsing succeeded";
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F5)
            {
                CallParse();
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
