using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gurobi;



namespace OfertaDiciplinas
{
    class ProblemaEscala
    {
        public int PeriodosDoCurso;
        public int TotalDeDisciplinas;
        public int QuantidadeMaximaDeDisciplinas;
        
        public int ConstanteCapacidade;
        public int QuantidadeOfertaMinimaDisciplinas;
        public int M;

        //Listas
        public List<List<int>> PreRequisitos;
        public List<string> AnosDeIngresso;       
        public List<string>[] DisciplinasDoPeriodoDoCurso; 
        public List<Estudante> DiscentesDoCurso;
        public List<string> Slots; 
        public List<string> TodasAsDisciplinasDoCurso;
        public List<string> TodosOsPreRequisitos;

        //Dicionários
        public Dictionary<string, int> DiciplinasPorPeriodoDoCurso;

        //Ambiente - Gurobi
        public GRBEnv Ambiente;
        public GRBModel Modelo;

        //Declaração de variaveis - Gurobi
        public GRBVar[,,,] X;        
        public GRBVar[,,] Y;        
        public GRBVar[,] F;
        public GRBVar[,] G;
        
        //Variaveis de folga
        public GRBVar[,] B;
        public GRBVar[] Q;

        public List<string> LeituraDisciplinas()
        {
            //Disciplinas

            //Leitura Disciplinas

            string[] LeituraDisciplinas = File.ReadAllLines(@"");


            List<string> TodasAsDisciplinas = new List<string>();


            foreach (string s in LeituraDisciplinas)
            {
                TodasAsDisciplinas.Add(s);
            }
            
            return TodasAsDisciplinas;
        }

        public List<Estudante> LeituraDiscentes()
        {
            //Leitura Discentes

            string[] Leitura = File.ReadAllLines(@"");

            int qtdLinhas = Leitura.Length;

            Estudante[] Estudantes = new Estudante[1000];

            int n = 1;

            for (int i = 0; i < qtdLinhas; i++) // caso tenha cabeçalho, subtrair 1 unidade 
            {
                string[] Atual = Leitura[i].Split(";"); // caso tenha cabeçalho, adicionar 1 unidade

                List<string> DisciplinasFaltantes = new List<string>();


                if (i == 0)
                {
                    Estudantes[i] = new Estudante();
                    
                    Estudantes[i].id = int.Parse(Atual[0]);

                    Estudantes[i].periodo = int.Parse(Atual[10]) - 1;
                    
                    Estudantes[i].AnoIngresso = int.Parse(Atual[1]);
                    
                    DisciplinasFaltantes.Add(Atual[2]);

                    Estudantes[i].DisciplinasFaltantes = DisciplinasFaltantes;
                    
                }

                if (i > 0)
                {

                    if (int.Parse(Atual[0]) == Estudantes[i - n].id)
                    {
                        Estudantes[i - n].DisciplinasFaltantes.Add(Atual[2]);
                        
                        n = n + 1;
                    }

                    else
                    {
                        DisciplinasFaltantes.Clear();                       

                        Estudantes[i - n + 1] = new Estudante();

                        Estudantes[i - n + 1].id = int.Parse(Atual[0]);

                        Estudantes[i - n + 1].periodo = int.Parse(Atual[10]) - 1;

                        Estudantes[i - n + 1].AnoIngresso = int.Parse(Atual[1]);
                        
                        DisciplinasFaltantes.Add(Atual[2]);

                        Estudantes[i - n + 1].DisciplinasFaltantes = DisciplinasFaltantes;
                       
                    }
                }
            }

            List<Estudante> ListaEstudantes = new List<Estudante>(Estudantes.Length);

            ListaEstudantes.AddRange(Estudantes);

            ListaEstudantes.RemoveAll(item => item == null);

            return ListaEstudantes;
        }

