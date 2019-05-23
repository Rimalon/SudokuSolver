using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SudokuSolver
{
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
            var result = new StringBuilder("p cnf " + Vars.Count.ToString() + " " + Clauses.Count().ToString() + "\n");
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
