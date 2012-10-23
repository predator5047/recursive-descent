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
    public partial class NewTokenDialog : Form
    {
        private readonly List<Token> m_tokens;

        public NewTokenDialog(List<Token> tokens)
        {
            m_tokens = tokens;
            InitializeComponent();

            CustomComponentInitialization();
        }

        private void CustomComponentInitialization()
        {
            var items = Enum.GetValues(typeof(TokenType));

            foreach (var item in items)
            {
                cmbTokens.Items.Add(item);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var token = new Token
                            {
                                TokenType = (TokenType)Enum.Parse(typeof (TokenType), cmbTokens.SelectedItem.ToString()),
                                Value = txtValue.Text
                            };
            
            m_tokens.Add(token);
        }
    }
}