        public void CriarProblemaReal()
        { 
            PeriodosDoCurso = 8;
            QuantidadeMaximaDeDisciplinas = 92;
            ConstanteCapacidade = 60;
            QuantidadeOfertaMinimaDisciplinas = 2;
            M = 1000;

            //Disciplinas
            TodasAsDisciplinasDoCurso = LeituraDisciplinas();
            TotalDeDisciplinas = TodasAsDisciplinasDoCurso.Count;

            //Disciplinas por período
            DisciplinasDoPeriodoDoCurso = new List<string>[8];
            DisciplinasDoPeriodoDoCurso[0] = new List<string>() { "SA095", "SA096_OU_SA111", "SA097", "SA050_OU_SA050EAD", "SA048", "SA106" };
            DisciplinasDoPeriodoDoCurso[1] = new List<string>() { "SA068", "SA047_OU_SA107", "HC250", "CM300", "SA092", "CE003" };
            DisciplinasDoPeriodoDoCurso[2] = new List<string>() { "SA098_OU_SA108A", "SA099_OU_SA099A", "SA053", "SC205", "SA055", "SA056" };
            DisciplinasDoPeriodoDoCurso[3] = new List<string>() { "SA057", "SA100_OU_SA100A", "SA059_OU_SA114", "SA060", "SA049", "SA062" };
            DisciplinasDoPeriodoDoCurso[4] = new List<string>() { "SA063", "SA064", "SA065", "SA066_OU_SA113", "SA067", "SA051" };
            DisciplinasDoPeriodoDoCurso[5] = new List<string>() { "SA069", "HP228", "SA070", "SA071", "SA072_OU_SA109" };
            DisciplinasDoPeriodoDoCurso[6] = new List<string>() { "SA073", "SA074_OU_SA074EAD", "SA075", "SA076", "SA077_OU_SA110" };
            DisciplinasDoPeriodoDoCurso[7] = new List<string>() { "SA078", "SA079", "SA080", "SA081", "SA082", "SA101"};

            
            DiciplinasPorPeriodoDoCurso = new Dictionary<string, int>()
            {
                {"SA095", 0},
                {"SA096_OU_SA111", 0},
                {"SA097", 0},
                {"SA050_OU_SA050EAD", 0},
                {"SA048", 0},
                {"SA106", 0},
                {"SA068", 1},
                {"SA047_OU_SA107", 1},
                {"HC250", 1},
                {"CM300", 1},
                {"SA092", 1},
                {"CE003", 1},
                {"SA098_OU_SA108A", 2},
                {"SA099_OU_SA099A", 2},
                {"SA053", 2},
                {"SC205", 2},
                {"SA055", 2},
                {"SA056", 2},
                {"SA057", 3},
                {"SA100_OU_SA100A", 3},
                {"SA059_OU_SA114", 3},
                {"SA060", 3},
                {"SA049", 3},
                {"SA062", 3},
                {"SA063", 4},
                {"SA064", 4},
                {"SA065", 4},
                {"SA066_OU_SA113", 4},
                {"SA067", 4},
                {"SA051", 4},
                {"SA069", 5},
                {"HP228", 5},
                {"SA070", 5},
                {"SA071", 5},
                {"SA072_OU_SA109", 5},
                {"SA073", 6},
                {"SA074_OU_SA074EAD", 6},
                {"SA075", 6},
                {"SA076", 6},
                {"SA077_OU_SA110", 6},
                {"SA078", 7},
                {"SA079", 7},
                {"SA080", 7},
                {"SA081", 7},
                {"SA082", 7},
                {"SA101", 7}
            };            

            //Pre-requisitos
            PreRequisitos = new List<List<int>>();

            List<int> SA095 = new List<int>() { };
            List<int> SA096_OU_SA111 = new List<int>() { };
            List<int> SA097 = new List<int>() { };
            List<int> SA050_OU_SA050EAD = new List<int>() { };
            List<int> SA048 = new List<int>() { };
            List<int> SA106 = new List<int>() { };
            List<int> SA068 = new List<int>() { };
            List<int> SA047_OU_SA107 = new List<int>() { };
            List<int> HC250 = new List<int>() { };
            List<int> CM300 = new List<int>() { };
            List<int> SA092 = new List<int>() { };
            List<int> CE003 = new List<int>() { };
            List<int> SA098_OU_SA108A = new List<int>() { };
            List<int> SA099_OU_SA099A = new List<int>() { 0 };
            List<int> SA053 = new List<int>() { 1 };
            List<int> SC205 = new List<int>() { };
            List<int> SA055 = new List<int>() { 3 };
            List<int> SA056 = new List<int>() { };
            List<int> SA057 = new List<int>() { };
            List<int> SA100_OU_SA100A = new List<int>() { 0 };
            List<int> SA059_OU_SA114 = new List<int>() { };
            List<int> SA060 = new List<int>() { };
            List<int> SA049 = new List<int>() { };
            List<int> SA062 = new List<int>() { };
            List<int> SA063 = new List<int>() { };
            List<int> SA064 = new List<int>() { };
            List<int> SA065 = new List<int>() { 1 };
            List<int> SA066_OU_SA113 = new List<int>() { };
            List<int> SA067 = new List<int>() { };
            List<int> SA051 = new List<int>() { };
            List<int> SA069 = new List<int>() { };
            List<int> HP228 = new List<int>() { };
            List<int> SA070 = new List<int>() { 1 };
            List<int> SA071 = new List<int>() { 2 };
            List<int> SA072_OU_SA109 = new List<int>() { };
            List<int> SA073 = new List<int>() { };
            List<int> SA074_OU_SA074EAD = new List<int>() { };
            List<int> SA075 = new List<int>() { 1 };
            List<int> SA076 = new List<int>() { };
            List<int> SA077_OU_SA110 = new List<int>() { 34 };
            List<int> SA078 = new List<int>() { };
            List<int> SA079 = new List<int>() { };
            List<int> SA080 = new List<int>() { 1 };
            List<int> SA081 = new List<int>() { };
            List<int> SA082 = new List<int>() { 16 };
            List<int> SA101 = new List<int>() { 8 };

            PreRequisitos.Add(SA095);
            PreRequisitos.Add(SA096_OU_SA111);
            PreRequisitos.Add(SA097);
            PreRequisitos.Add(SA050_OU_SA050EAD);
            PreRequisitos.Add(SA048);
            PreRequisitos.Add(SA106);
            PreRequisitos.Add(SA068);
            PreRequisitos.Add(SA047_OU_SA107);
            PreRequisitos.Add(HC250);
            PreRequisitos.Add(CM300);
            PreRequisitos.Add(SA092);
            PreRequisitos.Add(CE003);
            PreRequisitos.Add(SA098_OU_SA108A);
            PreRequisitos.Add(SA099_OU_SA099A);
            PreRequisitos.Add(SA053);
            PreRequisitos.Add(SC205);
            PreRequisitos.Add(SA055);
            PreRequisitos.Add(SA056);
            PreRequisitos.Add(SA057);
            PreRequisitos.Add(SA100_OU_SA100A);
            PreRequisitos.Add(SA059_OU_SA114);
            PreRequisitos.Add(SA060);
            PreRequisitos.Add(SA049);
            PreRequisitos.Add(SA062);
            PreRequisitos.Add(SA063);
            PreRequisitos.Add(SA064);
            PreRequisitos.Add(SA065);
            PreRequisitos.Add(SA066_OU_SA113);
            PreRequisitos.Add(SA067);
            PreRequisitos.Add(SA051);
            PreRequisitos.Add(SA069);
            PreRequisitos.Add(HP228);
            PreRequisitos.Add(SA070);
            PreRequisitos.Add(SA071);
            PreRequisitos.Add(SA072_OU_SA109);
            PreRequisitos.Add(SA073);
            PreRequisitos.Add(SA074_OU_SA074EAD);
            PreRequisitos.Add(SA075);
            PreRequisitos.Add(SA076);
            PreRequisitos.Add(SA077_OU_SA110);
            PreRequisitos.Add(SA078);
            PreRequisitos.Add(SA079);
            PreRequisitos.Add(SA080);
            PreRequisitos.Add(SA081);
            PreRequisitos.Add(SA082);
            PreRequisitos.Add(SA101);

            //Slots
            Slots = new List<string>() { "seg", "ter", "qua", "qui", "sex", "sab" };

            //Discentes
            DiscentesDoCurso = LeituraDiscentes();
        }    
                        
