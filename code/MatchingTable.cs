public class MatchingTable {
    public List<MatchingInfo> infos = new List<MatchingInfo>();

    public void Add(MatchingInfo info) {
        infos.Add(info);
    }

    public void RemoveLast() {
        infos.RemoveAt(infos.Count - 1);
    }

    public void Set(MatchingTable table) {
        List<MatchingInfo> infos = table.infos;

        this.infos.Clear();
        
        for (int i=0; i<infos.Count; i++)
            this.infos.Add(infos[i]);
    }

    public int GetLastRNGCount() {
        return infos[infos.Count - 1].rngCount;
    }
}

public class MatchingInfo {
    public int rngCount;
    public int firstFruitX;
    public int firstFruitTime;

    public int secondFruitX;

    public int secondFruitTime;


    public MatchingInfo(int rngCount, int firstFruitX, int firstFruitTime, int secondFruitX, int secondFruitTime) {
        this.rngCount = rngCount;
        this.firstFruitX = firstFruitX;
        this.firstFruitTime = firstFruitTime;
        this.secondFruitX = secondFruitX;
        this.secondFruitTime = secondFruitTime;
    }

}