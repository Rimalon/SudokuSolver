using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;


namespace SudokuSolverProj
{
    class Program
    {
        static void Main(string[] args)
        {
            var sudokuSolver = new SudokuSolver(args[0], args[1]);
            sudokuSolver.Solve();
        }
    }
    
    public class SudokuSolver
    {
        private InputSATSolverFile SatFileContent { get; }
        private int[][] SudokuTable { get; }
        private string OutputPath { get; }

        public SudokuSolver(string inputPath, string outputPath)
        {
            SatFileContent = new InputSATSolverFile();
            SudokuTable = GetTable(inputPath);
            OutputPath = outputPath;
        }

        public void Solve()
        {
            AddInitValues();
            FirstRule();
            SecondRule();
            ThirdRule();
            FourthRule();
            FifthRule();
            
            string satSolveInputPath = Path.GetTempPath() + "satSolveInput.txt";
            using (var writer = File.CreateText(satSolveInputPath))
            {
                writer.Write(SatFileContent.CreateFileContent().ToString());
            }

            string satSolveResultPath = Path.GetTempPath() + "satSolveResult.txt";
            
            using (Process command = new Process())
            {
                command.StartInfo.UseShellExecute = false;
                command.StartInfo.FileName = "minisat";
                command.StartInfo.Arguments = satSolveInputPath + " " + satSolveResultPath;
                command.Start();
                command.WaitForExit();
            }

            var result = ParseResult(satSolveResultPath);
            PrintTable(result,OutputPath);
        }
        
        
        private int[][] GetTable(string file)
        {
            int[][] result = new int[9][];
            var rowsList = new List<string>();
            using (var reader = new StreamReader(file))
            {
                var row = reader.ReadLine();
                while (row != null)
                {
                    rowsList.Add(row);
                    row = reader.ReadLine();
                }
            }
            for (int i = 0; i < 9; ++i)
            {
                result[i] = rowsList[i].Select(x =>
                {
                    if (Char.IsDigit(x))
                    {
                        return (int)(x - '0');
                    }
                    else
                    {
                        return -1;
                    }
                }).ToArray();
            }
            return result;
        }
        
        private void PrintTable(int[][] result, string path)
        {
            using (var writer = File.CreateText(path))
            {
                if (result == null)
                {
                    writer.Write("UNSAT");
                }
                else
                {
                    foreach (var row in result)
                    {
                        foreach (var value in row)
                        {
                            writer.Write(value);
                        }
                        writer.Write("\n");
                    }
                }
            }
        }

        private int[][] ParseResult(string path)
        {
            List<int> satContent = null; 
            using (var reader = File.OpenText(path))
            {
                string solveResult = reader.ReadLine();
                if (solveResult == "UNSAT")
                {
                    return null;
                }
                satContent = reader.ReadToEnd().Split(' ').Select(int.Parse).ToList();;
            }

            satContent = satContent.FindAll(x => (x >= 111 && x <= 999));
            var result = new int[9][];
            for (int i = 0; i < 9; ++i)
            {
                result[i] = new int[9];
            }
            
            foreach (var value in satContent)
            {
                result[value / 100 - 1][(value / 10) % 10 - 1] = value % 10;
            }

            return result;
        }

        private void AddInitValues()
        {
            for (int i = 0; i < 9; ++i)
            {
                for (int j = 0; j < 9; ++j)
                {
                    if (SudokuTable[i][j] != -1)
                    {
                        SatFileContent.AddClause(new List<int>() { MakeVar(i + 1, j + 1, SudokuTable[i][j]) });
                    }
                }
            }
        }


        private void FirstRule()
        {
            for (int i = 1; i <= 9; ++i)
            {
                for (int j = 1; j <= 9; ++j)
                {
                    for (int k = 1; k <= 9; ++k)
                    {
                        for (int t = 1; t <= 9; ++t)
                        {
                            if (k != t)
                            {
                                SatFileContent.AddClause(new List<int>() { -MakeVar(i, j, k), -MakeVar(i, j, t) });
                            }
                        }
                    }
                }
            }
        }

        private void SecondRule()
        {
            for (int i = 1; i <= 9; ++i)
            {
                for (int j = 1; j <= 9; ++j)
                {
                    var listToAdd = new List<int>();
                    for (int k = 1; k <= 9; ++k)
                    {
                        listToAdd.Add(MakeVar(i, j, k));
                    }
                    SatFileContent.AddClause(listToAdd);
                }
            }
        }

        private void ThirdRule()
        {
            for (int i = 1; i <= 9; ++i)
            {
                for (int j = 1; j <= 9; ++j)
                {
                    for (int k = 1; k <= 9; ++k)
                    {
                        for (int t = 1; t <= 9; ++t)
                        {
                            if (t != j)
                            {
                                SatFileContent.AddClause(new List<int>() { -MakeVar(i, j, k) ,- MakeVar(i, t, k) });
                            }
                        }
                    }
                }
            }
        }

        private void FourthRule()
        {
            for (int i = 1; i <= 9; ++i)
            {
                for (int j = 1; j <= 9; ++j)
                {
                    for (int k = 1; k <= 9; ++k)
                    {
                        for (int t = 1; t <= 9; ++t)
                        {
                            if (t != i)
                            {
                                SatFileContent.AddClause(new List<int>() { -MakeVar(i, j, k), - MakeVar(t, j, k) });
                            }
                        }
                    }
                }
            }
        }

        private void FifthRule()
        {
            for (int n = 0; n <= 2; ++n)
            {
                for (int m = 0; m <= 2; ++m)
                {
                    for (int i = 1; i <= 3; ++i)
                    {
                        for (int j = 1; j <= 3; ++j)
                        {
                            for (int a = 1; a <= 3; ++a)
                            {
                                for (int b = 1; b <= 3; ++b)
                                {
                                    for (int k = 1; k <= 9; ++k)
                                    {
                                        if (a != i && b != j)
                                        {
                                            SatFileContent.AddClause(new List<int>()
                                                {-MakeVar(3 * n + i, 3 * m + j, k), -MakeVar(3 * n + a, 3 * m + b, k)});
                                        }
                                        
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private int MakeVar (int i, int j, int k)
        {
            return i * 100 + j * 10 + k;
        }
    }
    
    class InputSATSolverFile
    {
        public HashSet<int> Vars { get; set; }
        public List<List<int>> Clauses { get; set; }

        public InputSATSolverFile()
        {
            Vars = new HashSet<int>();
            Clauses = new List<List<int>>();
        }

        public StringBuilder CreateFileContent()
        {
            var result = new StringBuilder("p cnf " + Vars.Count + " " + Clauses.Count() + "\n");
            foreach (var list in Clauses)
            {
                foreach (var value in list)
                {
                    result.Append(value.ToString() + " ");
                }
                result.Append("0\n");
            }
            return result;
        }

        public void AddClause(List<int> clause)
        {
            foreach (var value in clause)
            {
                Vars.Add(value);
            }
            Clauses.Add(clause);
        }

    }
    
}