        public void CriarModelo()
        {
            Ambiente = new GRBEnv();
            Ambiente.Set("LogFile", @"");
            Modelo = new GRBModel(Ambiente);          
            CriarProblemaReal();            
            CriarVariaveisDecisao();
            ConjuntoDeRestricoes01();
            ConjuntoDeRestricoes02();
            ConjuntoDeRestricoes03();
            ConjuntoDeRestricoes04();
            ConjuntoDeRestricoes05();
            ConjuntoDeRestricoes06();
            ConjuntoDeRestricoes07();
            ConjuntoDeRestricoes08();
            ConjuntoDeRestricoes09();
            ConjuntoDeRestricoes10();
            FO();
        }

        public void CriarVariaveisDecisao()
        {            
            //Variavel X
            X = new GRBVar[DiscentesDoCurso.Count, TotalDeDisciplinas, Slots.Count, PeriodosDoCurso];            

            //Variavel Y
            Y = new GRBVar[TodasAsDisciplinasDoCurso.Count, Slots.Count, PeriodosDoCurso];

            //Variavel F
            F = new GRBVar[DiscentesDoCurso.Count, PeriodosDoCurso];

            //Variavel de folga B
            B = new GRBVar[DiscentesDoCurso.Count, PeriodosDoCurso];

            //Variavel de folga Q
            Q = new GRBVar[PeriodosDoCurso];

            //Variavel G
            G = new GRBVar[DiscentesDoCurso.Count, PeriodosDoCurso];


            //Definir Variaveis de Decisão
            
            //X
            for (int e = 0; e < DiscentesDoCurso.Count; e++)
            {
                for (int d = 0; d < TotalDeDisciplinas; d++)
                {
                    for (int s = 0; s < Slots.Count; s++)
                    {
                        for (int p = 0; p < PeriodosDoCurso; p++)
                        {                            
                            X[e, d, s, p] = Modelo.AddVar(0, 1, 0, GRB.BINARY, $"x_{e}_{d}_{s}_{p}");
                        }
                    }
                }
            }

            //Y
            for (int d=0; d < TotalDeDisciplinas; d++)
            {
                for (int s=0; s < Slots.Count; s++)
                {
                    for(int p = 0; p < PeriodosDoCurso; p++)
                    {
                        Y[d, s, p] = Modelo.AddVar(0, 100, 0, GRB.INTEGER, $"y_{d}_{s}_{p}");
                    }                    
                }
            }

            //F
            for(int e=0; e < DiscentesDoCurso.Count; e++)
            {
                for(int p = 0; p < PeriodosDoCurso; p++)
                {
                    F[e, p] = Modelo.AddVar(0, 100, 0, GRB.INTEGER, $"f_{e}_{p}");
                }                
            }

            //B
            for (int e = 0; e < DiscentesDoCurso.Count; e++)
            {
                for (int p = 0; p < PeriodosDoCurso; p++)
                {
                    B[e, p] = Modelo.AddVar(0, 6, 1, GRB.INTEGER, $"b_{e}_{p}");
                }                    
            }

            //Q

            for (int p = 0; p < PeriodosDoCurso; p++)
            {
                Q[p] = Modelo.AddVar(0, 100, 0, GRB.INTEGER, $"q_{p}");
            }

            //G
            for (int e = 0; e < DiscentesDoCurso.Count; e++)
            {
                for (int p = 0; p < PeriodosDoCurso; p++)
                {
                    G[e, p] = Modelo.AddVar(0, 1, 0, GRB.BINARY, $"g_{e}_{p}");
                }
            }
        }

