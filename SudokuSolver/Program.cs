using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = new InputSATSolverFile();
            AddInitValues(input, GetTable(args[0]));
            FirstRule(input);
            SecondRule(input);
            ThirdRule(input);
            FourthRule(input);
            FifthRule(input);
            /*
            var table = GetTable(args[0]);
            for (int i = 0; i < 9; ++i)
            {
                for (int j = 0; j < 9; ++j)
                {
                    Console.Write(table[i][j]);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
            Console.WriteLine(input.CreateFileContent());
            Console.WriteLine(input.Clauses.Count);
            Console.WriteLine(input.Vars.Count);
            */
            Console.ReadKey();
        }

        static int[][] GetTable(string file)
        {
            int[][] result = new int[9][];
            var rowsList = new List<String>();
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

        static void AddInitValues(InputSATSolverFile file, int[][] table)
        {
            for (int i = 0; i < 9; ++i)
            {
                for (int j = 0; j < 9; ++j)
                {
                    if (table[i][j] != -1)
                    {
                        file.AddClause(new List<int>() { MakeVar(i + 1, j + 1, table[i][j]) });
                    }
                }
            }
        }


        static void FirstRule(InputSATSolverFile file)
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
                                file.AddClause(new List<int>() { -MakeVar(i, j, k), - MakeVar(i, j, t) });
                            }
                        }
                    }
                }
            }
        }

        static void SecondRule(InputSATSolverFile file)
        {
            for (int i = 1; i <= 9; ++i)
            {
                for (int j = 1; i <= 9; ++i)
                {
                    var listToAdd = new List<int>();
                    for (int k = 1; k <= 9; ++k)
                    {
                        listToAdd.Add(MakeVar(i, j, k));
                    }
                    file.AddClause(listToAdd);
                }
            }
        }

        static void ThirdRule(InputSATSolverFile file)
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
                                file.AddClause(new List<int>() { -MakeVar(i, j, k) ,- MakeVar(i, t, k) });
                            }
                        }
                    }
                }
            }
        }

        static void FourthRule(InputSATSolverFile file)
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
                                file.AddClause(new List<int>() { -MakeVar(i, j, k), - MakeVar(t, j, k) });
                            }
                        }
                    }
                }
            }
        }

        static void FifthRule(InputSATSolverFile file)
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
                                        for (int t = 1; t <= 9; ++t)
                                        {
                                            if (a != i && b != j && t != k)
                                            {
                                                file.AddClause(new List<int>() { -MakeVar(3 * n + i, 3 * m + j, k), -MakeVar(3 * n + a, 3 * m + b, t) });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        static int MakeVar (int i, int j, int k)
        {
            return i * 100 + j * 10 + k;
        }
    }
}
