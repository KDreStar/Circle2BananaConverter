using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BananaCounter {
	//counter[x][time] => bananaCount
	Dictionary<int, int>[] counter = new Dictionary<int, int>[Consts.CATCH_WIDTH + 1];

	public int realCount = 0;

	public BananaCounter() {
		for (int i = 0; i < counter.Length; i++) {
			counter[i] = new Dictionary<int, int>();
		}
	}

	public int Get(int x, int time) {
		if (counter[x].ContainsKey(time) == false)
			return 0;

		return counter[x][time];
	}

	public void Add(int x, int time) {
		if (counter[x].ContainsKey(time)) {
			if (counter[x][time] == 0)
				realCount++;
			counter[x][time]++;
		}
			
		else {
			counter[x].Add(time, 1);
			realCount++;
		}
	}

	public void Remove(int x, int time) {
		if (counter[x].ContainsKey(time) == false)
			return;

		if (counter[x][time] <= 0)
			return;

		counter[x][time]--;

		if (counter[x][time] == 0)
			realCount--;
	}


	public void Counting(MatchingTable table) {
		List<int> indexs = new List<int>();

		for (int i = 0; i < table.infos.Count; i++)
			indexs.Add(i);

		Counting(table, indexs);
	}

	public void Print() {
		for (int i = 0; i <= 512; i++) {
			if (counter[i].Count == 0)
				continue;

			foreach (KeyValuePair<int, int> pair in counter[i])
				Debug.Write((i, pair.Key, pair.Value));
			Debug.WriteLine("");
		}
	}

	public void Counting(MatchingTable table, int index) {
		Counting(table, new List<int>() { index });
	}

	public void Counting(MatchingTable table, List<int> indexs) {
		for (int i=0; i<indexs.Count; i++) {
			int currentIndex = indexs[i];
			MatchingInfo info = table.infos[currentIndex];

			int x1 = info.firstFruitX;
			int x2 = info.secondFruitX;

			int t1 = info.firstFruitTime;
			int t2 = info.secondFruitTime;

			Add(x1, t1);
			Add(x2, t2);
		}
	}

	public void UnCounting(MatchingTable table, List<int> indexs) {
		for (int i = 0; i < indexs.Count; i++) {
			int currentIndex = indexs[i];
			MatchingInfo info = table.infos[currentIndex];

			int x1 = info.firstFruitX;
			int x2 = info.secondFruitX;

			int t1 = info.firstFruitTime;
			int t2 = info.secondFruitTime;

			Remove(x1, t1);
			Remove(x2, t2);
		}
	}
}
