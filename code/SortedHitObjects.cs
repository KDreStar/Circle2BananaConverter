using OsuParsers.Beatmaps.Objects;

public class SortedHitObjects {
    public PosXes[] posXes;

    private int realCount = 0;
    public int RealCount {
        get {return realCount;}
    }

    public SortedHitObjects(List<HitObject> hitObjects) {
        //x: 0~512
        posXes = new PosXes[Consts.CATCH_WIDTH + 1];

        for (int i=0; i<=Consts.CATCH_WIDTH; i++)
            posXes[i] = new PosXes();

        for (int i=0; i<hitObjects.Count; i++) {
            HitObject hitObject = hitObjects[i];

            if (hitObject is not HitCircle)
                continue;
            
            int x = (int)hitObject.Position.X;
            int time = hitObject.StartTime;

            posXes[x].times.Add(time);
        }


        for (int i=0; i<=Consts.CATCH_WIDTH; i++) {
            posXes[i].Initialize();
            realCount += posXes[i].times.Count;
        }
    }

    public void Print() {
        for (int i=0; i<=Consts.CATCH_WIDTH; i++) {
            List<int> times = posXes[i].times;
            for (int j=0; j<times.Count; j++) {
                Console.Write((i, times[j]));
            }
        }
        Console.WriteLine();
    }

    public void Print2() {
        for (int i=0; i<=Consts.CATCH_WIDTH; i++) {
            List<int> times = posXes[i].possibleTimes;
            for (int j=0; j<times.Count; j++) {
                Console.Write((i, times[j]));
            }
        }
        Console.WriteLine();
    }

    public bool Visit(int x, int time) {
        if (posXes[x].Visit(time) == false)
            return false;
        
        realCount--;

        return true;
    }

    public void UnVisit(int x, int time) {
        if (posXes[x].timeCounts.ContainsKey(time) == false)
            return;

        realCount++;
        posXes[x].UnVisit(time);
    }

    public List<int> GetTimes(int x) {
        return posXes[x].times;
    }

    public List<int> GetPossibleTimes(int x) {
        return posXes[x].possibleTimes;
    }
}