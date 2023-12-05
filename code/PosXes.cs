
public class PosXes {
    public List<int> times = new List<int>();

    //<time, count>
    public Dictionary<int, int> timeCounts = new Dictionary<int, int>();

    public List<int> possibleTimes = new List<int>();

    public void Initialize() {
        times.Sort();
        
        for (int i=0; i<times.Count; i++) {
            int key = times[i];

            if (timeCounts.ContainsKey(key))
                timeCounts[key]++;
            else
                timeCounts.Add(key, 1);
        }

        UpdatePossibleTimes();
    }

    public bool Visit(int time) {
        if (timeCounts.ContainsKey(time) == false)
            return false;

        if (timeCounts[time] <= 0)
            return false;

        timeCounts[time]--;
        UpdatePossibleTimes();
        return true;
    }

    public void UnVisit(int time) {
        if (timeCounts.ContainsKey(time) == false)
            return;

        timeCounts[time]++;
        UpdatePossibleTimes();
    }

    public void UpdatePossibleTimes() {
        possibleTimes.Clear();

        for (int i=0; i<times.Count; i++) {
            int time = times[i];

            //시간 개수만큼 Add
            for (int j=0; j<timeCounts[time]; j++)
                possibleTimes.Add(time);
        }
    }
}