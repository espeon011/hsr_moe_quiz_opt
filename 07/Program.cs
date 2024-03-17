using Google.OrTools.Sat;

public class SolutionPrinter(BoolVar[] isHonest, BoolVar[] hasTreasure) : CpSolverSolutionCallback
{
    private int SolutionCount_ = 0;
    private BoolVar[] IsHonest_ = isHonest;
    private BoolVar[] HasTreasure_ = hasTreasure;

    public override void OnSolutionCallback()
    {
        SolutionCount_++;
        Console.WriteLine(String.Format("Solution #{0}: time = {1:F2} s", SolutionCount_, WallTime()));
        // foreach (BoolVar v in Variables_)
        // {
        //     Console.WriteLine(String.Format("  {0} = {1}", v.ToString(), Value(v)));
        // }

        int numGates = 2;
        string[] names = { "Gold", "Silver" };
        foreach (int i in Enumerable.Range(0, numGates))
        {
            bool isHonest = Value(IsHonest_[i]) != 0;
            bool hasTreasure = Value(HasTreasure_[i]) != 0;
            Console.WriteLine(String.Format("  {0,-6} gate is a {1,-6} {2}", names[i], isHonest ? "Honest" : "Liar", hasTreasure ? "TREASURE" : ""));
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

        int numGates = 2;
        BoolVar[] xs = new BoolVar[numGates];
        xs[0] = model.NewBoolVar(""); // 金の扉は真実を話す
        xs[1] = model.NewBoolVar(""); // 銀の扉は真実を話す
        BoolVar[] ys = new BoolVar[numGates];
        ys[0] = model.NewBoolVar(""); // 金の扉は嘘を話す
        ys[1] = model.NewBoolVar(""); // 銀の扉は嘘を話す
        BoolVar[] vs = new BoolVar[numGates];
        vs[0] = model.NewBoolVar(""); // 金の扉の後ろには財宝がある
        vs[1] = model.NewBoolVar(""); // 銀の扉の後ろには財宝がある
        BoolVar[] ws = new BoolVar[numGates];
        ws[0] = model.NewBoolVar(""); // 金の扉の後ろにはモンスターがいる
        ws[1] = model.NewBoolVar(""); // 銀の扉の後ろにはモンスターがいる
        BoolVar s = model.NewBoolVar(""); // 金の扉は銀の扉のの後ろに財宝があると主張する
        BoolVar t = model.NewBoolVar(""); // 金の扉は銀の扉のの後ろにモンスターがいると主張する
        BoolVar p = model.NewBoolVar(""); // 銀の扉は「金の扉が銀の扉の後ろには財宝があると言う」と主張する
        BoolVar q = model.NewBoolVar(""); // 銀の扉は「金の扉が銀の扉の後ろにはモンスターがいると言う」と主張する

        // 真実を話すか嘘を話すかどちらか一方
        foreach (int i in Enumerable.Range(0, numGates))
        {
            model.Add(xs[i] + ys[i] == 1);
        }

        // 片方は真実を話し, 片方は嘘を話す
        model.AddExactlyOne(xs);
        model.AddExactlyOne(ys);

        // 扉の後ろには財宝かモンスターかどちらか一方がある
        foreach (int i in Enumerable.Range(0, numGates))
        {
            model.Add(vs[i] + ws[i] == 1);
        }

        // 片方の後ろには財宝があり, 片方の後ろにはモンスターがいる
        model.AddExactlyOne(vs);
        model.AddExactlyOne(ws);

        // 金の扉の主張はどちらか一方
        model.Add(s + t == 1);

        // 金の扉が正直者なら銀の扉の後ろに何があるかそのまま伝える
        model.Add(xs[0] + vs[1] <= 1 + s);
        model.Add(xs[0] + ws[1] <= 1 + t);

        // 金の扉が嘘つきなら銀の扉のの後ろにあるものとは逆の主張をする
        model.Add(ys[0] + vs[1] <= 1 + t);
        model.Add(ys[0] + ws[1] <= 1 + s);

        // 銀の扉が金の扉に関していうことはどちらか一方のみ
        model.Add(p + q == 1);

        // 銀の扉が正直者であれば金の扉の主張をそのまま伝える
        model.Add(xs[1] + s <= 1 + p);
        model.Add(xs[1] + t <= 1 + q);

        // 銀の扉が嘘つきであれば金の扉の主張と逆のことを伝える
        model.Add(ys[1] + s <= 1 + q);
        model.Add(ys[1] + t <= 1 + p);

        // 銀の扉は「(金の扉は)この後ろにあるのは財宝であると言うのだろう」と答えた
        model.Add(p == 1);

        CpSolver solver = new();
        SolutionPrinter cb = new(xs, vs);
        solver.StringParameters = "enumerate_all_solutions:true";
        solver.Solve(model, cb);

        Console.WriteLine($"Number of solutions found: {cb.SolutionCount()}");
    }
}