        //R01: No mesmo horário do mesmo período "p" cada discente "e" deve se matricular em no máximo 1 disciplina "d"
        public void ConjuntoDeRestricoes01()
        {
            GRBLinExpr expr = new GRBLinExpr();
            
            for (int e = 0; e < DiscentesDoCurso.Count; e++)
            {
                for (int s = 0; s < Slots.Count; s++)
                {
                    for (int p = 0; p < PeriodosDoCurso; p++)
                    {
                        expr.Clear();
                        for (int d = 0; d < TotalDeDisciplinas; d++)
                        {
                            if (DiscentesDoCurso[e].DisciplinasFaltantes.Contains(TodasAsDisciplinasDoCurso[d]))
                            {
                                expr.AddTerm(1, X[e, d, s, p]);
                            }
                        }

                        Modelo.AddConstr(expr <= 1, $"R01_{e}_{s}_{p}");
                    }
                }
            }
        } 

        //R02: Para todo período "p", todo discente "e" deve ter matrícula em 6 disciplinas "d"
        public void ConjuntoDeRestricoes02()
        {
            GRBLinExpr expr = new GRBLinExpr();
            for (int e = 0; e < DiscentesDoCurso.Count; e++)
            {
                for (int p = 0; p < PeriodosDoCurso; p++)
                {
                    expr.Clear();
                    for (int s = 0; s < Slots.Count; s++)
                    {
                        for (int d = 0; d < TotalDeDisciplinas; d++)
                        {
                            if (DiscentesDoCurso[e].DisciplinasFaltantes.Contains(TodasAsDisciplinasDoCurso[d]))
                            {
                                expr.AddTerm(1, X[e, d, s, p]);
                            }
                        }
                    }
                    Modelo.AddConstr(expr + B[e, p] == 6, $"R02_{e}_{p}");
                }
               
            }
        }

