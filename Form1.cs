using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OfertaDiciplinas
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ProblemaEscala MeuProblemaEscala = new ProblemaEscala();            
            MeuProblemaEscala.CriarModelo();            
            MeuProblemaEscala.Modelo.Write(@"");
            MeuProblemaEscala.Modelo.Optimize();
            MeuProblemaEscala.Modelo.Write(@"");
            MeuProblemaEscala.EscreverRespostaX();
            MeuProblemaEscala.EscreverRespostaY();
        }
    }
}
