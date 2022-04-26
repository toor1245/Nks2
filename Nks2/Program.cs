using System;
using System.Collections.Generic;
using System.Linq;

Console.ForegroundColor = ConsoleColor.Cyan;

int[,] schema =
{
    {0, 0, 1, 1, 0, 0, 0, 0},
    {0, 0, 1, 0, 1, 0, 0, 0},
    {0, 0, 0, 1, 1, 0, 0, 0},
    {0, 0, 0, 0, 0, 1, 1, 0},
    {0, 0, 0, 0, 0, 1, 0, 1},
    {0, 0, 0, 0, 0, 0, 1, 1},
    {0, 0, 0, 0, 0, 0, 0, 0},
    {0, 0, 0, 0, 0, 0, 0, 0}
};
double[] probabilities = {0.88, 0.14, 0.93, 0.04, 0.98, 0.56, 0.59, 0.64};
int[] input = {0, 1};
int[] output = {6, 7};
double pSystem;
List<int[]> workableStates;
List<double> pStates;

void ShowWorkableStates()
{
    Console.WriteLine("Workable states (state - p)");
    for (var i = 0; i < workableStates.Count; i++)
    {
        for (var j = 0; j < workableStates[i].Length; j++)
        {
            Console.Write(workableStates[i][j] + " ");
        }

        Console.Write(" P(state) = " + pStates[i]);
        Console.WriteLine();
    }
}

void ShowPSystem()
{
    Console.WriteLine("P(system) = " + pSystem);
}

void ShowSchema()
{
    Console.WriteLine("Schema");
    for (var i = 0; i < schema.GetLength(0); i++)
    {
        for (var j = 0; j < schema.GetLength(1); j++)
        {
            Console.Write(schema[i, j]);
        }

        Console.Write(" P(" + (i + 1) + ") = " + probabilities[i]);
        Console.WriteLine();
    }
}

int[,] GetStates()
{
    int[,] states = new int[schema.GetLength(0), (int) Math.Pow(2, schema.GetLength(0))];
    for (var i = 0; i < states.GetLength(0); i++)
    {
        var a = GetPeriod(i + 1);
        var isInvert = true;
        for (int j = 0, k = 0; j < states.GetLength(1); j++, k++)
        {
            if (k == a)
            {
                isInvert = !isInvert;
                k = 0;
            }

            states[i, j] = isInvert ? 0 : 1;
        }
    }

    return states;
}

void EvaluateWorkable()
{
    workableStates = new List<int[]>();
    int[,] states = GetStates();
    for (var i = 0; i < states.GetLength(1); i++)
    {
        int[] state = new int[states.GetLength(0)];
        for (var j = 0; j < state.Length; j++)
        {
            state[j] = states[j, i];
        }

        if (IsWork(state))
        {
            workableStates.Add(state);
        }
    }
}

void GetPStates()
{
    pStates = workableStates.Select(GetPState).ToList();
}

double GetPState(IReadOnlyList<int> state)
{
    double p_state = 1;
    for (var i = 0; i < state.Count; i++)
    {
        p_state *= state[i] == 0 ? 1.0 - probabilities[i] : probabilities[i];
    }

    return p_state;
}

bool IsWork(IReadOnlyList<int> states) =>
    input.Any(k => output.Select(i => GetPath(k, i, states, new List<int>())).Any(path => path.Count > 0));

List<int> GetPath(int from, int to, IReadOnlyList<int> states, ICollection<int> prev)
{
    List<int> path = new();

    if (states[from] == 0 || states[to] == 0)
    {
        return path;
    }

    if (schema[from, to] == 1)
    {
        path.Add(from);
        path.Add(to);
        return path;
    }

    for (var i = 0; i < schema.GetLength(1); i++)
    {
        if (schema[from, i] != 1 || states[i] != 1 || prev.Contains(i)) continue;
        List<int> nPrev = new(prev) {i};
        List<int> p = GetPath(i, to, states, nPrev);
        if (p.Count <= 0) continue;
        path.Add(from);
        path.AddRange(p);
        return path;
    }

    return path;
}

static int GetPeriod(int column)
{
    return (int) Math.Pow(2, column) / 2;
}

void EvaluatePSystem()
{
    EvaluateWorkable();
    GetPStates();
    pSystem = pStates.Sum();
}

EvaluatePSystem();
ShowSchema();
ShowWorkableStates();
ShowPSystem();