        //R03: Todo discente "e" deve se matricular 1 única vez em cada disciplina faltante "d" 
        public void ConjuntoDeRestricoes03()
        {
            GRBLinExpr expr = new GRBLinExpr();

            for (int e = 0; e < DiscentesDoCurso.Count; e++)
            {

                for (int d = 0; d < TotalDeDisciplinas; d++)
                {
                    if (DiscentesDoCurso[e].DisciplinasFaltantes.Contains(TodasAsDisciplinasDoCurso[d]))
                    {
                        expr.Clear();
                        for (int s = 0; s < Slots.Count; s++)
                        {
                            for (int p = 0; p < PeriodosDoCurso; p++)
                            {
                                expr.AddTerm(1, X[e, d, s, p]);
                            }
                        }

                        Modelo.AddConstr(expr == 1, $"R03_{e}_{d}");
                    }                                     
                }                
            }                
        }

        //R04: O discente "e" só deve se matricular em uma disciplina "d", caso os pré-requisitos tenham sido concluidos
        public void ConjuntoDeRestricoes04()
        {
            GRBLinExpr expr1 = new GRBLinExpr();
            GRBLinExpr expr2 = new GRBLinExpr();

            for (int e = 0; e < DiscentesDoCurso.Count; e++)
            {
                for (int d = 0; d < TotalDeDisciplinas; d++)
                {
                    for (int p = 0; p < PeriodosDoCurso; p++)
                    {

                        foreach(int g in PreRequisitos[d])
                        {
                            if (DiscentesDoCurso[e].DisciplinasFaltantes.Contains(TodasAsDisciplinasDoCurso[g]))
                            {
                                expr1.Clear();
                                expr2.Clear();

                                for (int s = 0; s < Slots.Count; s++)
                                {
                                    expr1.AddTerm(1, X[e, d, s, p]);
                                }
                                
                                for (int s = 0; s < Slots.Count; s++)
                                {
                                    for (int q = 0; q < p; q++)
                                    {
                                        expr2.AddTerm(1, X[e, g, s, q]);
                                    }
                                }

                                Modelo.AddConstr(expr1 <= expr2, $"R04_{e}_{d}_{g}_{p}");
                            }
                        }                        
                    }                    
                }
            }
        }

