using Google.OrTools.Sat;

public class SolutionPrinter(BoolVar[] isGood, BoolVar[] isBad, BoolVar[] isLiar) : CpSolverSolutionCallback
{
    private int SolutionCount_ = 0;
    private BoolVar[] IsGood_ = isGood;
    private BoolVar[] IsBad_ = isBad;
    private BoolVar[] IsLiar_ = isLiar;

    public override void OnSolutionCallback()
    {
        SolutionCount_++;
        Console.WriteLine(String.Format("Solution #{0}: time = {1:F2} s", SolutionCount_, WallTime()));

        string[] names = { "Howard", "Philip", "Joyce" };
        foreach (int i in Enumerable.Range(0, 3))
        {
            bool isGood = Value(IsGood_[i]) != 0;
            bool isBad = Value(IsBad_[i]) != 0;
            bool isLiar = Value(IsLiar_[i]) != 0;
            Console.WriteLine(String.Format("  {0,-7} is a {1}{2}{3}", names[i], isGood ? "good guy" : "", isBad ? "bad guy" : "", isLiar ? "liar" : ""));
        }
    }

    public int SolutionCount()
    {
        return SolutionCount_;
    }
}

public class Problem
{
    static void Main()
    {
        CpModel model = new();

        int numHuman = 3;

        BoolVar howardIsGood = model.NewBoolVar("Howard is a good guy");
        BoolVar philipIsGood = model.NewBoolVar("Philip is a good guy");
        BoolVar joyceIsGood = model.NewBoolVar("Joyce is a good guy");
        BoolVar[] isGood = new BoolVar[] { howardIsGood, philipIsGood, joyceIsGood };

        BoolVar howardIsBad = model.NewBoolVar("Howard is a bad guy");
        BoolVar philipIsBad = model.NewBoolVar("Philip is a bad guy");
        BoolVar joyceIsBad = model.NewBoolVar("Joyce is a bad guy");
        BoolVar[] isBad = new BoolVar[] { howardIsBad, philipIsBad, joyceIsBad };

        BoolVar howardIsLiar = model.NewBoolVar("Howard is a liar");
        BoolVar philipIsLiar = model.NewBoolVar("Philip is a liar");
        BoolVar joyceIsLiar = model.NewBoolVar("Joyce is a liar");
        BoolVar[] isLiar = new BoolVar[] { howardIsLiar, philipIsLiar, joyceIsLiar };

        // 善人か悪人か嘘つきのどれか
        foreach (int i in Enumerable.Range(0, numHuman))
        {
            model.Add(isGood[i] + isBad[i] + isLiar[i] == 1);
        }

        // 善人, 悪人, 嘘つきはそれぞれ 1 人
        model.AddExactlyOne(isGood);
        model.AddExactlyOne(isBad);
        model.AddExactlyOne(isLiar);

        // ジョイスはフィリップが善人ではなく悪人であると主張している
        model.Add(joyceIsGood <= 1 - philipIsGood);
        model.Add(joyceIsGood <= philipIsBad);
        model.Add(joyceIsBad <= 1 - philipIsGood + philipIsBad);

        // フィリップはハワードとジョイスのどちらか 1 人は善人であると主張している
        model.Add(philipIsGood <= howardIsGood + joyceIsGood);
        model.Add(philipIsBad <= 1 - howardIsGood);
        model.Add(philipIsBad <= 1 - joyceIsGood);

        CpSolver solver = new();
        SolutionPrinter cb = new(isGood, isBad, isLiar);
        solver.StringParameters = "enumerate_all_solutions:true";
        solver.Solve(model, cb);

        Console.WriteLine($"Number of solutions found: {cb.SolutionCount()}");
    }
}
