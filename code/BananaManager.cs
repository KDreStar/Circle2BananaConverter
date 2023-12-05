
public class BananaManager {

    //바나나 2개만 사용하는 걸로(일단 구현)
    //max length = 8
    public List<int> rngNumbers;
    LegacyRandom rng;

    public int rngCount = 0;

    public BananaManager() {
        rngNumbers = new List<int>();
        rng = new LegacyRandom(Consts.RNG_SEED);

        for (int i=0; i<Consts.MAX_RNG_LENGTH; i++) {
            rngNumbers.Add(Next());
        }

        rngCount = 0;
    }

    private int Next() {
        double temp = rng.NextDouble() * Consts.CATCH_WIDTH;

        int x = (int)Math.Round(temp);

        rngCount++;

        return x;
    }

    public void ProceedForDroplet() {
        rngNumbers.RemoveAt(0);
        rngNumbers.Add(Next());
    }

    public void ProceedForBanana() {
        for (int i=0; i<Consts.MAX_RNG_LENGTH; i++) {
            rngNumbers.RemoveAt(0);
            rngNumbers.Add(Next());
        }
    }

    public int FirstBananaX {
        get {return rngNumbers[0];}
    }

    public int SecondBananaX {
        get {return rngNumbers[4];}
    }
}