        //R05: A partir do período p = 5, nenhuma disciplina d será ofertada no slot s = 5:
        public void ConjuntoDeRestricoes05()
        {
            for (int d = 0; d < TodasAsDisciplinasDoCurso.Count; d++)
            {
                for (int periodo = 5; periodo < 8; periodo++)
                {                                         
                    foreach (string disciplina in DisciplinasDoPeriodoDoCurso[periodo])
                    {
                        if (TodasAsDisciplinasDoCurso[d].Contains(disciplina))
                        {              
                            for (int p = 0; p < PeriodosDoCurso; p++)
                            {
                                Modelo.AddConstr(Y[d, 5, p] == 0, $"R05_{d}_{p}");
                            }
                        }
                    }
                }              
            }
        }
                
        //R06: Garantir a quantidade mínima da disciplina "d" ofertada em todos os slots "s"
        public void ConjuntoDeRestricoes06()
        {
            GRBLinExpr expr = new GRBLinExpr();
            for (int d = 0; d < TodasAsDisciplinasDoCurso.Count; d++)
            {
                for (int p = 0; p < PeriodosDoCurso; p++)
                {
                    expr.Clear();
                    for (int s = 0; s < Slots.Count; s++)
                    {
                        expr.AddTerm(1, Y[d, s, p]);
                    }
                    Modelo.AddConstr(expr >= QuantidadeOfertaMinimaDisciplinas, $"R06_{d}_{p}");
                }                 
            }
        }

        //R07: Respeitar a quantidade máxima de disciplinas "d" oferecidas em todo período "p"
        public void ConjuntoDeRestricoes07()
        {            
            GRBLinExpr expr = new GRBLinExpr();

            for (int p = 0; p < PeriodosDoCurso; p++)
            {
                expr.Clear();
                for (int d = 0; d < TodasAsDisciplinasDoCurso.Count; d++)
                {
                    for (int s = 0; s < Slots.Count; s++)
                    {
                        expr.AddTerm(1, Y[d, s, p]);
                    }
                }

                Modelo.AddConstr(expr <= QuantidadeMaximaDeDisciplinas + Q[p], $"R07_{p}");

            }
        }

        //R08: Quantidade máxima de alunos matriculados na disciplina "d" no slot "s"
        //deve ser menor que a quantidade de vagas ofertadas da disciplina "d" no slot "s"
        public void ConjuntoDeRestricoes08()
        {
            GRBLinExpr expr = new GRBLinExpr();            

            for (int d=0; d < TotalDeDisciplinas; d++)
            {
                for (int s=0; s < Slots.Count; s++)
                {
                    for (int p = 0; p < PeriodosDoCurso; p++)
                    {
                        expr.Clear();
                        for (int e = 0; e < DiscentesDoCurso.Count; e++)
                        {
                            if (DiscentesDoCurso[e].DisciplinasFaltantes.Contains(TodasAsDisciplinasDoCurso[d]))
                            {
                                expr.AddTerm(1, X[e, d, s, p]);
                            }
                        }

                        Modelo.AddConstr(expr <= ConstanteCapacidade * Y[d, s, p], $"R08_{d}_{s}_{p}");
                    }                       
                }
            }
        }        

        //R09: Quantidade de disciplinas "d" restantes para o discente "e"
        public void ConjuntoDeRestricoes09()
        {
            GRBLinExpr expr = new GRBLinExpr();
            for (int e = 0; e < DiscentesDoCurso.Count; e++)
            {
                for(int p = 1; p < PeriodosDoCurso; p++)
                {
                    expr.Clear();
                    for (int s = 0; s < Slots.Count; s++)
                    {
                        for (int d = 0; d < TotalDeDisciplinas; d++)
                        {
                            if (DiscentesDoCurso[e].DisciplinasFaltantes.Contains(TodasAsDisciplinasDoCurso[d]))
                            {
                                expr.AddTerm(1, X[e, d, s, p]);
                            }
                        }
                    }
                    
                    Modelo.AddConstr(F[e, p] == F[e, p - 1] - expr, $"R09_{e}_{p}");                    
                }                
            }
        }

