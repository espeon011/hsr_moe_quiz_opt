using Google.OrTools.Sat;

public class SolutionPrinter(BoolVar[] isHonest, BoolVar[] isLiar, BoolVar[] isCriminal) : CpSolverSolutionCallback
{
    private int SolutionCount_ = 0;
    private BoolVar[] IsHonest_ = isHonest;
    private BoolVar[] IsLiar_ = isLiar;
    private BoolVar[] IsCriminal_ = isCriminal;

    public override void OnSolutionCallback()
    {
        SolutionCount_++;
        Console.WriteLine(String.Format("Solution #{0}: time = {1:F2} s", SolutionCount_, WallTime()));

        string[] names = { "Jack", "Chris", "Eric" };
        foreach (int i in Enumerable.Range(0, 3))
        {
            bool isHonest = Value(IsHonest_[i]) != 0;
            bool isLiar = Value(IsLiar_[i]) != 0;
            bool isCriminal = Value(IsCriminal_[i]) != 0;
            Console.WriteLine(String.Format("  {0,-5} is a {1,-6} {2}", names[i], isHonest ? "Honest" : "Liar", isCriminal ? "Criminal" : ""));
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

        BoolVar jackIsHonest = model.NewBoolVar("Jack is honest");
        BoolVar chrisIsHonest = model.NewBoolVar("Chris is honest");
        BoolVar ericIsHonest = model.NewBoolVar("Eric is honest");
        BoolVar[] isHonest = new BoolVar[] { jackIsHonest, chrisIsHonest, ericIsHonest };

        BoolVar jackIsLiar = model.NewBoolVar("Jack is liar");
        BoolVar chrisIsLiar = model.NewBoolVar("Chris is liar");
        BoolVar ericIsLiar = model.NewBoolVar("Eric is liar");
        BoolVar[] isLiar = new BoolVar[] { jackIsLiar, chrisIsLiar, ericIsLiar };

        BoolVar jackIsCriminal = model.NewBoolVar("Jack is criminal");
        BoolVar chrisIsCriminal = model.NewBoolVar("Chris is criminal");
        BoolVar ericIsCriminal = model.NewBoolVar("Eric is criminal");
        BoolVar[] isCriminal = new BoolVar[] { jackIsCriminal, chrisIsCriminal, ericIsCriminal };

        // 正直者か嘘つきのどちらか
        foreach (int i in Enumerable.Range(0, numHuman))
        {
            model.Add(isHonest[i] + isLiar[i] == 1);
        }

        // ジャックはクリスが犯人だと主張している
        model.Add(jackIsHonest == chrisIsCriminal);

        // 犯人だけが正直者
        foreach (int i in Enumerable.Range(0, numHuman))
        {
            model.Add(isCriminal[i] == isHonest[i]);
        }


        CpSolver solver = new();
        SolutionPrinter cb = new(isHonest, isLiar, isCriminal);
        solver.StringParameters = "enumerate_all_solutions:true";
        solver.Solve(model, cb);

        Console.WriteLine($"Number of solutions found: {cb.SolutionCount()}");
    }
}