        //R10: Associar a função objetivo

        public void ConjuntoDeRestricoes10()
        {
            for (int e = 0; e < DiscentesDoCurso.Count; e++)
            {
                for (int p = 0; p < PeriodosDoCurso; p++)
                {
                    Modelo.AddConstr(F[e, p] <= M * G[e, p], $"R10_{e}_{p}");                   
                }
            }
        }
       
        //Funcao Objetivo

        public void FO()
        {

            GRBLinExpr expr1 = new GRBLinExpr();

            for (int e = 0; e < DiscentesDoCurso.Count; e++)
            {
                for (int p = 0; p < PeriodosDoCurso; p++)
                {
                    expr1.AddTerm(Math.Pow(2, p), F[e, p]);
                }
            }

            GRBLinExpr expr2 = new GRBLinExpr();

            for (int e = 0; e < DiscentesDoCurso.Count; e++)
            {
                for (int p = 0; p < PeriodosDoCurso; p++)
                {
                    expr2.AddTerm((8 - p), B[e, p]);
                }
            }

            GRBLinExpr expr3 = new GRBLinExpr();

            for (int e = 0; e < DiscentesDoCurso.Count; e++)
            {
                for (int d = 0; d < TodasAsDisciplinasDoCurso.Count; d++)
                {
                    if (DiscentesDoCurso[e].DisciplinasFaltantes.Contains(TodasAsDisciplinasDoCurso[d]))
                    {
                        for (int s = 0; s < Slots.Count; s++)
                        {
                            for (int p = 0; p < PeriodosDoCurso; p++)
                            {
                                if (DiciplinasPorPeriodoDoCurso[TodasAsDisciplinasDoCurso[d]] - DiscentesDoCurso[e].periodo - p > 0)
                                {
                                    expr3.AddTerm(10 * (DiciplinasPorPeriodoDoCurso[TodasAsDisciplinasDoCurso[d]] - DiscentesDoCurso[e].periodo - p), X[e, d, s, p]);
                                }
                            }
                        }
                    }
                }
            }

            Modelo.SetObjective(expr1 + expr2 + expr3, GRB.MINIMIZE);
        }

        public void EscreverRespostaX()
        {
            using (StreamWriter writer = new StreamWriter(@""))           

            //X
            for (int e = 0; e < DiscentesDoCurso.Count; e++)
            {
                for (int d = 0; d < TotalDeDisciplinas; d++)
                {
                    for (int s = 0; s < Slots.Count; s++)
                    {
                        for (int p = 0; p < PeriodosDoCurso; p++)
                        {
                            if (X[e, d, s, p].X > 0.001)
                            {
                                    writer.WriteLine($"x_{e}_{d}_{s}_{p} {X[e, d, s, p].X}");
                            }
                        }
                    }
                }
            }            
        }

        public void EscreverRespostaY()
        {
            using (StreamWriter writer = new StreamWriter(@""))

                //Y
                for (int d = 0; d < TotalDeDisciplinas; d++)
                {
                    for (int s = 0; s < Slots.Count; s++)
                    {
                        for (int p = 0; p < PeriodosDoCurso; p++)
                        {
                            if (Y[d, s, p].X > 0.001)
                            {
                                writer.WriteLine($"y_{d}_{s}_{p} {Y[d, s, p].X}");
                            }
                        }
                    }
                }
        }
        
    }

    class Estudante
    {
        public int id;
        public int periodo;
        public int AnoIngresso;
        public List<string> DisciplinasFaltantes; 
    }
}
